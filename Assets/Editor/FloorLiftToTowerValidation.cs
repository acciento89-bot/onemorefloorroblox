#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;

public static class FloorLiftToTowerValidation
{
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("LIFT_TO_TOWER: "+message);}
 public static void Validate(){var root=new GameObject("LiteralLiftToTowerFixture");try{
  var course=root.AddComponent<FloorCourse>();var profile=new FloorProfile{TimingVersion=1,Step=7,Checkpoint=4,RewardedFloor=7,Crystals=137,Style=2};profile.Styles[2]=true;profile.Normalize();typeof(FloorCourse).GetField("<Profile>k__BackingField",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(course,profile);course.Player=new GameObject("FixtureRunner").transform;course.Player.SetParent(root.transform,false);
  typeof(FloorCourse).GetMethod("BuildRoute",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(course,null);
  Check(root.GetComponentsInChildren<Transform>(true).Count(t=>t.name=="CopperLiftCabin")==1,"gameplay owns ONE carriage, not a chain of lift-to-lift targets");
  Check(course.Steps.Count==30,"all thirty accepted floor indices remain available");
  foreach(var floor in course.Steps){Check(floor.transform.parent.Find("ArchitecturalFloorBay")!=null,"every destination is an actual tower balcony");Check(floor.transform.parent.GetComponent<ElevatorMotion>()==null,"tower landings do not move with a cabin");Check(floor.transform.position.x>2,"tower lies beside the lift instead of along a second-cabin runway");}
  var cabin=root.transform.Find("SingleVerticalLift");Check(cabin!=null,"single live vertical lift is independently owned");var motion=cabin.GetComponent("TowerLiftMotion");Check(motion!=null,"carriage has the dedicated vertical ride controller");var at=motion.GetType().GetMethod("At");var a=(Vector3)at.Invoke(motion,new object[]{0f});var b=(Vector3)at.Invoke(motion,new object[]{1.8f});Check(a.x==b.x&&a.z==b.z&&Mathf.Abs(a.y-b.y)>1,"lift moves vertically while the adjacent tower remains stationary");
  var rails=root.transform.Find("VerticalTowerLiftGuides");Check(rails.position==Vector3.zero,"continuous guides are anchored to route base, not restored carriage height");var railBounds=rails.GetComponentsInChildren<Renderer>().Select(r=>r.bounds).ToArray();Check(railBounds.Min(r=>r.min.y)<=-4&&railBounds.Max(r=>r.max.y)>=29*TowerLiftMotion.FloorSpacing+4,"guides cover both lower checkpoint recovery and highest floor after saved-floor rebuild");
  Check(!course.LiftReady&&course.PredictedHit==0&&!course.LaunchTransfer()&&!profile.TimingInFlight,"new ride prevents premature jump and false perfect feedback");
  Check(profile.Step==7&&profile.Checkpoint==4&&profile.RewardedFloor==7&&profile.Crystals==137&&profile.Style==2,"reconstruction preserves progress, reward frontier, wallet and bought design");
  typeof(FloorCourse).GetMethod("BuildRoute",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(course,null);Check(root.GetComponentsInChildren<Transform>(true).Count(t=>t.name=="CopperLiftCabin")==1&&root.GetComponentsInChildren<Transform>(true).Count(t=>t.name=="VerticalTowerLiftGuides")==1,"rebuilding releases the previous carriage and continuous guide kit");
  Debug.Log("FLOOR_LIFT_TO_TOWER_CONSTRUCTION_PASS");
 }finally{UnityEngine.Object.DestroyImmediate(root);}}
}
#endif
