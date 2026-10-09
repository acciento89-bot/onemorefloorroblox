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
  public UI.FloorHud Hud;public Monetization.StorePurchases Store;public Monetization.RewardedVideos Videos;
  public bool Paused{get=>_paused||!_focused||_applicationPaused;set{_paused=value;SyncMotor();}}
  public void RefreshProfile()=>Refresh();
  public void Build(){Profile=FloorSave.Load();ElevatorRules.Restore(Profile,DateTime.UtcNow);Save();CityArt.Build(transform);BuildRoute();}
  private float _clock,_flightTime;private Vector3 _flightStart,_flightEnd,_velocity,_standingOffset;private bool _flying;
  public bool TimingActive=>Profile!=null&&Profile.TimingVersion==1;
  public bool IsTransferring=>_flying;public float TimingClock=>_clock;public Vector3 FlightVelocity=>_velocity;
  public Vector3 Destination=>_steps.Count==0?Vector3.zero:new Vector3(0,(Mathf.Min(29,Height+1))*1.7f+.65f,(Mathf.Min(29,Height+1))*3.7f);
  public int PredictedHit=>Height>=29?0:ElevatorRules.Hit(0,_steps[Height+1].transform.parent.GetComponent<ElevatorMotion>().At(_clock+ElevatorRules.FlightSeconds).x,ElevatorRules.HalfBay(Height+1));
  public Rect GameplayViewport=>Hud!=null?Hud.GameplayViewport:new Rect(.08f,.23f,.84f,.54f);
  public bool TargetFramed{get{if(_steps.Count==0)return false;var cam=UnityEngine.Camera.main;if(cam==null)return false;var view=GameplayViewport;for(int deck=0;deck<2;deck++){var center=_steps[Mathf.Min(29,Height+deck)].transform.position;for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){var v=cam.WorldToViewportPoint(center+new Vector3(x*2.4f,.65f,z*1.9f));if(v.z<=0||!view.Contains(new Vector2(v.x,v.y)))return false;}}var arc=cam.WorldToViewportPoint((_steps[Height].transform.position+Destination)*.5f+Vector3.up*2.2f);return arc.z>0&&view.Contains(new Vector2(arc.x,arc.y));}}
  private void BuildRoute(){foreach(var s in _steps)if(s!=null){s.transform.parent.gameObject.SetActive(false);Destroy(s.transform.parent.gameObject);}_steps.Clear();_clock=0;_flying=false;_velocity=Vector3.zero;
   int seed=Profile.Challenge?DaySeed(Profile.RunDay):260906+Profile.Realm;
   for(int i=0;i<30;i++){var root=new GameObject("Elevator_"+(i+1).ToString("00"));root.transform.SetParent(transform,false);root.transform.position=new Vector3(0,i*1.7f,i*3.7f);var marker=PlatformArt.Build(root.transform,i,Profile.Realm);_steps.Add(marker);
    var motion=root.AddComponent<ElevatorMotion>();motion.Floor=i;motion.Realm=Profile.Realm;motion.Seed=seed;motion.Origin=root.transform.position;motion.Tick(0);
    foreach(var rim in root.GetComponentsInChildren<Transform>()){if(rim.name=="PrecisionBaySide"){var pos=rim.localPosition;pos.x=Mathf.Sign(pos.x)*ElevatorRules.HalfBay(i);rim.localPosition=pos;}if(rim.name=="PrecisionBayEnd"){var scale=rim.localScale;scale.x=ElevatorRules.HalfBay(i)*2;rim.localScale=scale;}}
    MeshArt.Batch(root);
    var rail=new GameObject("StationaryLiftRail");rail.transform.SetParent(root.transform,false);motion.Rail=rail.transform;
    var cyan=MeshArt.Mat("LiftRailCyan",new Color(.02f,.55f,.85f),-1,true);var steel=MeshArt.Mat("LiftTrackSteel",new Color(.12f,.17f,.23f),1);
    MeshArt.Box(rail.transform,"Track",new Vector3(0,-.45f,0),new Vector3(7.2f,.17f,.35f),steel);
    MeshArt.Box(rail.transform,"TrackSignal",new Vector3(0,-.32f,-.22f),new Vector3(7.2f,.035f,.06f),cyan);
    for(int side=-1;side<=1;side+=2){MeshArt.Box(rail.transform,"LiftPylon",new Vector3(side*3.5f,-1.1f,0),new Vector3(.22f,1.7f,.3f),steel);MeshArt.Box(rail.transform,"LiftEndSignal",new Vector3(side*3.5f,-.12f,0),new Vector3(.12f,.25f,.18f),cyan);}
    var alloy=MeshArt.Mat("LiftAlloy",new Color(.46f,.59f,.65f),0);
    for(int side=-1;side<=1;side+=2){MeshArt.Box(rail.transform,"TrackGuide",new Vector3(0,-.42f,side*.23f),new Vector3(6.9f,.045f,.045f),alloy);MeshArt.Box(rail.transform,"DriveEndHousing",new Vector3(side*3.35f,-.44f,0),new Vector3(.52f,.40f,.64f),steel);for(int slot=0;slot<4;slot++)MeshArt.Box(rail.transform,"TrackEndVent",new Vector3(side*3.35f,-.34f,-.325f+slot*.10f),new Vector3(.35f,.035f,.018f),alloy);}
    for(int joint=0;joint<7;joint++)MeshArt.Box(rail.transform,"RailJoint",new Vector3(-3+joint,-.53f,0),new Vector3(.06f,.09f,.52f),steel);
    MeshArt.Batch(rail);
   }Physics.SyncTransforms();SetSafe();MoveToSafe(true);UpdateVisibility();UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();Refresh();RunStarted?.Invoke();}
  private void UpdateVisibility(){for(int i=0;i<_steps.Count;i++)_steps[i].transform.parent.gameObject.SetActive(i>=Height&&i<=Height+1);}
  public bool LaunchTransfer(){if(Paused||Profile==null||Profile.Completed||Profile.RunBanked||_flying||Height>=29)return false;_flying=true;_flightTime=0;_flightStart=Player.position;_flightEnd=Destination;Profile.TimingInFlight=true;Save();Player.rotation=Quaternion.LookRotation(new Vector3(0,0,1));Player.GetComponent<PlayerMotor>()?.NotifyTransfer();return true;}
  public void BankRun(){if(_flying)return;if(TutorialActive&&Tutorial.Stage!=ElevatorTutorialStage.Choice&&Tutorial.Stage!=ElevatorTutorialStage.Bank)return;ElevatorRules.Bank(Profile);if(!Profile.RunBanked)return;if(TutorialActive){Tutorial.Banked();if(Tutorial.Complete){EndTutorial(true);Hud?.ShowTutorialComplete();return;}}Save();Refresh();Hud?.ShowBanked();}
  private static int DaySeed(string day){unchecked{int hash=17;foreach(char c in day)hash=hash*31+c;return hash;}}
  public bool BeginTutorial(){if(_flying||TutorialActive)return false;_tutorialReturn=Profile;Profile=FloorSave.Parse(JsonUtility.ToJson(Profile));Tutorial=new ElevatorTutorial();Profile.Realm=0;Profile.Challenge=false;ElevatorRules.NewRun(Profile);BuildRoute();return true;}
  public void EndTutorial(bool complete){if(!TutorialActive)return;Profile=_tutorialReturn;_tutorialReturn=null;Tutorial=null;if(complete)Profile.TutorialCompleted=true;Save();BuildRoute();}
  public void StartRun(int realm,bool challenge){if(TutorialActive)return;if(realm<0||realm>Profile.UnlockedRealm)return;Profile.Realm=realm;Profile.Step=Profile.Checkpoint=Profile.RewardedFloor=Profile.Score=Profile.Falls=Profile.Perfects=0;ElevatorRules.NewRun(Profile);Profile.Elapsed=0;Profile.Completed=false;Profile.Challenge=challenge;Profile.RunDay=FloorRules.Day(DateTime.UtcNow);Save();BuildRoute();}
  private void SetSafe()=>_safe=_steps[Profile.Completed?29:Profile.Checkpoint].transform.position+Vector3.up*.65f;
  private void MoveToSafe(bool current=false){var cc=Player.GetComponent<CharacterController>();if(cc!=null)cc.enabled=false;Player.position=current?_steps[Height].transform.position+Vector3.up*.65f:_safe;_flying=false;_velocity=Vector3.zero;_standingOffset=new Vector3(0,.65f,0);Player.rotation=Quaternion.identity;Physics.SyncTransforms();if(cc!=null)cc.enabled=!TimingActive;Player.GetComponent<PlayerMotor>()?.ResetMotion();}
  private void Update(){if(_steps.Count==0||Player==null||Profile==null||Paused)return;float dt=Time.deltaTime;var prior=Player.position;_clock+=dt;foreach(var step in _steps)step.transform.parent.GetComponent<ElevatorMotion>().Tick(_clock);
   if(!Profile.Completed&&!Profile.RunBanked){Profile.Elapsed+=dt;_saveTimer+=dt;if(_saveTimer>=5){_saveTimer=0;Save();}}
   if(_flying){_flightTime+=dt;float t=Mathf.Clamp01(_flightTime/ElevatorRules.FlightSeconds);Player.position=Vector3.Lerp(_flightStart,_flightEnd,t)+Vector3.up*Mathf.Sin(t*Mathf.PI)*1.8f;
    if(t>=1){_flying=false;Profile.TimingInFlight=false;int next=Height+1;float arrivalX=_steps[next].transform.parent.GetComponent<ElevatorMotion>().At(ElevatorRules.ArrivalClock(_clock,_flightTime)).x;int hit=ElevatorRules.Hit(0,arrivalX,ElevatorRules.HalfBay(next));if(hit==0)Respawn();else{int frontier=Profile.RewardedFloor;ElevatorRules.Land(Profile,next,hit==2);_standingOffset=new Vector3(-arrivalX,.65f,0);Player.position=_steps[next].transform.position+_standingOffset;SetSafe();UpdateVisibility();
     if(next==29){ElevatorRules.Bank(Profile);FloorRules.Complete(Profile,DateTime.UtcNow);}if(TutorialActive){Tutorial.Landing(hit==2,next);if((next+1)%5==0)Tutorial.AtCheckpoint();}Save();Refresh();Landed?.Invoke(hit==2&&next>frontier);UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();if(next==29)PortalCompleted?.Invoke();else if((next+1)%5==0)CheckpointReached?.Invoke();}}

   }else Player.position=_steps[Height].transform.position+_standingOffset;
   _velocity=dt>0?(Player.position-prior)/dt:Vector3.zero;
  }
  public void Land(StepMarker step){if(Paused||Profile==null)return;var d=Player.position-step.transform.position;bool perfect=new Vector2(d.x,d.z).magnitude<=.8f;int prior=Profile.RewardedFloor;if(!FloorRules.Land(Profile,step.Index,perfect))return;SetSafe();Save();Refresh();Landed?.Invoke(perfect&&step.Index>prior);if((step.Index+1)%5==0){if(step.Index==29)CompletePortal();else CheckpointReached?.Invoke();}}
  public void Continue(){if(TutorialActive)Tutorial.Risk();if(Profile.RunBanked){Hud?.ShowBanked();return;}if(Profile.CheckpointDecisionPending){Profile.RiskLevel=Mathf.Min(5,(Height+1)/5);Profile.CheckpointDecisionPending=false;}Save();Hud?.Close();}public void RetryCheckpoint(){ElevatorRules.Recover(Profile);SetSafe();MoveToSafe();UpdateVisibility();UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();Save();Refresh();Hud?.Close();}
  public void Respawn(){if(Profile==null)return;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Debug.Log("FLOOR_RECOVERY frame="+Time.frameCount+" player="+Player.position+" safe="+_safe+" fallsBefore="+Profile.Falls);
#endif
   ElevatorRules.Recover(Profile);SetSafe();MoveToSafe();UpdateVisibility();UnityEngine.Camera.main?.GetComponent<CameraSystem.OrbitCamera>()?.FrameNow();Save();Refresh();Fell?.Invoke();}
  public bool ClaimDaily(){bool claimed=FloorRules.ClaimDaily(Profile,DateTime.UtcNow);if(claimed)Save();Refresh();return claimed;}
  public bool SelectStyle(int style){bool selected=FloorRules.SelectStyle(Profile,style);if(selected)Save();Refresh();return selected;}
  public int CompletePortal(){if(Profile.Completed)return 0;int stars=FloorRules.Complete(Profile,DateTime.UtcNow);if(stars>0){Save();Refresh();PortalCompleted?.Invoke();}return stars;}
  public void Save(){try{FloorSave.Save(PersistentProfile);}catch(Exception e){Debug.LogWarning("Profile save unavailable: "+e.Message);}}
  private void OnApplicationPause(bool pause){_applicationPaused=pause;SyncFocus();}private void OnApplicationFocus(bool focus){_focused=focus;SyncFocus();}
  private void SyncMotor(){if(Player!=null){var m=Player.GetComponent<PlayerMotor>();if(m!=null){m.Paused=Paused;m.ResetInput();}}}
  private void SyncFocus(){if(Profile!=null)Save();SyncMotor();}private void OnApplicationQuit(){if(Profile!=null)Save();}private void Refresh()=>Changed?.Invoke();
 }
}
