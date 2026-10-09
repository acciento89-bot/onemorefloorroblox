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
  static bool Enabled=>Array.Exists(Environment.GetCommandLineArgs(),s=>s=="-qaElevator"||s=="-qaElevatorShowcase")||Environment.GetEnvironmentVariable("ELEVATOR_QA")=="1"||Environment.GetEnvironmentVariable("ELEVATOR_QA_SHOWCASE")=="1";
  FloorCourse c;FloorHud h;string output;int checks;
  public static void Configure(){if(Enabled)FloorSave.QaKey="kamilunavo.onemorefloor.elevatorqa."+(Environment.GetEnvironmentVariable("ELEVATOR_QA_ID")??"local");}
  public static void MaybeStart(FloorCourse course,FloorHud hud){if(!Enabled)return;var q=new GameObject("ElevatorRuntimeQa").AddComponent<ElevatorRuntimeQa>();q.c=course;q.h=hud;
#if UNITY_STANDALONE_OSX
 Application.runInBackground=true;course.SendMessage("OnApplicationFocus",true);
#endif
q.StartCoroutine(q.Run());}
  IEnumerator Showcase(){
   c.Profile.UnlockedRealm=2;c.Profile.Crystals=428;c.Profile.Language="de";h.ShowHome();yield return new WaitForEndOfFrame();CheckHome("portrait DE");yield return Capture("lobby-de");c.Profile.Language="en";h.ShowHome();yield return new WaitForEndOfFrame();CheckHome("portrait EN");yield return Capture("lobby-en");
   h.ShowStyle();yield return Capture("styles");h.ShowSettings();yield return Capture("settings");h.ShowShop();yield return Capture("shop");h.ShowDaily();yield return Capture("gift");h.ShowHome();
   var original=c.Profile;int wallet=original.Crystals,step=original.Step,checkpoint=original.Checkpoint,score=original.Score;h.StartTutorial();yield return new WaitForEndOfFrame();Check(c.TutorialActive&&!h.ModalOpen&&!c.Paused,"practice enters actual gameplay");Check(c.PersistentProfile==original&&c.Profile!=original,"practice and live save have separate ownership");Check(c.TargetFramed,"tutorial coach preserves target framing");yield return Capture("tutorial-timing");
   for(int f=1;f<=4;f++){yield return TutorialTransfer(f);if(f==1){Check(c.Tutorial.Stage==ElevatorTutorialStage.Perfect,"first real landing teaches perfect");yield return Capture("tutorial-perfect");}if(f==2)Check(c.Tutorial.Stage==ElevatorTutorialStage.Checkpoint,"second actual perfect advances lesson");}
   Check(c.Tutorial.Stage==ElevatorTutorialStage.Choice&&h.CheckpointDecisionVisible,"actual checkpoint presents banking choice");CheckMenuContrast("practice choice");yield return Capture("tutorial-choice");var decision=GameObject.Find("Continue").GetComponent<UnityEngine.UI.Button>();decision.onClick.Invoke();Check(c.Tutorial.Stage==ElevatorTutorialStage.Risk&&!c.Paused,"real checkpoint continue selects risk");yield return Capture("tutorial-risk");yield return TutorialTransfer(5);Check(c.Tutorial.Stage==ElevatorTutorialStage.Bank,"actual bonus landing teaches bank next");yield return Capture("tutorial-bank-lesson");
   for(int f=6;f<=9;f++)yield return TutorialTransfer(f);GameObject.Find("Bank").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();yield return new WaitForEndOfFrame();Check(!c.TutorialActive&&c.Profile==original&&original.TutorialCompleted,"real bank restores original and completes tutorial");Check(original.Crystals==wallet&&original.Step==step&&original.Checkpoint==checkpoint&&original.Score==score,"practice creates no live coins or progress");Check(FloorSave.Load().TutorialCompleted,"tutorial completion persists");yield return Capture("tutorial-complete");
   h.ShowHome();h.StartTutorial();yield return TutorialTransfer(1);c.Save();Check(FloorSave.Load().Step==step&&FloorSave.Load().Crystals==wallet,"saving interrupted practice preserves live run");h.SkipTutorial();Check(!c.TutorialActive&&original.Step==step&&original.Crystals==wallet&&original.TutorialCompleted,"replay and skip preserve live run and completion");
   Screen.orientation=ScreenOrientation.LandscapeLeft;
#if UNITY_STANDALONE_OSX
   Screen.SetResolution(932,430,false);
#endif
   yield return new WaitForSecondsRealtime(6);h.ShowHome();yield return new WaitForEndOfFrame();
#if (UNITY_IOS && !UNITY_EDITOR) || UNITY_STANDALONE_OSX
   Check(Screen.width>Screen.height,"showcase actual landscape render");
#endif
   CheckHome("landscape EN");yield return Capture("lobby-landscape");yield return BankedPresentation("landscape");h.StartTutorial();yield return new WaitForEndOfFrame();Check(c.TargetFramed,"landscape tutorial excludes coach from next target");yield return Capture("tutorial-landscape");h.SkipTutorial();h.ShowShop();yield return Capture("shop-landscape");Screen.orientation=ScreenOrientation.Portrait;
#if UNITY_STANDALONE_OSX
   Screen.SetResolution(430,932,false);
#endif
   yield return new WaitForSecondsRealtime(6);Check(Screen.width<Screen.height,"showcase actual portrait restored");h.ShowHome();yield return new WaitForEndOfFrame();CheckHome("portrait restored");yield return Capture("lobby-restored");yield return BankedPresentation("portrait");File.WriteAllText(Path.Combine(output,"SHOWCASE-PASS.txt"),"ELEVATOR_SHOWCASE_PASS checks="+checks);
  }
  IEnumerator BankedPresentation(string pose){foreach(var language in new[]{"de","en"}){c.Profile.Language=language;c.StartRun(0,false);h.Close();for(int floor=1;floor<=4;floor++)yield return TutorialTransfer(floor);Check(h.CheckpointDecisionVisible,"regular risk decision visible "+pose+language);CheckMenuContrast(pose+language+" risk");yield return Capture("risk-"+pose+"-"+language);GameObject.Find("Bank").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();yield return new WaitForEndOfFrame();Check(c.Profile.RunBanked,"ordinary bank presents result "+pose+language);CheckMenuContrast(pose+language+" result");yield return Capture("banked-"+pose+"-"+language);h.ShowSettings();CheckMenuContrast(pose+language+" settings");yield return Capture("settings-"+pose+"-"+language);h.ShowHome();}}
  void CheckMenuContrast(string pose){var modal=GameObject.Find("ModalCard");Check(modal!=null,pose+" card exists");foreach(var button in modal.GetComponentsInChildren<UnityEngine.UI.Button>()){var image=button.GetComponent<UnityEngine.UI.Image>();foreach(var state in new[]{button.colors.normalColor,button.colors.highlightedColor,button.colors.pressedColor,button.colors.selectedColor,button.colors.disabledColor})foreach(var text in button.GetComponentsInChildren<UnityEngine.UI.Text>())if(!string.IsNullOrWhiteSpace(text.text))Check(UiFactory.Contrast(text.color,image.color*state*button.colors.colorMultiplier)>=4.5f,pose+" readable "+button.name+"/"+text.name);var viewport=button.transform.parent;while(viewport!=null&&viewport.name!="Viewport")viewport=viewport.parent;if(viewport!=null&&(pose.Contains("risk")||pose.Contains("result")))Check(Contains(UiMetrics.ScreenRect((RectTransform)viewport),UiMetrics.ScreenRect((RectTransform)button.transform)),pose+" choices visible above footer "+button.name);}}
  IEnumerator TutorialTransfer(int floor){float start=Time.realtimeSinceStartup;while(c.PredictedHit!=2&&Time.realtimeSinceStartup-start<15)yield return null;Check(c.PredictedHit==2,"practice perfect window reachable floor"+floor);h.Jump.OnPointerDown(new PointerEventData(EventSystem.current));Check(c.IsTransferring,"practice real touch launches floor"+floor);while(c.IsTransferring){yield return new WaitForEndOfFrame();Check(c.TargetFramed,"practice continuous framing floor"+floor);yield return null;}Check(c.Height==floor,"practice actual landing floor"+floor);yield return new WaitForEndOfFrame();}
  void CheckHome(string pose){var card=(RectTransform)GameObject.Find("Close").transform.parent;var viewport=(RectTransform)card.Find("Viewport");var usable=UiMetrics.ScreenRect((RectTransform)card.parent.parent);var body=UiMetrics.ScreenRect(viewport);Check(!viewport.GetComponent<UnityEngine.UI.ScrollRect>().vertical,pose+" home no hidden scroll controls");var buttons=card.GetComponentsInChildren<UnityEngine.UI.Button>();foreach(var button in buttons){Rect rect=UiMetrics.ScreenRect((RectTransform)button.transform);Check(rect.width/UiMetrics.PointScale>=47.999f&&rect.height/UiMetrics.PointScale>=47.999f,pose+" >=48 points "+button.name);Check(Contains(usable,rect),pose+" inside actual safe pane "+button.name);if(button.transform.IsChildOf(viewport))Check(Contains(body,rect),pose+" controls entirely above fixed footer "+button.name);}for(int a=0;a<buttons.Length;a++)for(int b=a+1;b<buttons.Length;b++)Check(!UiMetrics.ScreenRect((RectTransform)buttons[a].transform).Overlaps(UiMetrics.ScreenRect((RectTransform)buttons[b].transform)),pose+" separate targets "+buttons[a].name+"/"+buttons[b].name);}
  static bool Contains(Rect outer,Rect inner)=>inner.xMin>=outer.xMin-.5f&&inner.yMin>=outer.yMin-.5f&&inner.xMax<=outer.xMax+.5f&&inner.yMax<=outer.yMax+.5f;
  void Check(bool valid,string name){checks++;if(!valid){File.WriteAllText(Path.Combine(output,"FAIL.txt"),name);Debug.LogError("ELEVATOR_QA_FAIL "+name);throw new Exception(name);}Debug.Log("ELEVATOR_QA_PASS "+name);}
  IEnumerator Capture(string name){yield return new WaitForEndOfFrame();var t=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(output,name+".png"),t.EncodeToPNG());Destroy(t);}
  IEnumerator Run(){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,"-qaOutput");output=Environment.GetEnvironmentVariable("ELEVATOR_QA_OUTPUT")??(i>=0?a[i+1]:Path.Combine(Application.persistentDataPath,"ElevatorQA"));Directory.CreateDirectory(output);
#if UNITY_STANDALONE_OSX
 c.SendMessage("OnApplicationFocus",true);
#endif
 yield return new WaitForSecondsRealtime(1);
   if(Environment.GetEnvironmentVariable("ELEVATOR_QA_LANGUAGE")=="en"){c.Profile.Language="en";h.Refresh();if(c.Profile.CheckpointDecisionPending)h.ShowCheckpoint();else if(c.Profile.RunBanked)h.ShowBanked();else h.ShowHome();}
   if(Environment.GetEnvironmentVariable("ELEVATOR_QA_LANDSCAPE")=="1"){Screen.orientation=ScreenOrientation.LandscapeLeft;yield return new WaitForSecondsRealtime(6);}
   if(Array.Exists(a,s=>s=="-qaElevatorShowcase")||Environment.GetEnvironmentVariable("ELEVATOR_QA_SHOWCASE")=="1"){yield return Showcase();if(Array.Exists(a,s=>s=="-qaExit"))Application.Quit();yield break;}
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
