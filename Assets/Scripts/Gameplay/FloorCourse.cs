using System;
using System.Collections.Generic;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Visuals;
using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Gameplay
{
 [DefaultExecutionOrder(-100)] public sealed class FloorCourse:MonoBehaviour
 {
  public Transform Player;private readonly List<StepMarker> _steps=new();private Vector3 _safe;private bool _paused,_focused=true,_applicationPaused;private float _saveTimer;
  private FloorProfile _tutorialReturn;public ElevatorTutorial Tutorial{get;private set;}public bool TutorialActive=>Tutorial!=null;public FloorProfile PersistentProfile=>_tutorialReturn??Profile;
  public FloorProfile Profile{get;private set;}public int Height=>Profile?.Step??0;public IReadOnlyList<StepMarker> Steps=>_steps;public Vector3 SafePosition=>_safe;
  public event Action Changed,RunStarted,CheckpointReached,Fell,PortalCompleted;public event Action<bool> Landed;
  public UI.FloorHud Hud;public Monetization.StorePurchases Store;public Monetization.RewardedVideos Videos;public Monetization.RoundInterstitials Interstitials;
  public readonly Monetization.CommercePresentationLock Presentation=new();private bool _commerceAudioWasPaused;
  public bool CommerceForeground=>_focused&&!_applicationPaused;
  public bool AcquireCommerce(object owner){if(!CommerceForeground||IsTransferring||!Presentation.TryAcquire(owner))return false;_commerceAudioWasPaused=AudioListener.pause;AudioListener.pause=true;SyncMotor();return true;}
  public void ReleaseCommerce(object owner){if(!Presentation.Release(owner))return;AudioListener.pause=_commerceAudioWasPaused;SyncMotor();}
  private string _roundToken=Guid.NewGuid().ToString("N");
  public event Action<string,bool,string> SuccessfulRound;
  private void NotifySuccessfulRound(){if(!TutorialActive)SuccessfulRound?.Invoke(_roundToken,Profile.Challenge,Profile.RunDay);}
  public bool Paused{get=>_paused||!_focused||_applicationPaused||Presentation.IsLocked;set{_paused=value;SyncMotor();}}
  public void RefreshProfile()=>Refresh();
  public void Build(){Profile=FloorSave.Load();ElevatorRules.Restore(Profile,DateTime.UtcNow);Save();CityArt.Build(transform);BuildRoute();}
  private float _clock,_flightTime;private Vector3 _flightStart,_flightEnd,_velocity,_standingOffset;private bool _flying;
  public bool TimingActive=>Profile!=null&&Profile.TimingVersion==1;
  public bool IsTransferring=>_flying;public float TimingClock=>_clock;public Vector3 FlightVelocity=>_velocity;
  private TowerLiftMotion _lift;private Transform _guides;private bool _onBalcony,_returning;private float _balconyTime,_launchClock,_forecastY;private int _routeSeed;
  public Transform Cabin=>_lift!=null?_lift.transform:null;public bool LiftReady=>_lift!=null&&_lift.Ready(_clock)&&!_onBalcony&&!_returning;public bool OnBalcony=>_onBalcony;public bool Boarding=>_returning;public bool InJumpAir=>_flying&&(_returning?_flightTime/.65f<1-TowerLiftMotion.ExitFraction:_flightTime/ElevatorRules.FlightSeconds>TowerLiftMotion.ExitFraction);
  public Vector3 SourcePosition=>_lift!=null?_lift.transform.position+Vector3.up*.65f:_steps.Count>0?_steps[Height].transform.position+Vector3.up*.65f:Player.position;
  public Vector3 TargetPosition=>_steps.Count==0?SourcePosition:_steps[Mathf.Min(29,_onBalcony||_returning?Height:Height+1)].transform.position+Vector3.up*.65f;
  public Vector3 Destination=>TargetPosition;
  public float AlignmentError=>_lift==null||Height>=29?0:_lift.At(_clock+ElevatorRules.FlightSeconds).y-(Height+1)*TowerLiftMotion.FloorSpacing;
  public float AlignmentWindow=>TowerLiftMotion.LandingWindow(Mathf.Min(29,Height+1));
  public int PredictedHit=>Height>=29||!LiftReady?0:ElevatorRules.Hit(AlignmentError,0,AlignmentWindow);
  public Vector3 ForecastPoint(float t){var end=new Vector3(TowerLiftMotion.TowerX,_lift.At(_clock+ElevatorRules.FlightSeconds).y+.65f,TowerLiftMotion.TowerZ);return JumpPoint(Player.position,end,_clock,t);}
  private Vector3 JumpPoint(Vector3 begin,Vector3 end,float launch,float t){var point=Vector3.Lerp(begin,end,t);float exitY=_lift.At(launch+TowerLiftMotion.ExitFraction*ElevatorRules.FlightSeconds).y+.65f;if(t<=TowerLiftMotion.ExitFraction)point.y=_lift.At(launch+t*ElevatorRules.FlightSeconds).y+.65f;else{float phase=(t-TowerLiftMotion.ExitFraction)/(1-TowerLiftMotion.ExitFraction);point.y=Mathf.Lerp(exitY,end.y,phase)+Mathf.Sin(phase*Mathf.PI)*TowerLiftMotion.JumpArc;}return point;}
  public Rect GameplayViewport=>Hud!=null?Hud.GameplayViewport:new Rect(.08f,.23f,.84f,.54f);
  public bool TargetFramed{get{if(_steps.Count==0)return false;var cam=UnityEngine.Camera.main;if(cam==null)return false;var view=GameplayViewport;bool Visible(Vector3 p){var v=cam.WorldToViewportPoint(p);return v.z>0&&view.Contains(new Vector2(v.x,v.y));}var source=SourcePosition;var target=TargetPosition;for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){if(!Visible(source+new Vector3(x*1.3f,0,z*1.55f))||!Visible(target+new Vector3(x*1.4f,0,z<0?-1.55f:1.2f)))return false;}return Visible(Player.position)&&Visible(Player.position+Vector3.up*2.67f)&&Visible(source+Vector3.up*3.15f)&&Visible(target+Vector3.up*3.6f);}}
  private void BuildRoute(){CityArt.ApplyChapter(Profile.Realm);foreach(var step in _steps)if(step!=null){DisposeRoute(step.transform.parent.gameObject);}_steps.Clear();if(_lift!=null)DisposeRoute(_lift.gameObject);if(_guides!=null)DisposeRoute(_guides.gameObject);_clock=0;_flying=_onBalcony=_returning=false;_velocity=Vector3.zero;_routeSeed=Profile.Challenge?DaySeed(Profile.RunDay):260906+Profile.Realm;
   for(int i=0;i<30;i++){var floor=new GameObject("TowerFloor_"+(i+1).ToString("00"));floor.transform.SetParent(transform,false);floor.transform.position=new Vector3(TowerLiftMotion.TowerX,i*TowerLiftMotion.FloorSpacing,TowerLiftMotion.TowerZ);LiftArchitecture.Bay(floor.transform,i);floor.transform.Find("ArchitecturalFloorBay").localPosition=Vector3.zero;
    LiftArchitecture.LandingDeck(floor.transform,Profile.HighContrast);var landing=new GameObject("TowerLandingSurface",typeof(BoxCollider),typeof(StepMarker));landing.transform.SetParent(floor.transform,false);var collider=landing.GetComponent<BoxCollider>();collider.center=new Vector3(0,.25f,-.20f);collider.size=new Vector3(2.55f,.70f,2.70f);var marker=landing.GetComponent<StepMarker>();marker.Index=i;_steps.Add(marker);MeshArt.Batch(floor);
   }
   var carriage=new GameObject("SingleVerticalLift");carriage.transform.SetParent(transform,false);carriage.transform.localRotation=Quaternion.Euler(0,-90,0);PlatformArt.Build(carriage.transform,0,Profile.Realm);MeshArt.Batch(carriage);_lift=carriage.AddComponent<TowerLiftMotion>();_lift.BeginRide(Height,Mathf.Min(29,Height+1),Profile.Realm,_routeSeed,0);
   var guides=new GameObject("VerticalTowerLiftGuides");guides.transform.SetParent(transform,false);guides.transform.position=Vector3.zero;guides.transform.rotation=carriage.transform.rotation;var brass=FloorArchitecturePalette.Get(transform,FloorArchitectureSurface.Brass);var teal=FloorArchitecturePalette.Get(transform,FloorArchitectureSurface.Teal);for(int side=-1;side<=1;side+=2){MeshArt.Box(guides.transform,"ContinuousVerticalRail",new Vector3(side*1.6f,68f,1.65f),new Vector3(.12f,146f,.18f),teal);MeshArt.Box(guides.transform,"VerticalGuideInlay",new Vector3(side*1.6f,68f,1.545f),new Vector3(.031f,146f,.024f),brass);}_guides=guides.transform;MeshArt.Batch(guides);
   Physics.SyncTransforms();SetSafe();MoveToSafe(true);UpdateVisibility();UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();Refresh();RunStarted?.Invoke();}
  private static void DisposeRoute(GameObject root){root.SetActive(false);if(Application.isPlaying)Destroy(root);else DestroyImmediate(root);}
  private void UpdateVisibility(){for(int i=0;i<_steps.Count;i++)_steps[i].transform.parent.gameObject.SetActive(i>=Mathf.Max(0,Height-1)&&i<=Mathf.Min(29,Height+2));}
  public bool LaunchTransfer(){if(Paused||Profile==null||Profile.Completed||Profile.RunBanked||_flying||Height>=29)return false;if(_onBalcony)return BeginBoarding();if(!LiftReady)return false;_flying=true;_returning=false;_flightTime=0;_launchClock=_clock;_flightStart=Player.position;_forecastY=_lift.At(_clock+ElevatorRules.FlightSeconds).y;_flightEnd=new Vector3(TowerLiftMotion.TowerX,_forecastY+.65f,TowerLiftMotion.TowerZ);Profile.TimingInFlight=true;Save();Player.rotation=Quaternion.identity;Player.GetComponent<PlayerMotor>()?.NotifyTransfer();return true;}
  private bool BeginBoarding(){if(Profile.CheckpointDecisionPending)return false;_flying=_returning=true;_onBalcony=false;_flightTime=0;_flightStart=Player.position;_flightEnd=_lift.transform.position+Vector3.up*.65f;return true;}
  public void BankRun(){if(_flying||Presentation.IsLocked)return;if(TutorialActive&&Tutorial.Stage!=ElevatorTutorialStage.Choice&&Tutorial.Stage!=ElevatorTutorialStage.Bank)return;bool wasBanked=Profile.RunBanked;int banked=ElevatorRules.Bank(Profile);if(!Profile.RunBanked)return;if(TutorialActive){Tutorial.Banked();if(Tutorial.Complete){EndTutorial(true);Hud?.ShowTutorialComplete();return;}}Save();Refresh();Hud?.ShowBanked();if(!wasBanked&&banked>0)NotifySuccessfulRound();}
  private static int DaySeed(string day){unchecked{int hash=17;foreach(char c in day)hash=hash*31+c;return hash;}}
  public bool BeginTutorial(){if(_flying||TutorialActive||Presentation.IsLocked)return false;_tutorialReturn=Profile;Profile=FloorSave.Parse(JsonUtility.ToJson(Profile));Tutorial=new ElevatorTutorial();Profile.Realm=0;Profile.Challenge=false;ElevatorRules.NewRun(Profile);BuildRoute();return true;}
  public void EndTutorial(bool complete){if(!TutorialActive)return;Profile=_tutorialReturn;_tutorialReturn=null;Tutorial=null;if(complete)Profile.TutorialCompleted=true;Save();BuildRoute();}
  public void StartRun(int realm,bool challenge){if(TutorialActive||Presentation.IsLocked)return;if(realm<0||realm>Profile.UnlockedRealm)return;_roundToken=Guid.NewGuid().ToString("N");Profile.Realm=realm;Profile.Step=Profile.Checkpoint=Profile.RewardedFloor=Profile.Score=Profile.Falls=Profile.Perfects=0;ElevatorRules.NewRun(Profile);Profile.Elapsed=0;Profile.Completed=false;Profile.Challenge=challenge;Profile.RunDay=FloorRules.Day(DateTime.UtcNow);Save();BuildRoute();}
  private void SetSafe()=>_safe=new Vector3(0,(Profile.Completed?29:Profile.Checkpoint)*TowerLiftMotion.FloorSpacing+.65f,0);
  private void MoveToSafe(bool current=false){var cc=Player.GetComponent<CharacterController>();if(cc!=null)cc.enabled=false;int floor=current?Height:Profile.Checkpoint;_flying=_returning=false;_onBalcony=Profile.RunBanked||Profile.Completed||Profile.CheckpointDecisionPending;_balconyTime=0;_velocity=Vector3.zero;_standingOffset=new Vector3(0,.65f,0);if(_onBalcony){_lift.Hold(floor);Player.position=_steps[floor].transform.position+_standingOffset;}else{_lift.BeginRide(floor,Mathf.Min(29,floor+1),Profile.Realm,_routeSeed,_clock);Player.position=_lift.transform.position+_standingOffset;}Player.rotation=Quaternion.identity;Physics.SyncTransforms();if(cc!=null)cc.enabled=!TimingActive;Player.GetComponent<PlayerMotor>()?.ResetMotion();}
  private void Update(){if(_steps.Count==0||Player==null||Profile==null||Paused)return;float dt=Time.deltaTime;var prior=Player.position;_clock+=dt;_lift.Tick(_clock);
   if(!Profile.Completed&&!Profile.RunBanked){Profile.Elapsed+=dt;_saveTimer+=dt;if(_saveTimer>=5){_saveTimer=0;Save();}}
   if(_flying){_flightTime+=dt;float duration=_returning?.65f:ElevatorRules.FlightSeconds,t=Mathf.Clamp01(_flightTime/duration);Player.position=_returning?Vector3.Lerp(_flightStart,_flightEnd,t)+Vector3.up*(t<1-TowerLiftMotion.ExitFraction?Mathf.Sin(t/(1-TowerLiftMotion.ExitFraction)*Mathf.PI)*TowerLiftMotion.JumpArc:0):JumpPoint(_flightStart,_flightEnd,_launchClock,t);
    if(t>=1){_flying=false;if(_returning){_returning=false;_lift.BeginRide(Height,Mathf.Min(29,Height+1),Profile.Realm,_routeSeed,_clock);Player.position=SourcePosition;}else{Profile.TimingInFlight=false;int next=Height+1;int hit=ElevatorRules.Hit(_forecastY,next*TowerLiftMotion.FloorSpacing,TowerLiftMotion.LandingWindow(next));if(hit==0)Respawn();else{int frontier=Profile.RewardedFloor;ElevatorRules.Land(Profile,next,hit==2);_onBalcony=true;_balconyTime=0;_lift.Hold(next);Player.position=_steps[next].transform.position+Vector3.up*.65f;SetSafe();UpdateVisibility();
     if(next==29){ElevatorRules.Bank(Profile);FloorRules.Complete(Profile,DateTime.UtcNow);}if(TutorialActive){Tutorial.Landing(hit==2,next);if((next+1)%5==0)Tutorial.AtCheckpoint();}Save();Refresh();Landed?.Invoke(hit==2&&next>frontier);UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();if(next==29){PortalCompleted?.Invoke();NotifySuccessfulRound();}else if((next+1)%5==0)CheckpointReached?.Invoke();}}}
   }else if(_onBalcony){Player.position=_steps[Height].transform.position+Vector3.up*.65f;if(!Profile.Completed&&!Profile.RunBanked&&!Profile.CheckpointDecisionPending){_balconyTime+=dt;if(_balconyTime>=.75f)BeginBoarding();}}
   else Player.position=SourcePosition;
   _velocity=dt>0?(Player.position-prior)/dt:Vector3.zero;
  }
  public void Land(StepMarker step){if(Paused||Profile==null)return;var d=Player.position-step.transform.position;bool perfect=new Vector2(d.x,d.z).magnitude<=.8f;int prior=Profile.RewardedFloor;if(!FloorRules.Land(Profile,step.Index,perfect))return;SetSafe();Save();Refresh();Landed?.Invoke(perfect&&step.Index>prior);if((step.Index+1)%5==0){if(step.Index==29)CompletePortal();else CheckpointReached?.Invoke();}}
  public void Continue(){if(Presentation.IsLocked)return;if(TutorialActive)Tutorial.Risk();if(Profile.RunBanked){Hud?.ShowBanked();return;}if(Profile.CheckpointDecisionPending){Profile.RiskLevel=Mathf.Min(5,(Height+1)/5);Profile.CheckpointDecisionPending=false;}Save();Hud?.Close();}public void RetryCheckpoint(){if(Presentation.IsLocked)return;ElevatorRules.Recover(Profile);SetSafe();MoveToSafe();UpdateVisibility();UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();Save();Refresh();Hud?.Close();}
  public void Respawn(){if(Profile==null)return;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Debug.Log("FLOOR_RECOVERY frame="+Time.frameCount+" player="+Player.position+" safe="+_safe+" fallsBefore="+Profile.Falls);
#endif
   ElevatorRules.Recover(Profile);SetSafe();MoveToSafe();UpdateVisibility();UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();Save();Refresh();Fell?.Invoke();}
  public bool ClaimDaily(){bool claimed=FloorRules.ClaimDaily(Profile,DateTime.UtcNow);if(claimed)Save();Refresh();return claimed;}
  public bool SelectStyle(int style){bool selected=FloorRules.SelectStyle(Profile,style);if(selected)Save();Refresh();return selected;}
  public int CompletePortal(){if(Profile.Completed||Presentation.IsLocked)return 0;int stars=FloorRules.Complete(Profile,DateTime.UtcNow);if(stars>0){Save();Refresh();PortalCompleted?.Invoke();NotifySuccessfulRound();}return stars;}
  public void Save(){try{FloorSave.Save(PersistentProfile);}catch(Exception e){Debug.LogWarning("Profile save unavailable: "+e.Message);}}
  private void OnApplicationPause(bool pause){_applicationPaused=pause;SyncFocus();}private void OnApplicationFocus(bool focus){_focused=focus;SyncFocus();}
  private void SyncMotor(){if(Player!=null){var m=Player.GetComponent<PlayerMotor>();if(m!=null){m.Paused=Paused;m.ResetInput();}}}
  private void SyncFocus(){if(Profile!=null)Save();SyncMotor();}private void OnApplicationQuit(){if(Profile!=null)Save();}private void Refresh()=>Changed?.Invoke();
 }
}
