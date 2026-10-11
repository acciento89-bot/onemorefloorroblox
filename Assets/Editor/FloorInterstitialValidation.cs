#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Monetization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Kamilunavo.OneMoreFloor.Editor
{
 public static class FloorInterstitialValidation
 {
  static int checks;
  const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
  static void Require(bool value,string label){checks++;if(!value)throw new InvalidOperationException("Floor interstitial: "+label);}
  static object Invoke(object target,string method,params object[] args)=>target.GetType().GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public).Invoke(target,args);
  static object Field(object target,string field)=>target.GetType().GetField(field,Hidden).GetValue(target);
  static void Set(object target,string field,object value)=>target.GetType().GetField(field,Hidden).SetValue(target,value);
  static void DropModal(UI.FloorHud hud){var modal=(RectTransform)Field(hud,"_modal");if(modal!=null)UnityEngine.Object.DestroyImmediate(modal.gameObject);Set(hud,"_modal",null);}
  static int Successful(object policy)=>(int)policy.GetType().GetProperty("SuccessfulRounds").GetValue(policy);
  static string RemoveId=>(string)typeof(CommerceRules).GetField("RemoveAds").GetRawConstantValue();
  static bool Removed(FloorProfile p)=>(bool)typeof(CommerceRules).GetMethod("AdsRemoved").Invoke(null,new object[]{p});
  public static void Validate()
  {
   checks=0;var errors=new List<string>();Application.LogCallback log=(message,stack,kind)=>{if(kind==LogType.Error||kind==LogType.Exception||kind==LogType.Assert)errors.Add(message);};Application.logMessageReceived+=log;
   string priorQa=FloorSave.QaKey;FloorSave.QaKey="kamilunavo.onemorefloor.commerceqa."+Guid.NewGuid().ToString("N");FloorSave.Load();
   try{BaselineRed();RunPolicy(typeof(CommerceRules).Assembly.GetType("Kamilunavo.OneMoreFloor.Monetization.InterstitialPolicy"));ValidateEntitlements();ValidateProductionBoundary();Require(errors.Count==0,"no logged native/editor errors: "+string.Join("; ",errors));Debug.Log("FLOOR_INTERSTITIAL_PASS checks="+checks+" loggedErrors=0");}
   finally{PlayerPrefs.DeleteKey(FloorSave.Key);FloorSave.QaKey=priorQa;FloorSave.Load();Application.logMessageReceived-=log;}
  }
  public static void BaselineRed()
  {
   var rules=typeof(Monetization.CommerceRules);
   if(rules.GetField("RemoveAds",BindingFlags.Public|BindingFlags.Static)==null)
    throw new InvalidOperationException("FLOOR_INTERSTITIAL_RED: permanent Remove Ads product is missing");
   if(rules.Assembly.GetType("Kamilunavo.OneMoreFloor.Monetization.InterstitialPolicy")==null)
    throw new InvalidOperationException("FLOOR_INTERSTITIAL_RED: successful-round and session-time gate is missing");
   Debug.Log("FLOOR_INTERSTITIAL_BASELINE_CONTRACT_PRESENT");
  }
 static void RunPolicy(Type t)
 {
  object p=Activator.CreateInstance(t,new object[]{0d});
  Func<string,bool,bool,string,string,Action<string>,bool> count=(token,tutorial,daily,day,prior,persist)=>(bool)t.GetMethod("TryCountSuccess").Invoke(p,new object[]{token,tutorial,daily,day,prior,persist});
  Func<double,bool[],bool> eligible=(now,flags)=>(bool)t.GetMethod("Eligible").Invoke(p,new object[]{now,flags[0],flags[1],flags[2],flags[3],flags[4],flags[5],flags[6],flags[7]});
  bool[] valid={false,false,true,false,true,false,true,true};
  Require(!eligible(200,valid),"zero rounds never eligible");
  Require(!count("tutorial",true,false,"","",null),"tutorial never counts");
  Require(!count(" ",false,false,"","",null),"empty round token fails closed");
  for(int i=0;i<3;i++){Require(count("normal"+i,false,false,"","",null),"each legitimate normal success counts");Require(!count("normal"+i,false,false,"","",null),"duplicate result callback counts once");if(i<2)Require(!eligible(200,valid),"first two successes never eligible");}
  Require(!eligible(179.999,valid),"session 180s threshold");Require(eligible(180,valid),"third completed round at 180s eligible");
  for(int i=0;i<8;i++){var f=(bool[])valid.Clone();f[i]=!f[i];Require(!eligible(200,f),"independent availability/ownership/tutorial/result/flight/focus/lock/consent guard "+i);}
  Require(!eligible(double.NaN,valid)&&!eligible(double.PositiveInfinity,valid)&&!eligible(-1,valid),"invalid clock fails closed");
  t.GetMethod("RecordImpression").Invoke(p,new object[]{200d});Require(!eligible(400,valid),"three successes required between shown ads");
  for(int i=0;i<3;i++)Require(count("later"+i,false,false,"","",null),"fresh successful replay run counts");
  Require(!eligible(379.999,valid),"last shown 180s threshold");Require(eligible(380,valid),"new successes after cooldown eligible");
  var delay=t.GetMethod("DelayAfterVoluntaryAd");Require(delay!=null,"voluntary fullscreen delays forced-ad clock");
  int successful=(int)t.GetProperty("SuccessfulRounds").GetValue(p),remaining=(int)t.GetProperty("RoundsSinceShown").GetValue(p);
  delay.Invoke(p,new object[]{400d});Require(!eligible(579.999,valid)&&eligible(580,valid),"voluntary opened enforces a fresh 180s cooldown");
  Require((int)t.GetProperty("SuccessfulRounds").GetValue(p)==successful&&(int)t.GetProperty("RoundsSinceShown").GetValue(p)==remaining,"voluntary ad preserves successful-round counts");
  delay.Invoke(p,new object[]{double.NaN});delay.Invoke(p,new object[]{double.PositiveInfinity});delay.Invoke(p,new object[]{399d});
  Require((double)t.GetProperty("LastShownAt").GetValue(p)==400d,"invalid or older voluntary clock cannot rewind pacing");
  string saved="";Require(count("daily",false,true,"2026-10-11","",d=>saved=d)&&saved=="2026-10-11","first daily success persists earned day");
  Require(!count("dailyReplay",false,true,"2026-10-11","",d=>saved=d),"daily route replay blocked even with stale argument");
  Require(!count("oldDay",false,true,"2026-10-10","2026-10-11",d=>saved=d),"older daily day cannot count");
  Require(!count("futurePayload",false,true,"broken","",d=>saved=d),"invalid daily date fails closed");
  Require(!count("noPersistence",false,true,"2026-10-12","",null),"daily never counts without persistence");
  Require(!count("diskFailure",false,true,"2026-10-12","",d=>{throw new Exception("disk");}),"failed persistence never counts");
  Require(count("diskFailure",false,true,"2026-10-12","2026-10-11",d=>saved=d),"persistence retry can count later successful result");
  object reload=Activator.CreateInstance(t,new object[]{500d});Require(!(bool)t.GetMethod("TryCountSuccess").Invoke(reload,new object[]{"reloadedDaily",false,true,"2026-10-12",saved,(Action<string>)(d=>saved=d)}),"saved daily marker survives session reload");
  var lt=t.Assembly.GetType("Kamilunavo.OneMoreFloor.Monetization.CommercePresentationLock");Require(lt!=null,"shared provider lock exists");object gate=Activator.CreateInstance(lt);object a=new object(),b=new object();
  Require((bool)lt.GetMethod("TryAcquire").Invoke(gate,new[]{a}),"first provider acquires");Require(!(bool)lt.GetMethod("TryAcquire").Invoke(gate,new[]{b}),"second native provider blocked");Require(!(bool)lt.GetMethod("Release").Invoke(gate,new[]{b}),"foreign provider cannot release");Require((bool)lt.GetMethod("Release").Invoke(gate,new[]{a}),"owner releases");Require((bool)lt.GetMethod("TryAcquire").Invoke(gate,new[]{b}),"another provider may enter after close");
 }

  static void ValidateEntitlements()
  {
   var p=FloorSave.Parse("{\"Schema\":1,\"Crystals\":73,\"Commerce\":{\"Entitlements\":3}}");
   Require(!Removed(p)&&p.Commerce.Entitlements==3&&p.Crystals==73,"legacy premium ownership never grants ad removal or changes currency");
   bool[] styles=(bool[])p.Styles.Clone();Require(CommerceRules.FulfillPending(p,RemoveId,"remove1",FloorSave.Save)&&Removed(p),"verified pending RemoveAds grants after persistence");
   Require(p.Crystals==73&&Equal(styles,p.Styles),"ad removal grants neither currency nor premium styles");
   Require(CommerceRules.FulfillPending(p,RemoveId,"remove1",FloorSave.Save)&&p.Commerce.FulfilledTransactions.Count==1,"transaction replay acknowledged idempotently");
   Require(Removed(FloorSave.Load()),"RemoveAds persists and reloads");
   var restored=new FloorProfile();restored.Normalize();Require(CommerceRules.RestoreEntitlement(restored,RemoveId)&&Removed(restored)&&restored.Crystals==0&&!restored.Styles[4],"restore grants only permanent ad removal");
   CommerceRules.ReconcileEntitlements(restored,new[]{CommerceRules.Starter,CommerceRules.Collection});Require(!Removed(restored)&&restored.Styles[4]&&restored.Styles[7],"store revocation removes only missing RemoveAds entitlement");
   var rollback=new FloorProfile();rollback.Normalize();bool threw=false;try{CommerceRules.FulfillPending(rollback,RemoveId,"disk",_=>throw new Exception("disk"));}catch{threw=true;}
   Require(threw&&!Removed(rollback)&&rollback.Commerce.FulfilledTransactions.Count==0,"persistence failure rolls back entitlement and marker");
   string future="{\"Schema\":999,\"Commerce\":{\"Entitlements\":4},\"FuturePayload\":\"preserve\"}";PlayerPrefs.SetString(FloorSave.Key,future);var unknown=FloorSave.Load();threw=false;try{CommerceRules.FulfillPending(unknown,RemoveId,"future",FloorSave.Save);}catch{threw=true;}
   Require(threw&&PlayerPrefs.GetString(FloorSave.Key)==future&&!Removed(unknown),"unknown save remains byte-preserved and purchase rolls back");PlayerPrefs.DeleteKey(FloorSave.Key);FloorSave.Load();
   var config=new MonetizationConfig{InternalTestAds=false};var unit=typeof(MonetizationConfig).GetMethod("InterstitialUnit");Require(unit!=null,"separate configured interstitial units");Require(string.IsNullOrEmpty((string)unit.Invoke(config,new object[]{true}))&&string.IsNullOrEmpty((string)unit.Invoke(config,new object[]{false})),"blank production unit fails closed on both platforms");config.InternalTestAds=true;Require((string)unit.Invoke(config,new object[]{true})=="ca-app-pub-3940256099942544/4411468910"&&(string)unit.Invoke(config,new object[]{false})=="ca-app-pub-3940256099942544/1033173712","official Google interstitial test IDs");
  }
  static bool Equal(bool[] a,bool[] b){if(a.Length!=b.Length)return false;for(int i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
  static void ValidateProductionBoundary()
  {
   var fixture=typeof(FloorValidation).GetMethod("Fixture",BindingFlags.Static|BindingFlags.NonPublic);object[] args={null,null};var hud=(UI.FloorHud)fixture.Invoke(null,args);var root=(GameObject)args[0];var canvas=(Canvas)args[1];bool originalAudio=AudioListener.pause;
   try{
    var course=hud.Course;course.Profile.TutorialCompleted=true;course.Hud=hud;course.Store=root.AddComponent<StorePurchases>();course.Store.Initialize(course);course.Videos=root.AddComponent<RewardedVideos>();course.Videos.Initialize(course);
    var type=typeof(CommerceRules).Assembly.GetType("Kamilunavo.OneMoreFloor.Monetization.RoundInterstitials");Require(type!=null,"scene-owned single-use interstitial provider");var provider=root.AddComponent(type);course.GetType().GetField("Interstitials").SetValue(course,provider);Invoke(provider,"Initialize",course);
    bool sdkRead=false;Set(provider,"consent",(Func<bool>)(()=>{sdkRead=true;throw new Exception("QA must not read UMP");}));Invoke(provider,"Prepare");Invoke(provider,"Update");Require(!sdkRead,"nonmobile/QA provider gates run before consent SDK access");
    Set(provider,"clock",(Func<double>)(()=>180d));Set(provider,"consent",(Func<bool>)(()=>true));var policyType=typeof(CommerceRules).Assembly.GetType("Kamilunavo.OneMoreFloor.Monetization.InterstitialPolicy");var policy=Activator.CreateInstance(policyType,new object[]{0d});Set(provider,"<Policy>k__BackingField",policy);
    var p=course.Profile;p.Step=p.Checkpoint=4;p.PendingCoins=12;p.CheckpointDecisionPending=true;p.RunBanked=false;p.Challenge=false;course.BankRun();Require(Successful(policy)==1&&p.RunBanked,"actual bank transition counts once");Invoke(provider,"Initialize",course);Require(Successful(policy)==1,"same-scene initialization preserves session pacing and unique subscription");int wallet=p.Crystals;DropModal(hud);course.BankRun();DropModal(hud);hud.ShowBanked();Require(Successful(policy)==1&&p.Crystals==wallet,"repeated banking and result reopen never count or pay twice");
    p.RunBanked=false;p.CheckpointDecisionPending=true;DropModal(hud);hud.ShowCheckpoint();Require(Successful(policy)==1,"risk checkpoint alone never counts");DropModal(hud);((Action)typeof(Gameplay.FloorCourse).GetField("CheckpointReached",Hidden).GetValue(course)).Invoke();Require(Successful(policy)==1,"actual checkpoint notification has no ad-count hook");((Action)typeof(Gameplay.FloorCourse).GetField("Fell",Hidden).GetValue(course)).Invoke();Require(Successful(policy)==1,"actual failure notification has no ad-count hook");
    Set(course,"_flying",true);p.PendingCoins=13;course.BankRun();Require(Successful(policy)==1,"mid-transfer banking never counts");Set(course,"_flying",false);
    p.PendingCoins=0;DropModal(hud);course.BankRun();Require(Successful(policy)==1,"empty bank after lost pending coins never counts as success");
    for(int i=0;i<2;i++){Set(course,"_roundToken","fresh"+i);p.RunBanked=false;p.PendingCoins=12;DropModal(hud);course.BankRun();}Require(Successful(policy)==3,"legitimate repeated normal rounds count by fresh token");
    int shown=0;Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>throw new Exception("provider unavailable")),(Action)(()=>shown++)),"provider readiness exception skips without acquiring native lock");Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>false),(Action)(()=>shown++))&&shown==0,"no-fill skips immediately at result");Invoke(provider,"Update");Require(shown==0,"Update cannot defer the skipped result into a surprise show");
    Set(course,"_applicationPaused",true);Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>shown++)),"paused app rejects presentation");Set(course,"_focused",false);Set(course,"_applicationPaused",false);Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>shown++)),"return from pause cannot bypass lost focus");Set(course,"_focused",true);
    Set(provider,"consent",(Func<bool>)(()=>false));Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>shown++)),"no consent skips presentation");Set(provider,"consent",(Func<bool>)(()=>true));
    var gate=course.GetType().GetField("Presentation").GetValue(course);var other=new object();Invoke(gate,"TryAcquire",other);Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>shown++)),"other native provider owns exclusive lock");Invoke(gate,"Release",other);
    CommerceRules.RestoreEntitlement(p,RemoveId);Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>shown++)),"restored RemoveAds wins even over a ready ad");CommerceRules.ReconcileEntitlements(p,Array.Empty<string>());
    Require((bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>shown++))&&shown==1&&course.Paused&&AudioListener.pause,"eligible result acquires shared pause and audio lock");hud.Close();hud.ShowHome();Require(hud.ModalOpen&&(bool)hud.GetType().GetProperty("ResultOpen").GetValue(hud),"native presentation cannot be bypassed by navigation");
    Invoke(provider,"Finish",new object[]{null});Require(!(bool)Invoke(gate,"get_IsLocked")&&AudioListener.pause==originalAudio,"close releases only its own lock and restores prior audio pause");
    Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>throw new Exception("no provider")))&&!(bool)Invoke(gate,"get_IsLocked"),"provider exception skips safely and releases presentation");
    // Actual no-order reconciliation must preserve partial-catalog entitlements.
    CommerceRules.RestoreEntitlement(p,RemoveId);var fetched=(HashSet<string>)Field(course.Store,"_fetched");fetched.Add(CommerceRules.Starter);fetched.Add(CommerceRules.Collection);var empty=new Orders(Array.Empty<ConfirmedOrder>(),Array.Empty<PendingOrder>(),Array.Empty<DeferredOrder>());Invoke(course.Store,"PurchasesFetched",empty);Require(Removed(p),"partial old two-product catalog never revokes RemoveAds");fetched.Add(RemoveId);Invoke(course.Store,"PurchasesFetched",empty);Require(!Removed(p),"complete no-order three-product catalog revokes RemoveAds");
    p.Step=29;p.Completed=false;p.RunBanked=false;Set(course,"_roundToken","complete");DropModal(hud);course.CompletePortal();Require(Successful(policy)==4&&p.Completed,"actual full tower completion counts once");course.CompletePortal();DropModal(hud);hud.ShowCompletion();Require(Successful(policy)==4,"duplicate tower completion and reopen do not count");
    string today=FloorRules.Day(DateTime.UtcNow);p.Challenge=true;p.RunDay=today;p.Step=p.Checkpoint=4;p.Completed=p.RunBanked=false;p.PendingCoins=7;Set(course,"_roundToken","firstDaily");DropModal(hud);course.BankRun();Require(Successful(policy)==5,"first genuinely banked daily result counts");Require((string)p.Commerce.GetType().GetField("InterstitialDailyDay").GetValue(p.Commerce)==today&&(string)FloorSave.Load().Commerce.GetType().GetField("InterstitialDailyDay").GetValue(FloorSave.Load().Commerce)==today,"actual daily exclusion marker persists across save/reload");p.RunBanked=false;p.PendingCoins=7;Set(course,"_roundToken","dailyReplay");DropModal(hud);course.BankRun();Require(Successful(policy)==5,"genuine same-day daily replay pays normal bank but does not count toward ads");p.Challenge=false;
    ValidateShop(hud,canvas);ValidateStoreMetadata(hud,canvas);ValidateConsentBoundary(course,hud);ValidatePrivacyCache(course,provider);ValidateVoluntaryOpened(course,provider);
    // Component disposal must release the same shared pause/audio owner.
    DropModal(hud);hud.ShowBanked();Require((bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>{})),"ready source enters before teardown");UnityEngine.Object.DestroyImmediate(provider);Require(!(bool)Invoke(gate,"get_IsLocked")&&AudioListener.pause==originalAudio,"provider teardown releases its owned native lock");
   }finally{AudioListener.pause=originalAudio;UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}
  }
  static void ValidatePrivacyCache(Gameplay.FloorCourse course,object provider)
  {
   var loads=Field(provider,"loads");long oldGeneration=(long)Invoke(loads,"Begin");Set(provider,"loading",true);
   Invoke(provider,"InvalidateConsent");Require(!(bool)Invoke(loads,"IsCurrent",oldGeneration)&&!(bool)Field(provider,"loading"),"privacy options invalidates both cached forced ad ownership and pending load generation");
   Require(course.AcquireCommerce(course.Videos),"privacy form owns shared presentation gate");
   try{Require(!(bool)Invoke(provider,"TryPresentAtBoundary",(Func<bool>)(()=>true),(Action)(()=>throw new Exception("must never show"))),"no forced ad may open while privacy form owns presentation");}
   finally{course.ReleaseCommerce(course.Videos);}
  }
  static void ValidateVoluntaryOpened(Gameplay.FloorCourse course,object provider)
  {
   object previousPolicy=Field(provider,"<Policy>k__BackingField"),previousClock=Field(provider,"clock");
   var t=previousPolicy.GetType();object policy=Activator.CreateInstance(t,new object[]{0d});
   for(int i=0;i<3;i++)Invoke(policy,"TryCountSuccess","ready"+i,false,false,"","",null);
   var shown=(GoogleMobileAds.Api.RewardedAd)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(GoogleMobileAds.Api.RewardedAd));
   var stale=(GoogleMobileAds.Api.RewardedAd)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(GoogleMobileAds.Api.RewardedAd));
   try{
    Set(provider,"<Policy>k__BackingField",policy);Set(provider,"clock",(Func<double>)(()=>400d));
    Set(course.Videos,"_ad",shown);Set(course.Videos,"_fullscreenRecorded",false);Set(course.Videos,"<IsPresenting>k__BackingField",false);
    Invoke(course.Videos,"RecordFullscreenOpen",shown);Require(double.IsNegativeInfinity((double)t.GetProperty("LastShownAt").GetValue(policy)),"loaded but unpresented rewarded ad cannot delay forced clock");
    Set(course.Videos,"<IsPresenting>k__BackingField",true);Invoke(course.Videos,"RecordFullscreenOpen",stale);Require(double.IsNegativeInfinity((double)t.GetProperty("LastShownAt").GetValue(policy)),"stale rewarded opened callback cannot delay current pacing");
    Invoke(course.Videos,"RecordFullscreenOpen",shown);Require((double)t.GetProperty("LastShownAt").GetValue(policy)==400d,"actual current rewarded opened forwards shared 180s clock");
    Set(provider,"clock",(Func<double>)(()=>401d));Invoke(course.Videos,"RecordFullscreenOpen",shown);Require((double)t.GetProperty("LastShownAt").GetValue(policy)==400d,"duplicate rewarded opened callback counted once");
    Require(Successful(policy)==3&&(int)t.GetProperty("RoundsSinceShown").GetValue(policy)==3,"rewarded fullscreen preserves earned successes");
   }finally{Set(course.Videos,"_ad",null);Set(course.Videos,"<IsPresenting>k__BackingField",false);Set(course.Videos,"_fullscreenRecorded",false);Set(provider,"<Policy>k__BackingField",previousPolicy);Set(provider,"clock",previousClock);}
  }
  static void ValidateStoreMetadata(UI.FloorHud hud,Canvas canvas)
  {
   var store=hud.Course.Store;string priorQa=FloorSave.QaKey;var controller=(StoreController)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(StoreController));
   var products=new MetadataProducts();
   products.Product=(Product)Activator.CreateInstance(typeof(Product),BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{new ProductDefinition(RemoveId,ProductType.NonConsumable),new ProductMetadata("$2.49","Remove Ads","Permanent","USD",2.49m),true},null);
   typeof(StoreController).GetField("m_ProductService",Hidden).SetValue(controller,products);Set(store,"_store",controller);Set(store,"<Ready>k__BackingField",true);
   try{
    Require((bool)store.GetType().GetProperty("QaTrafficBlocked",Hidden).GetValue(store)&&!store.CanRetry&&!store.CanBuy(RemoveId),"QA blocks retry and even ready metadata purchases");
    string status=store.Status;store.Buy(RemoveId);store.Restore();Require(!store.Busy&&!store.IsPresenting&&!hud.Course.Presentation.IsLocked&&store.Status==status,"QA Buy/Restore cannot touch native provider or acquire ownership");
    Set(store,"<Ready>k__BackingField",false);Invoke(store,"Connect");store.RetryConnection();Require(!store.Busy&&store.Status==status,"QA direct Connect and retry stay inert");Set(store,"<Ready>k__BackingField",true);
    FloorSave.QaKey="";
    Require(store.Price(RemoveId)=="$2.49"&&store.CanBuy(RemoveId),"actual store ProductMetadata is the sole localized price and buy availability");
    DropModal(hud);hud.ShowShop();var card=canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Viewport/Content/"+RemoveId);Require(card.Find("ProductPrice").GetComponent<Text>().text=="$2.49"&&card.GetComponent<Button>().interactable,"direct Shop displays real store metadata instead of target euro price");
    var deferred=(HashSet<string>)Field(store,"_deferred");deferred.Add(RemoveId);Require(!store.CanBuy(RemoveId),"Ask-to-Buy deferred purchase cannot be started again");deferred.Clear();
    CommerceRules.RestoreEntitlement(hud.Course.Profile,RemoveId);DropModal(hud);hud.ShowShop();card=canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Viewport/Content/"+RemoveId);Require(card.Find("ProductPrice").GetComponent<Text>().text==hud.T("GEKAUFT","OWNED")&&!card.GetComponent<Button>().interactable,"owned permanent removal cannot be bought twice");Require(RewardRules.CanClaim(hud.Course.Profile,DateTime.UtcNow),"RemoveAds leaves clearly optional rewarded bonus available");
   }finally{FloorSave.QaKey=priorQa;Set(store,"_store",null);Set(store,"<Ready>k__BackingField",false);CommerceRules.ReconcileEntitlements(hud.Course.Profile,Array.Empty<string>());}
  }
  sealed class MetadataProducts:IProductService
  {
   public Product Product;
   public IAppleStoreExtendedProductService Apple=>null;
   public event Action<List<Product>> OnProductsFetched{add{}remove{}}
   public event Action<ProductFetchFailed> OnProductsFetchFailed{add{}remove{}}
   public void FetchProductsWithNoRetries(List<ProductDefinition> products){}
   public void FetchProducts(List<ProductDefinition> products,IRetryPolicy policy=null){}
   public ReadOnlyObservableCollection<Product> GetProducts()=>new(new ObservableCollection<Product>{Product});
   public Product GetProductById(string id)=>id==RemoveId?Product:null;
   public Product GetProductByCatalogListingId(string id)=>GetProductById(id);
  }
  static void ValidateConsentBoundary(Gameplay.FloorCourse course,UI.FloorHud hud)
  {
   course.Profile.TutorialCompleted=false;foreach(Action page in new Action[]{hud.ShowHome,hud.ShowShop,hud.ShowBanked}){DropModal(hud);page();Require(!hud.AdConsentOpen,"unfinished tutorial blocks consent on Home/Shop/Result");}
   course.Profile.TutorialCompleted=true;
   int shown=0;DropModal(hud);hud.Close();Require(!(bool)Invoke(course.Videos,"TryPresentConsent",(Action)(()=>shown++)),"late consent cannot appear during active course");hud.ShowHome();Require((bool)Invoke(course.Videos,"TryPresentConsent",(Action)(()=>shown++))&&shown==1,"first-start consent has a safe home path without opening Shop");Require(course.Paused&&AudioListener.pause,"consent shares native pause/audio ownership");course.GetType().GetMethod("ReleaseCommerce").Invoke(course,new object[]{course.Videos});Set(course.Videos,"<IsPresenting>k__BackingField",false);
   Set(course,"<Tutorial>k__BackingField",new ElevatorTutorial());Require(!(bool)Invoke(course.Videos,"TryPresentConsent",(Action)(()=>shown++)),"tutorial never presents consent");Set(course,"<Tutorial>k__BackingField",null);
  }
  static void ValidateShop(UI.FloorHud hud,Canvas canvas)
  {
   foreach(string language in new[]{"de","en"}){
    hud.Course.Profile.Language=language;DropModal(hud);hud.ShowHome();var safe=canvas.transform.Find("SafeArea");var dock=safe.Find("ModalBackdrop/ModalCard/Viewport/Content/Shop");Require(dock!=null,"Shop is a direct lobby action "+language);var click=dock.GetComponent<Button>().onClick;DropModal(hud);click.Invoke();Canvas.ForceUpdateCanvases();Require(hud.ShopOpen,"one-tap direct Shop opens "+language);var content=safe.Find("ModalBackdrop/ModalCard/Viewport/Content");var remove=content.Find(RemoveId);Require(remove!=null&&remove.GetSiblingIndex()==0,"RemoveAds immediately leads direct Shop "+language);Require(content.Find(CommerceRules.Starter)!=null&&content.Find(CommerceRules.Collection)!=null&&content.Find("Restore")!=null,"premium and Restore share direct Shop "+language);Require(remove.GetSiblingIndex()<content.Find("DesignGrid").GetSiblingIndex(),"ad removal precedes long design list");Require(remove.GetComponent<LayoutElement>().preferredHeight>=48,"compact purchase card preserves touch size");Require(remove.Find("ProductPrice").GetComponent<Text>().text==hud.T("NICHT VERFÜGBAR","UNAVAILABLE")&&!remove.GetComponent<Button>().interactable,"missing store metadata is unavailable without a fabricated €1.99 price");Require(content.Find("RewardVideo").GetComponentInChildren<Text>().text.Contains(hud.T("FREIWILLIG","OPTIONAL")),"rewarded video remains explicitly optional "+language);Require(remove.Find("ProductDetail").GetComponent<Text>().text.Contains(hud.T("für dieses Spiel","this game")),"purchase clearly scoped permanently to this game "+language);
   }
  }

 }
}
#endif
