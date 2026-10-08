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
  public FloorProfile Profile{get;private set;}public int Height=>Profile?.Step??0;public IReadOnlyList<StepMarker> Steps=>_steps;public Vector3 SafePosition=>_safe;
  public event Action Changed,RunStarted,CheckpointReached,Fell,PortalCompleted;public event Action<bool> Landed;
  public UI.FloorHud Hud;public Monetization.StorePurchases Store;public Monetization.RewardedVideos Videos;
  public bool Paused{get=>_paused||!_focused||_applicationPaused;set{_paused=value;SyncMotor();}}
  public void RefreshProfile()=>Refresh();
  public void Build(){Profile=FloorSave.Load();if(!Profile.Completed)Profile.Step=Profile.Checkpoint;CityArt.Build(transform);BuildRoute();}
  private void BuildRoute(){foreach(var s in _steps)if(s!=null){s.transform.parent.gameObject.SetActive(false);Destroy(s.transform.parent.gameObject);}_steps.Clear();var points=CoursePatterns.Points(Profile.Realm,Profile.Challenge?DaySeed(Profile.RunDay):260906+Profile.Realm);
   for(int i=0;i<points.Length;i++){var root=new GameObject("Floor_"+(i+1).ToString("00"));root.transform.SetParent(transform,false);root.transform.position=points[i];var marker=PlatformArt.Build(root.transform,i,Profile.Realm);_steps.Add(marker);if(CoursePatterns.Moves(i)){var moving=root.AddComponent<MovingPlatform>();moving.Speed=.7f+Profile.Realm*.15f;}else MeshArt.Batch(root);}Physics.SyncTransforms();SetSafe();MoveToSafe();Refresh();RunStarted?.Invoke();}
  private static int DaySeed(string day){unchecked{int hash=17;foreach(char c in day)hash=hash*31+c;return hash;}}
  public void StartRun(int realm,bool challenge){if(realm<0||realm>Profile.UnlockedRealm)return;Profile.Realm=realm;Profile.Step=Profile.Checkpoint=Profile.RewardedFloor=Profile.Score=Profile.Falls=Profile.Perfects=0;Profile.Elapsed=0;Profile.Completed=false;Profile.Challenge=challenge;Profile.RunDay=FloorRules.Day(DateTime.UtcNow);Save();BuildRoute();}
  private void SetSafe()=>_safe=_steps[Profile.Completed?29:Profile.Checkpoint].transform.position+Vector3.up*.65f;
  private void MoveToSafe(){var cc=Player.GetComponent<CharacterController>();if(cc!=null)cc.enabled=false;Player.position=_safe;Physics.SyncTransforms();if(cc!=null)cc.enabled=true;Player.GetComponent<PlayerMotor>()?.ResetMotion();}
  private void Update(){if(_steps.Count==0||Player==null||Profile==null)return;foreach(var step in _steps){var move=step.transform.parent.GetComponent<MovingPlatform>();if(move!=null)move.Tick(Paused?0:Time.deltaTime);}if(Paused)return;Physics.SyncTransforms();
   if(!Profile.Completed){Profile.Elapsed+=Time.deltaTime;_saveTimer+=Time.deltaTime;if(_saveTimer>=5){_saveTimer=0;Save();}}
   if(Player.position.y<_steps[Height].transform.position.y-5)Respawn();}
  public void Land(StepMarker step){if(Paused||Profile==null)return;var d=Player.position-step.transform.position;bool perfect=new Vector2(d.x,d.z).magnitude<=.8f;int prior=Profile.RewardedFloor;if(!FloorRules.Land(Profile,step.Index,perfect))return;SetSafe();Save();Refresh();Landed?.Invoke(perfect&&step.Index>prior);if((step.Index+1)%5==0){if(step.Index==29)CompletePortal();else CheckpointReached?.Invoke();}}
  public void Continue()=>Hud?.Close();public void RetryCheckpoint(){FloorRules.Recover(Profile);SetSafe();MoveToSafe();Save();Refresh();Hud?.Close();}
  public void Respawn(){if(Profile==null)return;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Debug.Log("FLOOR_RECOVERY frame="+Time.frameCount+" player="+Player.position+" safe="+_safe+" fallsBefore="+Profile.Falls);
#endif
   FloorRules.Recover(Profile);SetSafe();MoveToSafe();Save();Refresh();Fell?.Invoke();}
  public bool ClaimDaily(){bool claimed=FloorRules.ClaimDaily(Profile,DateTime.UtcNow);if(claimed)Save();Refresh();return claimed;}
  public bool SelectStyle(int style){bool selected=FloorRules.SelectStyle(Profile,style);if(selected)Save();Refresh();return selected;}
  public int CompletePortal(){if(Profile.Completed)return 0;int stars=FloorRules.Complete(Profile,DateTime.UtcNow);if(stars>0){Save();Refresh();PortalCompleted?.Invoke();}return stars;}
  public void Save(){try{FloorSave.Save(Profile);}catch(Exception e){Debug.LogWarning("Profile save unavailable: "+e.Message);}}
  private void OnApplicationPause(bool pause){_applicationPaused=pause;SyncFocus();}private void OnApplicationFocus(bool focus){_focused=focus;SyncFocus();}
  private void SyncMotor(){if(Player!=null){var m=Player.GetComponent<PlayerMotor>();if(m!=null){m.Paused=Paused;m.ResetInput();}}}
  private void SyncFocus(){if(Profile!=null)Save();SyncMotor();}private void OnApplicationQuit(){if(Profile!=null)Save();}private void Refresh()=>Changed?.Invoke();
 }
}
