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
  Debug.Log("ELEVATOR_RULES_PASS checks="+checks);
 }
}
#endif
