#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
public static class FloorValidation
{
 static int checks;static void Check(bool yes,string name){checks++;if(!yes)throw new Exception(name);}
 public static void ValidateAll(){FloorArtImports.Ensure();ValidateRules();ValidateUI();ValidateInput();ValidateGeometry();ValidateLifetime();ValidateReviewRegressions();ValidateMovement();CommerceValidation.Validate();Debug.Log("FLOOR_ALL_PASS checks="+checks);}
 public static void ValidateInput(){
 var canvas=Kamilunavo.OneMoreFloor.UI.UiFactory.Canvas();var j=Kamilunavo.OneMoreFloor.Input.VirtualJoystick.Create(canvas.transform,Vector2.zero,Vector2.one);
 var r=(RectTransform)j.transform;r.anchorMin=r.anchorMax=Vector2.zero;r.pivot=Vector2.zero;r.sizeDelta=new Vector2(112,112);Canvas.ForceUpdateCanvases();
 var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){pointerId=100,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};typeof(Kamilunavo.OneMoreFloor.Input.VirtualJoystick).GetMethod("Awake",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(j,null);j.OnPointerDown(pointer);Check(j.Value.sqrMagnitude<.001f,"joystick center is neutral with bottom-left pivot");
 pointer.position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center+Vector2.left*50));j.OnDrag(pointer);Check(j.Value.x<-.8f&&Mathf.Abs(j.Value.y)<.01f,"joystick can move left");j.ResetInput();UnityEngine.Object.DestroyImmediate(canvas.gameObject);Debug.Log("FLOOR_INPUT_PASS checks="+checks);
 }
 public static void ValidateReviewRegressions(){var errors=new System.Collections.Generic.List<string>();try{ValidateScroll();}catch(Exception e){errors.Add(e.Message);}try{ValidateCompact();}catch(Exception e){errors.Add(e.Message);}if(errors.Count>0)throw new Exception(string.Join("; ",errors));Debug.Log("FLOOR_REVIEW_REGRESSIONS_PASS checks="+checks);}
 static Kamilunavo.OneMoreFloor.UI.FloorHud Fixture(out GameObject root,out Canvas canvas){root=new GameObject("HudProbe");var course=root.AddComponent<FloorCourse>();typeof(FloorCourse).GetField("<Profile>k__BackingField",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(course,new FloorProfile());course.Player=new GameObject("ProbePlayer").transform;course.Player.SetParent(root.transform);var hud=root.AddComponent<Kamilunavo.OneMoreFloor.UI.FloorHud>();hud.Initialize(course);var safe=(RectTransform)typeof(Kamilunavo.OneMoreFloor.UI.FloorHud).GetField("_safe",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();var modal=typeof(Kamilunavo.OneMoreFloor.UI.FloorHud).GetField("_modal",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);UnityEngine.Object.DestroyImmediate(((RectTransform)modal.GetValue(hud)).gameObject);modal.SetValue(hud,null);return hud;}
 public static void ValidateScroll(){var hud=Fixture(out var root,out var canvas);try{hud.ShowAchievements();var viewport=canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Viewport");Check(viewport.GetComponent<UnityEngine.UI.Image>().raycastTarget,"achievement descriptions and gaps accept scrolling");Check(!canvas.transform.Find("SafeArea").GetComponent<UnityEngine.UI.Image>().raycastTarget,"gameplay root still accepts camera touches");}finally{UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateCompact(){var hud=Fixture(out var root,out var canvas);try{canvas.GetComponent<UnityEngine.UI.CanvasScaler>().enabled=false;canvas.scaleFactor=3*Mathf.Sqrt(375f/390f*667f/844f);Kamilunavo.OneMoreFloor.UI.UiMetrics.QaPointScale=3;typeof(Kamilunavo.OneMoreFloor.UI.FloorHud).GetMethod("Layout",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(hud,null);hud.ShowAchievements();
 var menu=(RectTransform)canvas.transform.Find("SafeArea/Menu");var close=(RectTransform)canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Close");Check(menu.rect.width*canvas.scaleFactor/3>=47.999f&&menu.rect.height*canvas.scaleFactor/3>=47.999f,"compact375x667 menu >=48 logical points");Check(close.rect.height*canvas.scaleFactor/3>=47.999f,"compact375x667 close >=48 logical points");}finally{Kamilunavo.OneMoreFloor.UI.UiMetrics.QaPointScale=null;UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateLifetime(){
 var root=new GameObject("LifetimeProbe");Kamilunavo.OneMoreFloor.Visuals.PlatformArt.Build(root.transform,0,0);
 var originals=new System.Collections.Generic.HashSet<Mesh>();foreach(var f in root.GetComponentsInChildren<MeshFilter>())originals.Add(f.sharedMesh);
 Kamilunavo.OneMoreFloor.Visuals.MeshArt.Batch(root);var combined=new System.Collections.Generic.HashSet<Mesh>();foreach(var f in root.GetComponentsInChildren<MeshFilter>())combined.Add(f.sharedMesh);
 UnityEngine.Object.DestroyImmediate(root);
 foreach(var mesh in originals)Check(mesh==null,"rebuilt island releases original mesh");foreach(var mesh in combined)Check(mesh==null,"rebuilt island releases batched mesh");Debug.Log("FLOOR_LIFETIME_PASS checks="+checks);
 }
 public static void ValidateGeometry(){
 var root=new GameObject("GeometryProbe");Kamilunavo.OneMoreFloor.Visuals.PlatformArt.Build(root.transform,0,0);
 foreach(var filter in root.GetComponentsInChildren<MeshFilter>())foreach(var normal in filter.sharedMesh.normals)Check(normal.sqrMagnitude>.5f,"valid lighting normal "+filter.name);
 UnityEngine.Object.DestroyImmediate(root);Debug.Log("FLOOR_GEOMETRY_PASS checks="+checks);
 }
 public static void ValidateUI(){
 var safe=new Rect(0,0,1,1);var division=new Rect(.48f,0,.04f,1);var pane=Kamilunavo.OneMoreFloor.UI.UsableRegion.Choose(safe,division);Check(pane.xMax<=division.xMin+.001f||pane.xMin>=division.xMax-.001f,"either maximum-area pane excludes division despite float tie");
 var canvas=Kamilunavo.OneMoreFloor.UI.UiFactory.Canvas();var root=Kamilunavo.OneMoreFloor.UI.UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);
 Check(!root.GetComponent<UnityEngine.UI.Image>().raycastTarget,"transparent root camera touch");
 var label=Kamilunavo.OneMoreFloor.UI.UiFactory.Label(root,"Label","Text",30,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white);
 Check(!label.raycastTarget,"label does not intercept touch");UnityEngine.Object.DestroyImmediate(canvas.gameObject);Debug.Log("FLOOR_UI_PASS checks="+checks);
 }
 public static void ValidateMovement(){var root=new GameObject("MovingProbe");try{var move=root.AddComponent<MovingPlatform>();move.Distance=.7f;move.Speed=.8f;move.Tick(.25f);Check(move.Delta.x>.1f&&move.Delta.x<.2f,"moving bounded delta");var at=root.transform.position;move.Tick(0);Check(root.transform.position==at&&move.Delta==Vector3.zero,"paused clock no accumulated carry");for(int i=0;i<30;i++)Check(!CoursePatterns.Moves(i)||(i+1)%5!=0,"checkpoint never moves");}finally{UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateRules(){
 var day=new DateTime(2026,10,8,0,0,0,DateTimeKind.Utc);var p=new FloorProfile();
 Check(FloorRules.ClaimDaily(p,day)&&p.Crystals==100,"daily first");Check(!FloorRules.ClaimDaily(p,day)&&!FloorRules.ClaimDaily(p,day.AddDays(-1)),"daily replay/time reversal");
 for(int i=1;i<9;i++)FloorRules.ClaimDaily(p,day.AddDays(i));Check(p.DailyStreak==7&&p.Crystals==1230,"UTC streak7 cap");
 p=new FloorProfile();Check(!FloorRules.Land(p,2,true),"skipped floor blocked");
 for(int i=1;i<=7;i++){Check(FloorRules.Land(p,i,true),"ordered floor");Check(!FloorRules.Land(p,i,true),"duplicate floor");}
 Check(p.Checkpoint==4&&p.RewardedFloor==7&&p.Step==7&&p.BestFloor==7&&p.BestScore==980,"floor5 checkpoint");int coins=p.Crystals,score=p.Score,perfects=p.Perfects;
 FloorRules.Recover(p);Check(p.Step==4&&p.Falls==1,"immediate saved checkpoint");
 for(int i=5;i<=7;i++)Check(FloorRules.Land(p,i,true),"replay course advances");Check(p.Crystals==coins&&p.Score==score&&p.Perfects==perfects,"replay never mints score coins perfects");
 for(int i=8;i<=29;i++)Check(FloorRules.Land(p,i,true),"tower30");Check(p.Checkpoint==29&&p.Step==29&&p.Score==7250&&p.BestFloor==29&&p.BestScore==7250&&p.Crystals==133&&p.Perfects==29,"all30 floors rewards");
 Check(FloorRules.Complete(p,day)==1&&p.UnlockedRealm==1,"fall gives1star and unlock");coins=p.Crystals;Check(FloorRules.Complete(p,day)==1&&p.Crystals==coins,"complete duplicate");
 p=new FloorProfile{Step=29,RewardedFloor=29,Elapsed=240};Check(FloorRules.Complete(p,day)==3,"240seconds3stars");p=new FloorProfile{Step=29,RewardedFloor=29,Elapsed=240.01f};Check(FloorRules.Complete(p,day)==2,"slow clean2stars");
 p=new FloorProfile{Step=29,RewardedFloor=29,Challenge=true,Perfects=16,RunDay=FloorRules.Day(day)};FloorRules.Complete(p,day);Check(p.Crystals==75&&p.UnlockedRealm==0,"daily challenge reward");FloorRules.Complete(p,day);Check(p.Crystals==75,"challenge once");
 p=new FloorProfile{Step=29,RewardedFloor=29,Challenge=true,Perfects=29,RunDay=FloorRules.Day(day.AddDays(-1))};FloorRules.Complete(p,day);Check(p.Crystals==0,"yesterday challenge cannot claim");
 p=new FloorProfile{Crystals=500};Check(FloorRules.SelectStyle(p,1)&&p.Crystals==425,"earned style");Check(FloorRules.SelectStyle(p,1)&&p.Crystals==425,"owned style no charge");
 p.Step=7;p.RewardedFloor=9;p.Checkpoint=4;p.Score=100;var restored=FloorSave.Parse(JsonUtility.ToJson(p));Check(restored.Step==7&&restored.RewardedFloor==9&&restored.Checkpoint==4&&restored.Styles[1],"save keeps reward frontier checkpoint");
 Check(FloorSave.Parse("invalid").Step==0,"corrupt fallback");restored=FloorSave.Parse("{\"Schema\":1,\"Step\":99,\"Checkpoint\":7,\"RewardedFloor\":99,\"Crystals\":-1,\"Styles\":[],\"Stars\":[]}");Check(restored.Step==29&&restored.Checkpoint==4&&restored.RewardedFloor==29&&restored.Crystals==0&&restored.Styles.Length==8,"normalized profile");
 string original=PlayerPrefs.GetString(FloorSave.Key,"");try{PlayerPrefs.SetString(FloorSave.Key,"{\"Schema\":99,\"Crystals\":123}");FloorSave.Load();Check(!FloorSave.Writable,"unknown schema economy disabled");bool rejected=false;try{FloorSave.Save(new FloorProfile());}catch(InvalidOperationException){rejected=true;}Check(rejected&&PlayerPrefs.GetString(FloorSave.Key,"").Contains("123"),"unknown schema original preserved");}finally{PlayerPrefs.SetString(FloorSave.Key,original);FloorSave.Load();}
 for(int realm=0;realm<3;realm++)for(int seed=0;seed<30;seed++){var a=CoursePatterns.Points(realm,seed);var b=CoursePatterns.Points(realm,seed);Check(a.Length==30,"thirtyfloors");for(int i=1;i<30;i++){Check(a[i]==b[i],"deterministic");var d=a[i]-a[i-1];float flight=(9.2f+Mathf.Sqrt(84.64f-44*d.y))/22;Check(d.y>0&&d.y<1.3f&&new Vector2(Mathf.Abs(d.x)+1.4f,d.z).magnitude-2.1f<6.2f*flight-.35f,"worst moving gap reachable");}}
 Debug.Log("FLOOR_RULES_PASS checks="+checks);
 }
}
#endif
