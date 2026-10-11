using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.Core;
namespace Kamilunavo.OneMoreFloor.Monetization
{
    [ExecuteAlways]
    public sealed class RewardedVideos:MonoBehaviour
    {
        private FloorCourse _game;
        private MonetizationConfig _config;
        private RewardedAd _ad;
        private bool _prepared,_initialized,_loading,_initializing;
        public bool SdkReady=>_initialized;
        private readonly AdLoadGeneration _loads=new();
        private double _loadedAt,_retryAt;
        private string _session="";
        private bool _rewarded,_fullscreenRecorded;
        public bool IsPresenting {get;private set;}
        public bool PrivacyRequired=>_prepared && ConsentInformation.PrivacyOptionsRequirementStatus==PrivacyOptionsRequirementStatus.Required;
        public event Action Changed;
        public string Status {get;private set;}="";
        public bool CanWatch=>_game!=null&&_game.CommerceForeground&&!_game.Presentation.IsLocked&&FloorSave.Writable && Application.isMobilePlatform && !IsPresenting && _ad!=null && _ad.CanShowAd() && Time.realtimeSinceStartupAsDouble-_loadedAt<3500 && ConsentInformation.CanRequestAds() && RewardRules.CanClaim(_game.PersistentProfile,DateTime.UtcNow);
        private string T(string de,string en)=>_game.Hud.T(de,en);
        private void Main(Action action)=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(this!=null)action();});
        public void Initialize(FloorCourse game)
        {
            _game=game;_config=MonetizationConfig.Load();
            Status=T("Video gerade nicht verfügbar","Video currently unavailable");
            PrepareBackground();
        }
        public void PrepareBackground()
        {
            if(!Application.isMobilePlatform||_game==null||!_game.PersistentProfile.TutorialCompleted||_game.TutorialActive)return;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(!string.IsNullOrEmpty(FloorSave.QaKey))return;
#endif
            if(ConsentInformation.CanRequestAds())AfterConsent();
        }
        public void Prepare()
        {
            if(!Application.isMobilePlatform || _game==null||_game.Hud?.AdConsentOpen!=true||!_game.CommerceForeground||_game.Presentation.IsLocked||Time.realtimeSinceStartupAsDouble<_retryAt)return;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(!string.IsNullOrEmpty(FloorSave.QaKey))return;
#endif
            if(!_game.Hud.ShopOpen&&CommerceRules.AdsRemoved(_game.PersistentProfile))return;
            if(_prepared){if(_initialized)Load();return;}
            _prepared=true;SetStatus(T("Video wird vorbereitet …","Preparing video …"));
            ConsentInformation.Update(new ConsentRequestParameters(),error=>Main(()=>
            {
                if(error!=null){_prepared=false;_retryAt=Time.realtimeSinceStartupAsDouble+60;AfterConsent();return;}
                if(!TryPresentConsent(()=>ConsentForm.LoadAndShowConsentFormIfRequired(formError=>Main(()=>{IsPresenting=false;_game.ReleaseCommerce(this);AfterConsent();})))){_prepared=false;AfterConsent();}
            }));
        }
        private bool TryPresentConsent(Action show)
        {
            // A late UMP response may only present on a safe home, shop or successful-result page.
            if(_game==null || _game.Hud?.AdConsentOpen!=true || !_game.Paused || _game.Store?.IsPresenting==true||!_game.AcquireCommerce(this))return false;
            _game.Save();IsPresenting=true;Changed?.Invoke();
            try{show();return true;}catch(Exception){IsPresenting=false;_game.ReleaseCommerce(this);_prepared=false;Changed?.Invoke();return false;}
        }
        private void AfterConsent()
        {
            Changed?.Invoke();
            if(!ConsentInformation.CanRequestAds()){SetStatus(T("Video nicht verfügbar – Datenschutzeinstellungen prüfen","Video unavailable – check privacy settings"));return;}
            if(_initialized){Load();return;}
            if(_initializing)return;_initializing=true;
            MobileAds.Initialize(status=>Main(()=>
            {
                _initializing=false;if(status==null){SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));return;}
                _initialized=true;Load();Changed?.Invoke();
            }));
        }
        private void Load()
        {
            if(!_initialized || _loading || IsPresenting || !ConsentInformation.CanRequestAds() || Time.realtimeSinceStartupAsDouble<_retryAt)return;
            if(_ad!=null && _ad.CanShowAd() && Time.realtimeSinceStartupAsDouble-_loadedAt<3500)return;
            _ad?.Destroy();_ad=null;
            var id=_config.RewardedId;
            if(string.IsNullOrWhiteSpace(id)){SetStatus(T("Video gerade nicht verfügbar","Video currently unavailable"));return;}
            _loading=true;var generation=_loads.Begin();
            RewardedAd.Load(id,new AdRequest(),(ad,error)=>
            {
                if(this==null){ad?.Destroy();return;}
                MobileAdsEventExecutor.ExecuteInUpdate(()=>
                {
                    if(this==null||!_loads.IsCurrent(generation) || !ConsentInformation.CanRequestAds()){ad?.Destroy();return;}
                    _loading=false;
                    if(error!=null || ad==null){ad?.Destroy();_retryAt=Time.realtimeSinceStartupAsDouble+60;SetStatus(T("Kein Video verfügbar – später erneut versuchen","No video available – try later"));return;}
                    _ad=ad;_loadedAt=Time.realtimeSinceStartupAsDouble;
                    ad.OnAdFullScreenContentOpened+=()=>Main(()=>RecordFullscreenOpen(ad));
                    ad.OnAdFullScreenContentClosed+=()=>Main(()=>Finish(ad,false));
                    ad.OnAdFullScreenContentFailed+=failure=>Main(()=>Finish(ad,true));
                    SetStatus(T("Freiwilliges Video: +50 Münzen","Optional video: +50 coins"));
                });
            });
        }
        public void Watch()
        {
            if(!CanWatch || !_game.Hud.ShopOpen||!_game.AcquireCommerce(this))return;
            _game.Save();IsPresenting=true;_session=Guid.NewGuid().ToString("N");_rewarded=false;_fullscreenRecorded=false;
            var session=_session;var ad=_ad;Changed?.Invoke();
            try{ad.Show(reward=>Main(()=>
            {
                // Only this specific SDK completion callback can award this view, once.
                if(_session!=session || _rewarded)return;
                try
                {
                    if(RewardRules.Fulfill(_game.PersistentProfile,session,DateTime.UtcNow,FloorSave.Save))
                    {_rewarded=true;SetStatus(T("+50 Münzen erhalten","Received +50 coins"));_game.RefreshProfile();}
                }
                catch(Exception){SetStatus(T("Belohnung konnte nicht gespeichert werden","Reward could not be saved"));}
            }));}
            catch(Exception){Finish(ad,true);}
        }
        private void RecordFullscreenOpen(RewardedAd shown)
        {
            if(shown==null||_ad!=shown||!IsPresenting||_fullscreenRecorded)return;
            _fullscreenRecorded=true;_game.Interstitials?.VoluntaryOpened();
        }
        private void Finish(RewardedAd ad,bool failed)
        {
            if(_ad!=ad)return;
            IsPresenting=false;_game.ReleaseCommerce(this);
            SetStatus(failed?T("Video konnte nicht geöffnet werden","Video could not open"):_rewarded?T("+50 Münzen erhalten","Received +50 coins"):T("Video beendet – keine Prämie erhalten","Video ended – no reward received"));
            // Keep the session until the next Watch; delayed SDK reward callbacks remain valid.
            _ad=null;ad.Destroy();_retryAt=Time.realtimeSinceStartupAsDouble+2;
        }
        public void ShowPrivacy()
        {
            if(!PrivacyRequired || IsPresenting||_game.Hud?.AdConsentOpen!=true||!_game.Hud.ShopOpen||!_game.AcquireCommerce(this))return;
            _game.Save();IsPresenting=true;_loads.Invalidate();_loading=false;_retryAt=0;_ad?.Destroy();_ad=null;_game.Interstitials?.InvalidateConsent();Changed?.Invoke();
            try{ConsentForm.ShowPrivacyOptionsForm(error=>Main(()=>{IsPresenting=false;_game.ReleaseCommerce(this);AfterConsent();}));}catch(Exception){IsPresenting=false;_game.ReleaseCommerce(this);SetStatus(T("Datenschutz gerade nicht verfügbar","Privacy currently unavailable"));}
        }
        private void SetStatus(string status){Status=status;Changed?.Invoke();}
        private void OnDestroy(){_game?.ReleaseCommerce(this);_loads.Invalidate();_ad?.Destroy();_ad=null;}
    }
}
