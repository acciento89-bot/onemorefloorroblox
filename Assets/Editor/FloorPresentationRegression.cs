#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.UI;
using Kamilunavo.OneMoreFloor.Monetization;

// Reproduces native first-frame density and retained-menu navigation, without SDK calls.
public static class FloorPresentationRegression
{
 static int checks;
 static readonly BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool valid,string label){checks++;if(!valid)throw new Exception(label);}
 static void Set(object owner,string field,object value)=>owner.GetType().GetField(field,Private).SetValue(owner,value);
 static void RemoveModal(FloorHud hud){var modal=(RectTransform)typeof(FloorHud).GetField("_modal",Private).GetValue(hud);if(modal!=null)UnityEngine.Object.DestroyImmediate(modal.gameObject);Set(hud,"_modal",null);}
 static void Call(FloorHud hud,string method)=>typeof(FloorHud).GetMethod(method,Private).Invoke(hud,null);
 static bool Inside(Rect outer,Rect inner)=>inner.xMin>=outer.xMin-.5f&&inner.yMin>=outer.yMin-.5f&&inner.xMax<=outer.xMax+.5f&&inner.yMax<=outer.yMax+.5f;
 public static void Validate(){checks=0;var failures=new List<string>();foreach(var size in new[]{new Vector2(375,627),new Vector2(734,343),new Vector2(280,580)})foreach(var language in new[]{"de","en"}){
  try{Fixture(size,language,true);}catch(Exception e){failures.Add("tutorial "+size+" "+language+": "+e.GetBaseException().Message);}
  try{Fixture(size,language,false);}catch(Exception e){failures.Add("navigation "+size+" "+language+": "+e.GetBaseException().Message);}
 }if(failures.Count>0)throw new Exception("FLOOR_PRESENTATION_REGRESSION: "+string.Join("; ",failures));Debug.Log("FLOOR_PRESENTATION_REGRESSION_PASS checks="+checks);}
 static void Fixture(Vector2 size,string language,bool tutorial){var root=new GameObject("FloorPresentationFixture");Canvas canvas=null;string previousKey=FloorSave.QaKey;try{
  FloorSave.QaKey="floor.presentation.editor.fixture";var course=root.AddComponent<FloorCourse>();var profile=new FloorProfile{Language=language,UnlockedRealm=2,TimingVersion=1,Crystals=428};profile.Normalize();Set(course,"<Profile>k__BackingField",profile);course.Player=new GameObject("Player").transform;course.Player.SetParent(root.transform);var hud=root.AddComponent<FloorHud>();course.Hud=hud;hud.Initialize(course);
  var safe=(RectTransform)typeof(FloorHud).GetField("_safe",Private).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();canvas.GetComponent<CanvasScaler>().enabled=false;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.sizeDelta=size;UiMetrics.QaPointScale=3;canvas.scaleFactor=3;Call(hud,"Layout");
  if(tutorial){RemoveModal(hud);hud.Close();Set(course,"<Tutorial>k__BackingField",new ElevatorTutorial());
   // Native first frame can report point scale 3 while CanvasScaler still reports 1.
   UnityEngine.Object.DestroyImmediate(safe.Find("TutorialCoach").gameObject);canvas.scaleFactor=1;Call(hud,"CreateCoach");canvas.scaleFactor=3;Call(hud,"Layout");Call(hud,"UpdateCoach");Canvas.ForceUpdateCanvases();
   foreach(var density in new[]{3f,2f,3f}){UiMetrics.QaPointScale=density;canvas.scaleFactor=density;Call(hud,"Layout");Canvas.ForceUpdateCanvases();var coach=(RectTransform)safe.Find("TutorialCoach");var skip=(RectTransform)coach.Find("SkipTutorial");var body=(RectTransform)coach.Find("Instruction");var title=(RectTransform)coach.Find("Lesson");Rect r=UiMetrics.ScreenRect(skip),c=UiMetrics.ScreenRect(coach);Check(r.height/density>=48&&r.width/density>=48,"exit retains 48 logical point target");Check(r.height/density<=56,"exit must rescale from initialization density instead of remaining oversized: "+r.height/density);Check(Inside(c,r),"exit stays in coach");Check(!r.Overlaps(UiMetrics.ScreenRect(body))&&!r.Overlaps(UiMetrics.ScreenRect(title)),"exit cannot overlap tutorial text");Check(!r.Overlaps(UiMetrics.ScreenRect((RectTransform)hud.Jump.transform)),"exit cannot overlap gameplay action");Check(Inside(UiMetrics.ScreenRect(safe),c),"coach remains in safe pane");}
  }else{
   RemoveModal(hud);hud.ShowHome();RemoveModal(hud);hud.ShowStyle();RemoveModal(hud);hud.ShowSettings();profile.Language=language=="de"?"en":"de";hud.Refresh();RemoveModal(hud);Call(hud,"Reopen");safe.sizeDelta=new Vector2(size.y,size.x);Call(hud,"Layout");RemoveModal(hud);Call(hud,"Reopen");Canvas.ForceUpdateCanvases();
   var close=safe.Find("ModalBackdrop/ModalCard/Close").GetComponent<Button>();Check(!close.GetComponentInChildren<Text>().text.Contains("GAME")&&!close.GetComponentInChildren<Text>().text.Contains("SPIEL"),"child settings has a parent back action");var back=close.onClick;RemoveModal(hud);back.Invoke();Check(hud.ModalOpen&&course.Paused,"back keeps gameplay paused");Check(safe.Find("ModalBackdrop/ModalCard/Viewport/Content/DesignGrid")!=null,"refresh/locale/orientation preserves settings parent style page");
   RemoveModal(hud);hud.ShowHome();RemoveModal(hud);hud.ShowSettings();var store=root.AddComponent<StorePurchases>();var videos=root.AddComponent<RewardedVideos>();course.Store=store;course.Videos=videos;
   foreach(var provider in new UnityEngine.Object[]{store,videos}){Set(provider,"<IsPresenting>k__BackingField",true);hud.Close();Check(hud.ModalOpen&&course.Paused,"explicit resume cannot leave presenting provider");safe.Find("ModalBackdrop/ModalCard/Close").GetComponent<Button>().onClick.Invoke();Check(safe.Find("ModalBackdrop/ModalCard/Viewport/Content/Language")!=null&&course.Paused,"parent back cannot leave presenting provider");hud.ShowStyle();Check(safe.Find("ModalBackdrop/ModalCard/Viewport/Content/Language")!=null&&course.Paused,"opening another menu cannot leave presenting provider");Set(provider,"<IsPresenting>k__BackingField",false);}
   RemoveModal(hud);hud.Close();Check(!hud.ModalOpen&&!course.Paused,"explicit resume closes modal when providers are idle");Check(profile.Crystals==428&&profile.Step==0&&profile.Checkpoint==0,"menu navigation preserves wallet/run");
  }
 }finally{UiMetrics.QaPointScale=null;FloorSave.QaKey=previousKey;if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
}
#endif
