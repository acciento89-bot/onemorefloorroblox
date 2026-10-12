#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.UI;
using Kamilunavo.OneMoreFloor.Visuals;

public static class FloorLandingInstrumentValidation
{
 static readonly BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool yes,string message){if(!yes)throw new InvalidOperationException("LANDING_INSTRUMENT: "+message);}
 public static void Validate(){var failures=new List<string>();try{Meter();}catch(Exception e){failures.Add(e.GetBaseException().Message);}try{Mapping();}catch(Exception e){failures.Add(e.GetBaseException().Message);}try{Needle();}catch(Exception e){failures.Add(e.GetBaseException().Message);}try{Botany();}catch(Exception e){failures.Add(e.GetBaseException().Message);}if(failures.Count>0)throw new InvalidOperationException(string.Join("; ",failures));Debug.Log("FLOOR_LANDING_INSTRUMENT_PASS actualConceptReview=REQUIRED");}
 static void Needle(){var root=new GameObject("NeedleGeometryFixture",typeof(RectTransform));try{var graphic=FloorTimingInstrument.Add(root.transform,"Needle",FloorTimingInstrument.Face.Needle,Vector2.zero,Vector2.zero);graphic.rectTransform.sizeDelta=new Vector2(4.2f,25);using var mesh=new VertexHelper();typeof(FloorTimingInstrument).GetMethod("OnPopulateMesh",Private,null,new[]{typeof(VertexHelper)},null).Invoke(graphic,new object[]{mesh});var vertex=new UIVertex();for(int i=0;i<mesh.currentVertCount;i++){mesh.PopulateUIVertex(ref vertex,i);Check(graphic.rectTransform.rect.Contains(new Vector2(vertex.position.x,vertex.position.y)+Vector2.one*.00001f)||Mathf.Abs(vertex.position.x)<=2.1001f&&Mathf.Abs(vertex.position.y)<=12.5001f,"needle artwork stays inside its narrow rect rather than obscuring the landing windows with a disk");}}finally{UnityEngine.Object.DestroyImmediate(root);}}
 static void Meter(){var root=new GameObject("LandingInstrumentFixture");Canvas canvas=null;try{
  var course=root.AddComponent<FloorCourse>();typeof(FloorCourse).GetField("<Profile>k__BackingField",Private).SetValue(course,new FloorProfile{TimingVersion=1});course.Player=new GameObject("Player").transform;course.Player.SetParent(root.transform,false);var hud=root.AddComponent<FloorHud>();hud.Initialize(course);var safe=(RectTransform)typeof(FloorHud).GetField("_safe",Private).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();
  var meter=(RectTransform)safe.Find("TimingMeter");Check(meter.Find("LiftStation")!=null&&meter.Find("TowerStation")!=null,"Concept instrument visibly distinguishes elevator and tower at the two ends");
  foreach(var graphic in meter.GetComponentsInChildren<Graphic>(true))Check(!graphic.raycastTarget,"all instrument decoration passes through gameplay touches");
  canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=1;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);UiMetrics.QaPointScale=1;
  foreach(var size in new[]{new Vector2(280,580),new Vector2(375,627),new Vector2(734,343)})foreach(bool practice in new[]{false,true}){
   safe.sizeDelta=size;typeof(FloorCourse).GetField("<Tutorial>k__BackingField",Private).SetValue(course,practice?new ElevatorTutorial():null);typeof(FloorHud).GetMethod("Layout",Private).Invoke(hud,null);Canvas.ForceUpdateCanvases();
   Check(meter.gameObject.activeSelf,"timing instrument is visible during the timing tutorial too");var meterRect=UiMetrics.ScreenRect(meter);Check(!meterRect.Overlaps(UiMetrics.ScreenRect((RectTransform)hud.Jump.transform)),"instrument stays directly above, separate from the transfer action");
   Check(meterRect.height>=32&&meterRect.height<=40,"Concept instrument retains readable slim height");var track=(RectTransform)meter.Find("MeterTrack");Check(track.rect.width>meter.rect.width*.65f&&track.rect.width<meter.rect.width*.80f,"track reserves end stations without losing the useful timing range");
   if(practice)Check(!meterRect.Overlaps(UiMetrics.ScreenRect((RectTransform)safe.Find("TutorialCoach"))),"coach leaves the actual landing instrument unobstructed in both orientations");
  }
  typeof(FloorHud).GetMethod("UpdateMeter",Private).Invoke(hud,null);var landing=(RectTransform)meter.Find("MeterTrack/LandingWindow");var perfect=(RectTransform)meter.Find("MeterTrack/PerfectWindow");Check(landing.anchorMax.x-landing.anchorMin.x>=.32f&&perfect.anchorMax.x-perfect.anchorMin.x>=.10f,"Concept centre zones are readable instead of tiny colour slivers");
 }finally{UiMetrics.QaPointScale=null;if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 static void Mapping(){foreach(float window in new[]{.30f,.25f,.22f}){float previous=0;for(int tick=-600;tick<=600;tick++){float error=tick*window/100;float needle=FloorTimingInstrument.Position(error,window);Check(needle>=previous&&needle>=.01f-.00001f&&needle<=.99f+.00001f,"display calibration is monotonic and bounded");previous=needle;int shown=Mathf.Abs(needle-.5f)>.180001f?0:Mathf.Abs(needle-.5f)<=.060001f?2:1;Check(shown==ElevatorRules.Hit(error,0,window),"display zones agree with actual hit authority: window="+window.ToString("R")+" tick="+tick+" error="+error.ToString("R")+" needle="+needle.ToString("R")+" shown="+shown+" actual="+ElevatorRules.Hit(error,0,window));}}}
 static void Botany(){var root=new GameObject("ConceptBotanyFixture");try{
  var shade=FloorArchitecturePalette.Get(root.transform,FloorArchitectureSurface.Leaf);var light=FloorArchitecturePalette.Get(root.transform,FloorArchitectureSurface.LeafLight);FloorArchitectureDetails.Plant(root.transform,Vector3.zero,shade,light,19);FloorArchitectureDetails.Ivy(root.transform,Vector3.zero,1.64f,19,shade,light);
  foreach(var filter in root.GetComponentsInChildren<MeshFilter>()){var mesh=filter.sharedMesh;Check(mesh.uv.Length==mesh.vertexCount,"authored foliage has leaf coordinates for vein shading");Check(mesh.vertexCount<650,"folded leaves avoid opposite-face cancellation and expanded fallback geometry");foreach(var normal in mesh.normals)Check(normal.sqrMagnitude>.5f,"foliage retains lighting normals");}
  Check(shade.shader.name=="Floor/Foliage"&&light.shader==shade.shader,"shared two-sided foliage surface shades both faces without duplicate reversed triangles");Check(root.GetComponentsInChildren<Collider>().Length==0,"plants remain scenery");
 }finally{UnityEngine.Object.DestroyImmediate(root);}}
}
#endif
