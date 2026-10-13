using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.Monetization
{
 // Preload can run in Update; presentation can only be called by a new successful result.
 [ExecuteAlways]
 public sealed class RoundInterstitials:MonoBehaviour
 {
  FloorCourse game;MonetizationConfig config;InterstitialAd ad;
  readonly AdLoadGeneration loads=new();bool loading,impressionRecorded;
  double loadedAt,retryAt;
  Func<double> clock=()=>Time.realtimeSinceStartupAsDouble;
  Func<bool> consent=()=>ConsentInformation.CanRequestAds();
  bool NativeAdsAllowed{get{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   if(!string.IsNullOrEmpty(FloorSave.QaKey))return false;
#endif
   return Application.isMobilePlatform;
  }}
  public InterstitialPolicy Policy{get;private set;}
  public bool IsPresenting{get;private set;}
  public void Initialize(FloorCourse owner)
  {
   if(game==owner&&Policy!=null)return;
   if(game!=null){game.SuccessfulRound-=SuccessfulResult;if(game.Videos!=null)game.Videos.Changed-=Prepare;game.ReleaseCommerce(this);Discard();IsPresenting=false;}
   game=owner;config=MonetizationConfig.Load();Policy=new InterstitialPolicy(clock());
   game.SuccessfulRound+=SuccessfulResult;if(game.Videos!=null)game.Videos.Changed+=Prepare;
   Prepare();
  }
  void SuccessfulResult(string token,bool daily,string day)
  {
   if(game==null||game.TutorialActive||!FloorSave.Writable)return;
   var commerce=game.PersistentProfile.Commerce;
   if(daily&&day!=FloorRules.Day(DateTime.UtcNow))return;
   if(!Policy.TryCountSuccess(token,false,daily,day,commerce.InterstitialDailyDay,d=>{
    string old=commerce.InterstitialDailyDay;commerce.InterstitialDailyDay=d;
    try{FloorSave.Save(game.PersistentProfile);}catch{commerce.InterstitialDailyDay=old;throw;}
   }))return;
   // The opportunity is consumed even when unavailable: no pending show, retry or popup.
   if(NativeAdsAllowed)TryPresentAtBoundary(()=>ad!=null&&ad.CanShowAd()&&clock()-loadedAt<3500,()=>ad.Show());
  }
  internal bool TryPresentAtBoundary(Func<bool> ready,Action show)
  {
   if(game==null||Policy==null||ready==null||show==null||!FloorSave.Writable)return false;
   try{if(!Policy.Eligible(clock(),CommerceRules.AdsRemoved(game.PersistentProfile),game.TutorialActive,game.Hud?.ResultOpen==true,game.IsTransferring,game.CommerceForeground,game.Presentation.IsLocked||game.Store?.IsPresenting==true||game.Videos?.IsPresenting==true||IsPresenting,consent(),ready()))return false;}catch{return false;}
   if(!game.AcquireCommerce(this))return false;
   IsPresenting=true;impressionRecorded=false;game.Save();
   try{show();return true;}catch{Finish(ad);return false;}
  }
  void Update()
  {
   if(!Application.isPlaying||!NativeAdsAllowed||game==null||IsPresenting)return;
   try{
    if(clock()<retryAt)return;
    if(CommerceRules.AdsRemoved(game.PersistentProfile)||!consent()){Discard();return;}
    Prepare();
   }catch{LoadFailed();}
  }
  void Prepare()
  {
   // The platform/QA gate must run before touching UMP or any native ad object.
   if(!NativeAdsAllowed||game==null||IsPresenting||loading)return;
   try{
    if(!game.PersistentProfile.TutorialCompleted||game.TutorialActive||game.Presentation.IsLocked||!game.CommerceForeground||game.Videos?.SdkReady!=true||clock()<retryAt||!consent()||CommerceRules.AdsRemoved(game.PersistentProfile))return;
    if(ad!=null&&ad.CanShowAd()&&clock()-loadedAt<3500)return;
    Discard();if(string.IsNullOrWhiteSpace(config.InterstitialId))return;
    loading=true;var generation=loads.Begin();
    InterstitialAd.Load(config.InterstitialId,new AdRequest { Extras = { { "npa", "1" } } },(loaded,error)=>{
     if(this==null){DestroyNative(loaded);return;}
     MobileAdsEventExecutor.ExecuteInUpdate(()=>{
      if(this==null){DestroyNative(loaded);return;}
      try{
       if(!loads.IsCurrent(generation)){DestroyNative(loaded);return;}
       if(!NativeAdsAllowed||!consent()||CommerceRules.AdsRemoved(game.PersistentProfile)){loading=false;DestroyNative(loaded);return;}
       loading=false;
       if(error!=null||loaded==null){DestroyNative(loaded);LoadFailed();return;}
       ad=loaded;loadedAt=clock();
       loaded.OnAdFullScreenContentOpened+=()=>Main(()=>RecordShown(loaded));
       loaded.OnAdImpressionRecorded+=()=>Main(()=>RecordShown(loaded));
       loaded.OnAdFullScreenContentClosed+=()=>Main(()=>Finish(loaded));
       loaded.OnAdFullScreenContentFailed+=failure=>Main(()=>Finish(loaded));
      }catch{if(ad!=loaded)DestroyNative(loaded);LoadFailed();}
     });
    });
   }catch{LoadFailed();}
  }
  void LoadFailed(){Discard();retryAt=clock()+60;}
  static void DestroyNative(InterstitialAd value){try{value?.Destroy();}catch{}}
  public void InvalidateConsent(){Discard();retryAt=0;}
  public void VoluntaryOpened(){Policy?.DelayAfterVoluntaryAd(clock());}
  void RecordShown(InterstitialAd shown){if(ad!=shown||!IsPresenting||impressionRecorded)return;impressionRecorded=true;Policy.RecordImpression(clock());}
  void Main(Action action)=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(this!=null)action();});
  void Finish(InterstitialAd finished)
  {
   if(ad!=finished)return;
   IsPresenting=false;game?.ReleaseCommerce(this);ad=null;DestroyNative(finished);retryAt=clock()+2;
  }
  void Discard(){loads.Invalidate();loading=false;var owned=ad;ad=null;DestroyNative(owned);}
  void OnDestroy(){if(game!=null){game.SuccessfulRound-=SuccessfulResult;if(game.Videos!=null)game.Videos.Changed-=Prepare;game.ReleaseCommerce(this);}Discard();}
 }
}
