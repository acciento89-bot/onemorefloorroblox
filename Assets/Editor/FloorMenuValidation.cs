#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.UI;
public static class FloorMenuValidation
{
 static int checks;
 static void Check(bool valid,string name){checks++;if(!valid)throw new Exception("LIFT_MENU: "+name);}
 public static void Validate(){checks=0;ValidateGraphicFactories();var legacy=FloorSave.Parse("{\"Schema\":1,\"Crystals\":428,\"Step\":7,\"Checkpoint\":4}");Check(!legacy.TutorialCompleted&&legacy.Crystals==428&&legacy.Step==7,"missing tutorial field retains old profile");legacy.TutorialCompleted=true;var copy=FloorSave.Parse(JsonUtility.ToJson(legacy));Check(copy.TutorialCompleted&&copy.Crystals==428&&copy.Step==7,"tutorial completion round trips without progress migration");
  var lesson=new ElevatorTutorial();lesson.Landing(false,1);lesson.AtCheckpoint();Check(lesson.Stage==ElevatorTutorialStage.Perfect,"checkpoint cannot replace actual perfect");lesson.Landing(true,2);lesson.AtCheckpoint();lesson.Risk();lesson.Landing(true,5);lesson.AtCheckpoint();lesson.Banked();Check(lesson.Complete,"actual risk landing then banking completes");
  foreach(var size in new[]{new Vector2(375,627),new Vector2(393,759),new Vector2(430,839),new Vector2(734,343),new Vector2(809,372),new Vector2(280,580)})foreach(var lang in new[]{"de","en"})LayoutFixture(size,lang);Debug.Log("LIFT_MENU_EDITOR_PASS checks="+checks);
 }
 static void ValidateGraphicFactories(){var canvas=UiFactory.Canvas();try{var pane=UiFactory.Panel(canvas.transform,"ClippedGraphicProbe",Color.white,Vector2.zero,Vector2.one);pane.gameObject.AddComponent<RectMask2D>();UiFactory.Label(pane,"Label","Graphic probe",16,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white);HudIcons.Add(pane,HudIcons.Kind.Menu);LiftTowerGraphic.Create(pane,0);Kamilunavo.OneMoreFloor.Input.VirtualJoystick.Create(pane,Vector2.zero,Vector2.one);Kamilunavo.OneMoreFloor.Input.PressButton.Create(pane,"Circle",Vector2.zero,Vector2.one);Kamilunavo.OneMoreFloor.Input.PressButton.Create(pane,"Wide",Vector2.zero,Vector2.one,true);foreach(var graphic in canvas.GetComponentsInChildren<Graphic>())Check(graphic.GetComponent<CanvasRenderer>()!=null,"graphic factory owns CanvasRenderer "+graphic.name);Canvas.ForceUpdateCanvases();}finally{UnityEngine.Object.DestroyImmediate(canvas.gameObject);}}
 static void LayoutFixture(Vector2 size,string language){var root=new GameObject("LiftMenuProbe");Canvas canvas=null;try{var course=root.AddComponent<FloorCourse>();var p=new FloorProfile{Language=language,UnlockedRealm=2,TimingVersion=1,Crystals=428};typeof(FloorCourse).GetField("<Profile>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(course,p);course.Player=new GameObject("Player").transform;course.Player.SetParent(root.transform);var hud=root.AddComponent<FloorHud>();hud.Initialize(course);var safe=(RectTransform)typeof(FloorHud).GetField("_safe",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=3;UiMetrics.QaPointScale=3;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.sizeDelta=size;typeof(FloorHud).GetMethod("Layout",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,null);var modalField=typeof(FloorHud).GetField("_modal",BindingFlags.Instance|BindingFlags.NonPublic);UnityEngine.Object.DestroyImmediate(((RectTransform)modalField.GetValue(hud)).gameObject);modalField.SetValue(hud,null);hud.ShowHome();Canvas.ForceUpdateCanvases();
   var card=(RectTransform)safe.Find("ModalBackdrop/ModalCard");var viewport=(RectTransform)card.Find("Viewport");Check(!viewport.GetComponent<ScrollRect>().vertical,size+" home uses fixed hierarchy");var content=viewport.Find("Content");Check(content.Find("TowerPreview/LiftElevation")!=null,size+" authored lift preview exists");
   Rect usable=UiMetrics.ScreenRect(safe),body=UiMetrics.ScreenRect(viewport);var buttons=card.GetComponentsInChildren<Button>();foreach(var b in buttons){Rect r=UiMetrics.ScreenRect((RectTransform)b.transform);Check(r.width/3>=47.999f&&r.height/3>=47.999f,size+" "+language+" >=48 points "+b.name);Check(Inside(usable,r),size+" safe bounds "+b.name);if(b.transform.IsChildOf(content))Check(Inside(body,r),size+" visible without scrolling "+b.name);}
   for(int a=0;a<buttons.Length;a++)for(int b=a+1;b<buttons.Length;b++)Check(!UiMetrics.ScreenRect((RectTransform)buttons[a].transform).Overlaps(UiMetrics.ScreenRect((RectTransform)buttons[b].transform)),size+" controls never overlap "+buttons[a].name+"/"+buttons[b].name);
   Check(Inside(body,UiMetrics.ScreenRect((RectTransform)content.Find("TowerPreview"))),size+" hero inside viewport");
  }finally{UiMetrics.QaPointScale=null;if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 static bool Inside(Rect outer,Rect inner)=>inner.xMin>=outer.xMin-.5f&&inner.yMin>=outer.yMin-.5f&&inner.xMax<=outer.xMax+.5f&&inner.yMax<=outer.yMax+.5f;
}
#endif
