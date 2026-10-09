#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
public static class ElevatorValidation
{
 static int checks;
 static void Check(bool value,string name){checks++;if(!value)throw new Exception("ELEVATOR: "+name);}
 public static void Validate(){checks=0;
  Check(ElevatorRules.Hit(0,0,1)==2,"center perfect");Check(ElevatorRules.Hit(.7f,0,1)==1,"edge normal");Check(ElevatorRules.Hit(1.01f,0,1)==0,"outside bay misses");
  Check(ElevatorRules.Hit(float.NaN,0,1)==0,"invalid landing rejected");
  for(int r=0;r<3;r++)for(int f=1;f<30;f++)for(int t=0;t<60;t++){
   float x=ElevatorRules.MotionX(f,r,31,t*.1f);Check(x==ElevatorRules.MotionX(f,r,31,t*.1f),"deterministic motion");Check(Math.Abs(x)<=3.01f,"bounded elevator");}
  Check(ElevatorRules.HalfBay(1)>ElevatorRules.HalfBay(25),"early generous bay");
  var p=new FloorProfile{Crystals=600,UnlockedRealm=2,Step=7,Checkpoint=4,Style=2};p.Styles[2]=true;
  ElevatorRules.Migrate(p);Check(p.TimingVersion==1&&p.Step==4&&p.Crystals==600&&p.Style==2&&p.Styles[2],"legacy ownership wallet checkpoint preserved");
  ElevatorRules.NewRun(p);for(int f=1;f<=9;f++)Check(ElevatorRules.Land(p,f,true),"ordered advance");
  Check(p.Checkpoint==9&&p.PendingCoins>0&&p.Crystals==600&&p.PerfectChain==9,"pending distinct wallet");
  int pending=p.PendingCoins;Check(!ElevatorRules.Land(p,9,true)&&p.PendingCoins==pending,"duplicate landing no mint");
  Check(ElevatorRules.Bank(p)==pending&&p.Crystals==600+pending,"bank once");Check(ElevatorRules.Bank(p)==0,"bank duplicate");Check(!ElevatorRules.Land(p,10,true),"banked round cannot continue");
  p=FloorSave.Parse(JsonUtility.ToJson(p));Check(ElevatorRules.Bank(p)==0&&p.RunBanked,"reload bank idempotent");
  ElevatorRules.NewRun(p);for(int f=1;f<=7;f++)ElevatorRules.Land(p,f,true);int wallet=p.Crystals,frontier=p.RewardedFloor;
  ElevatorRules.Recover(p);Check(p.Step==4&&p.PendingCoins==0&&p.Crystals==wallet&&p.PerfectChain==0,"fall only pending loss checkpoint");
  for(int f=5;f<=frontier;f++)ElevatorRules.Land(p,f,true);Check(p.PendingCoins==0,"replay cannot recover lost rewards");
  for(int f=frontier+1;f<30;f++)Check(ElevatorRules.Land(p,f,true),"complete30");Check(p.Checkpoint==29&&p.Step==29&&p.PendingCoins>0,"floor30 checkpoint");
  Check(ElevatorRules.Bank(p)>0&&ElevatorRules.Bank(p)==0,"completion payout exactly once");
  var old=FloorSave.Parse("{\"Schema\":1,\"Crystals\":421,\"Checkpoint\":4,\"Step\":7,\"Language\":\"en\"}");ElevatorRules.Migrate(old);Check(old.Crystals==421&&old.Language=="en"&&old.Step==4,"old serialized profile migration");
  var saved=FloorSave.Parse(JsonUtility.ToJson(old));ElevatorRules.Migrate(saved);Check(saved.Step==4&&saved.Crystals==421,"migration repeat safe");
  var migrated=new FloorProfile{Step=7,Checkpoint=4,RewardedFloor=7,Crystals=100};ElevatorRules.Migrate(migrated);Check(migrated.RewardedFloor==7,"legacy paid frontier survives conversion");for(int f=5;f<=7;f++)ElevatorRules.Land(migrated,f,true);Check(migrated.PendingCoins==0,"legacy replay cannot mint again");
  var interrupted=new FloorProfile{TimingVersion=1,Step=7,Checkpoint=4,RewardedFloor=7,PendingCoins=38,PerfectChain=7,RiskLevel=1,TimingInFlight=true,Crystals=150};ElevatorRules.Restore(interrupted,DateTime.UtcNow);Check(interrupted.Step==4&&interrupted.PendingCoins==0&&interrupted.Falls==1&&!interrupted.TimingInFlight&&interrupted.Crystals==150,"interrupted flight recovers checkpoint loses pending only");ElevatorRules.Restore(interrupted,DateTime.UtcNow);Check(interrupted.Falls==1,"interrupted recovery idempotent");
  var idle=new FloorProfile{TimingVersion=1,Step=7,Checkpoint=4,RewardedFloor=7,PendingCoins=38,PerfectChain=7,RiskLevel=1};ElevatorRules.Restore(idle,DateTime.UtcNow);Check(idle.Step==7&&idle.PendingCoins==38&&idle.PerfectChain==7,"idle reload preserves exact position and risk");
  var terminal=new FloorProfile{TimingVersion=1,Step=29,Checkpoint=29,RewardedFloor=29,PendingCoins=83,Crystals=100};ElevatorRules.Restore(terminal,DateTime.UtcNow);Check(terminal.Completed&&terminal.RunBanked&&terminal.Crystals==183&&terminal.PendingCoins==0,"unfinished terminal reload banks and completes");ElevatorRules.Restore(terminal,DateTime.UtcNow);Check(terminal.Crystals==183,"terminal repair does not repay");
  var source=new Vector3(0,.6f,0);var target=new Vector3(0,2.3f,3.7f);var rotation=Quaternion.Euler(27,-18,0);var focus=(source+target)*.5f;
  foreach(float aspect in new[]{390f/844,844f/390,667f/375,375f/667}){float distance=Kamilunavo.OneMoreFloor.CameraSystem.OrbitCamera.FitDistance(focus,rotation,58,aspect,source,target,source+Vector3.up*2.5f);var origin=focus-rotation*Vector3.forward*distance;for(int deck=0;deck<2;deck++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){var local=Quaternion.Inverse(rotation)*((deck==0?source:target)+new Vector3(x*2.4f,0,z*1.9f)-origin);float tangent=Mathf.Tan(29*Mathf.Deg2Rad);float vx=.5f+local.x/(local.z*tangent*aspect)*.5f,vy=.5f+local.y/(local.z*tangent)*.5f;Check(vx>=.079f&&vx<=.921f&&vy>=.229f&&vy<=.771f,"source and next deck fully outside HUD in every pose");}}
  float launch=.04f,elapsed=.76f+.33f,end=launch+elapsed;float exact=ElevatorRules.ArrivalClock(end,elapsed);Check(Math.Abs(exact-(launch+ElevatorRules.FlightSeconds))<.00001f,"uneven-frame exact arrival timestamp");Check(ElevatorRules.Hit(0,ElevatorRules.MotionX(15,2,260908,launch+ElevatorRules.FlightSeconds),ElevatorRules.HalfBay(15))==2,"advertised perfect regression fixture");Check(ElevatorRules.Hit(0,ElevatorRules.MotionX(15,2,260908,end),ElevatorRules.HalfBay(15))==0,"old overshoot negative control fails");Check(ElevatorRules.Hit(0,ElevatorRules.MotionX(15,2,260908,exact),ElevatorRules.HalfBay(15))==2,"hitch preserves advertised perfect");
  var decision=new FloorProfile();ElevatorRules.NewRun(decision);for(int f=1;f<=4;f++)ElevatorRules.Land(decision,f,true);var decisionReload=FloorSave.Parse(JsonUtility.ToJson(decision));ElevatorRules.Restore(decisionReload,DateTime.UtcNow);Check(decisionReload.CheckpointDecisionPending&&decisionReload.PendingCoins==decision.PendingCoins,"checkpoint decision and pending restore");
  Debug.Log("ELEVATOR_RULES_PASS checks="+checks);
 }
}
#endif
