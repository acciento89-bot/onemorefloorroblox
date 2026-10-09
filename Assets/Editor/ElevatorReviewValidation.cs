#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.CameraSystem;
public static class ElevatorReviewValidation
{
 static void Check(bool value,string label){if(!value)throw new Exception(label);}
 public static void Validate(){var errors=new List<string>();try{var p=new FloorProfile{Step=7,Checkpoint=4,RewardedFloor=7,Crystals=100};ElevatorRules.Migrate(p);Check(p.RewardedFloor==7,"paid frontier regression: old floors reward again");}catch(Exception e){errors.Add(e.Message);}
 try{var root=new GameObject("TimingTouchRegression");try{var button=root.AddComponent<Kamilunavo.OneMoreFloor.Input.PressButton>();var pressed=typeof(Kamilunavo.OneMoreFloor.Input.PressButton).GetEvent("Pressed");Check(pressed!=null,"timing input must launch at pointer timestamp rather than next frame");bool dispatched=false;pressed.AddEventHandler(button,new Action(()=>dispatched=true));button.OnPointerDown(new UnityEngine.EventSystems.PointerEventData(null));Check(dispatched,"timing pointer dispatch is immediate");}finally{UnityEngine.Object.DestroyImmediate(root);}}catch(Exception e){errors.Add(e.Message);}
 try{CameraFixture();}catch(Exception e){errors.Add(e.Message);}if(errors.Count>0)throw new Exception(string.Join("; ",errors));Debug.Log("ELEVATOR_REVIEW_REGRESSION_PASS");}
 public static void CameraFixture(){var root=new GameObject("CameraRegression");try{var course=root.AddComponent<FloorCourse>();typeof(FloorCourse).GetField("<Profile>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(course,new FloorProfile{TimingVersion=1});course.Player=new GameObject("PlayerProbe").transform;course.Player.SetParent(root.transform);var steps=(List<StepMarker>)typeof(FloorCourse).GetField("_steps",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(course);
 for(int f=0;f<2;f++){var deck=new GameObject("DeckProbe");deck.transform.SetParent(root.transform);deck.transform.position=new Vector3(0,f*1.7f,f*3.7f);var marker=new GameObject("SurfaceProbe").AddComponent<StepMarker>();marker.transform.SetParent(deck.transform,false);marker.Index=f;steps.Add(marker);}
 var obj=new GameObject("CameraProbe",typeof(Camera),typeof(OrbitCamera));obj.transform.SetParent(root.transform);var cam=obj.GetComponent<Camera>();cam.fieldOfView=58;cam.aspect=667f/375;var orbit=obj.GetComponent<OrbitCamera>();orbit.Target=course.Player;orbit.Course=course;typeof(OrbitCamera).GetMethod("Awake",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(orbit,null);typeof(OrbitCamera).GetMethod("LateUpdate",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(orbit,null);
 foreach(var marker in steps)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){var point=cam.WorldToViewportPoint(marker.transform.position+new Vector3(x*2.4f,.6f,z*1.9f));Check(point.z>0&&point.x>=.079f&&point.x<=.921f&&point.y>=.229f&&point.y<=.771f,"actual landscape source/target corner overlaps HUD: "+point);}
 }finally{UnityEngine.Object.DestroyImmediate(root);}}
}
#endif
