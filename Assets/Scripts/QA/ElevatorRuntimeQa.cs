#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.UI;
namespace Kamilunavo.OneMoreFloor.QA
{
 public sealed class ElevatorRuntimeQa:MonoBehaviour
 {
  static bool Enabled=>Array.Exists(Environment.GetCommandLineArgs(),s=>s=="-qaElevator")||Environment.GetEnvironmentVariable("ELEVATOR_QA")=="1";
  FloorCourse c;FloorHud h;string output;int checks;
  public static void Configure(){if(Enabled)FloorSave.QaKey="kamilunavo.onemorefloor.elevatorqa."+(Environment.GetEnvironmentVariable("ELEVATOR_QA_ID")??"local");}
  public static void MaybeStart(FloorCourse course,FloorHud hud){if(!Enabled)return;var q=new GameObject("ElevatorRuntimeQa").AddComponent<ElevatorRuntimeQa>();q.c=course;q.h=hud;
#if UNITY_STANDALONE_OSX
 Application.runInBackground=true;course.SendMessage("OnApplicationFocus",true);
#endif
q.StartCoroutine(q.Run());}
  void Check(bool valid,string name){checks++;if(!valid){File.WriteAllText(Path.Combine(output,"FAIL.txt"),name);Debug.LogError("ELEVATOR_QA_FAIL "+name);throw new Exception(name);}Debug.Log("ELEVATOR_QA_PASS "+name);}
  IEnumerator Capture(string name){yield return new WaitForEndOfFrame();var t=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(output,name+".png"),t.EncodeToPNG());Destroy(t);}
  IEnumerator Run(){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,"-qaOutput");output=Environment.GetEnvironmentVariable("ELEVATOR_QA_OUTPUT")??(i>=0?a[i+1]:Path.Combine(Application.persistentDataPath,"ElevatorQA"));Directory.CreateDirectory(output);
#if UNITY_STANDALONE_OSX
 c.SendMessage("OnApplicationFocus",true);
#endif
 yield return new WaitForSecondsRealtime(1);
   if(Environment.GetEnvironmentVariable("ELEVATOR_QA_LANGUAGE")=="en"){c.Profile.Language="en";h.Refresh();if(c.Profile.CheckpointDecisionPending)h.ShowCheckpoint();else if(c.Profile.RunBanked)h.ShowBanked();else h.ShowHome();}
   if(Environment.GetEnvironmentVariable("ELEVATOR_QA_LANDSCAPE")=="1"){Screen.orientation=ScreenOrientation.LandscapeLeft;yield return new WaitForSecondsRealtime(6);}
   Check(h.ModalOpen&&c.Paused,"home freezes clock");
   if(Environment.GetEnvironmentVariable("ELEVATOR_QA_RELOAD")=="1"){Check(c.Height==4&&c.Profile.CheckpointDecisionPending&&c.Profile.PendingCoins>0,"fresh process restores unresolved checkpoint and pending coins");Check(h.CheckpointDecisionVisible,"checkpoint banking menu restored");int amount=c.Profile.PendingCoins,reloadWallet=c.Profile.Crystals;c.BankRun();Check(c.Profile.RunBanked&&c.Profile.Crystals==reloadWallet+amount,"restored checkpoint banks exactly once");c.BankRun();Check(c.Profile.Crystals==reloadWallet+amount,"bank repeat after reload no mint");yield return Capture("restored-checkpoint-banked");File.WriteAllText(Path.Combine(output,"RELOAD-PASS.txt"),"ELEVATOR_RELOAD_PASS checks="+checks);yield break;}
Check(!h.Joystick.gameObject.activeSelf,"no parkour joystick");yield return Capture("home");h.Close();
#if UNITY_STANDALONE_OSX
 c.SendMessage("OnApplicationFocus",true);
#endif
 c.StartRun(0,false);yield return null;
   var rotation=Camera.main.transform.rotation;
   for(int realm=0;realm<3;realm++){c.Profile.UnlockedRealm=2;c.StartRun(realm,false);for(int f=1;f<30;f++){
    float start=Time.realtimeSinceStartup;while(c.PredictedHit!=2&&Time.realtimeSinceStartup-start<15)yield return null;Check(c.PredictedHit==2,"perfect window reachable floor"+f);int falls=c.Profile.Falls;
    h.Jump.OnPointerDown(new PointerEventData(EventSystem.current));Check(c.IsTransferring,"real timing touch immediately starts transfer");yield return null;Check(!c.LaunchTransfer(),"duplicate transfer blocked");
    if(realm==0&&f==2){c.SendMessage("OnApplicationPause",true);float time=c.TimingClock;var position=c.Player.position;h.Jump.OnPointerDown(new PointerEventData(EventSystem.current));yield return new WaitForSecondsRealtime(.2f);Check(c.TimingClock==time&&c.Player.position==position,"background freezes in-flight transfer");c.SendMessage("OnApplicationPause",false);Check(!h.Jump.Consume(),"resume discards stale touch");}
    while(c.IsTransferring){yield return new WaitForEndOfFrame();Check(c.TargetFramed,"continuous source target arc framing");Check(Quaternion.Angle(rotation,Camera.main.transform.rotation)<.01f,"actual rendered camera fixed");yield return null;}
    Check(c.Height==f&&c.Profile.Falls==falls,"actual arrival floor"+f);yield return new WaitForEndOfFrame();Check(c.TargetFramed,"next deck within unobstructed gameplay pane");
    if(realm==0&&(f==1||f==6||f==15))yield return Capture("floor"+(f+1));if(h.ModalOpen&&f<29)c.Continue();
   }Check(c.Profile.Completed&&c.Profile.RunBanked&&c.Profile.PendingCoins==0,"tower30 payout complete"+realm);yield return Capture("tower"+realm+"-complete");h.Close();}
   c.StartRun(0,false);for(int f=1;f<=7;f++){while(c.PredictedHit!=2)yield return null;c.LaunchTransfer();while(c.IsTransferring)yield return null;if(h.ModalOpen)c.Continue();}
   int wallet=c.Profile.Crystals;while(c.PredictedHit!=0)yield return null;int before=c.Profile.Falls;c.LaunchTransfer();while(c.IsTransferring)yield return null;Check(c.Height==4&&c.Profile.Falls==before+1&&c.Profile.PendingCoins==0&&c.Profile.Crystals==wallet,"real mistimed launch checkpoint and pending loss");
   float clock=c.TimingClock;h.ShowSettings();yield return new WaitForSecondsRealtime(.2f);Check(c.TimingClock==clock&&!c.LaunchTransfer(),"modal freeze and input rejected");h.Close();c.SendMessage("OnApplicationFocus",false);yield return null;Check(!c.LaunchTransfer(),"background input rejected");c.SendMessage("OnApplicationFocus",true);
   yield return Capture("portrait");Screen.orientation=ScreenOrientation.LandscapeLeft;yield return new WaitForSecondsRealtime(6);
#if UNITY_IOS && !UNITY_EDITOR
 Check(Screen.width>Screen.height,"true native landscape dimensions");
#endif
 Check(c.TargetFramed,"landscape next target unobstructed");for(int floor=5;floor<=9;floor++){while(c.PredictedHit!=2)yield return null;c.LaunchTransfer();while(c.IsTransferring){yield return new WaitForEndOfFrame();Check(c.TargetFramed,"landscape continuous framing");yield return null;}Check(c.Height==floor,"actual landscape arrival floor"+floor);if(h.ModalOpen)c.Continue();}yield return Capture("landscape");h.ShowShop();yield return Capture("landscape-shop");h.Close();Screen.orientation=ScreenOrientation.Portrait;yield return new WaitForSecondsRealtime(6);
   #if UNITY_IOS && !UNITY_EDITOR
 Check(Screen.height>Screen.width,"true native portrait dimensions");
#endif
   Check(c.TargetFramed,"portrait target unobstructed");var r=UiMetrics.ScreenRect((RectTransform)h.Jump.transform);Check(r.height/UiMetrics.PointScale>=48,"timing control >=48points");Check(!h.Joystick.gameObject.activeSelf,"joystick remains hidden after rotations");yield return Capture("portrait-restored");
   c.StartRun(0,false);yield return new WaitForEndOfFrame();Check(c.TargetFramed,"restart camera frames origin immediately");for(int floor=1;floor<=4;floor++){while(c.PredictedHit!=2)yield return null;c.LaunchTransfer();while(c.IsTransferring)yield return null;}Check(c.Profile.CheckpointDecisionPending&&h.CheckpointDecisionVisible,"leave unresolved checkpoint for fresh-process reload");c.Save();
   File.WriteAllText(Path.Combine(output,"PASS.txt"),"ELEVATOR_QA_PASS checks="+checks+" real87 guided transfers,3towers,miss,bank,pause,renderedcamera,framing");if(Array.Exists(a,s=>s=="-qaExit"))Application.Quit();
  }
 }
}
#endif
