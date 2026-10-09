using System;
namespace Kamilunavo.OneMoreFloor.Core
{
 public static class ElevatorRules
 {
  public const float FlightSeconds=.78f;
  public static float HalfBay(int floor)=>floor<5?1.25f:floor<15?1.02f:.83f;
  public static int Hit(float runnerX,float deckX,float halfBay){if(float.IsNaN(runnerX)||float.IsNaN(deckX)||float.IsInfinity(runnerX)||float.IsInfinity(deckX)||halfBay<=0)return 0;float d=Math.Abs(runnerX-deckX);return d>halfBay?0:d<=halfBay*.24f?2:1;}
  public static float MotionX(int floor,int realm,int seed,float clock){if(floor==0||(floor+1)%5==0)return 0;double speed=(floor<5?.65:.95)+realm*.22+floor*.014,phase=(floor*.71+(seed%101)*.013);double wave=Math.Sin(clock*speed+phase);if(realm==1)wave=.78*wave+.22*Math.Sin(clock*speed*2+phase);if(realm==2)wave=Math.Sin(clock*speed+phase+.35*Math.Sin(clock*.8));return (float)(wave*(floor<5?2:3));}
  public static void Migrate(FloorProfile p){if(p.TimingVersion==1)return;p.TimingVersion=1;p.Step=p.Checkpoint;p.RewardedFloor=p.Step;p.PendingCoins=p.PerfectChain=p.RiskLevel=0;p.RunBanked=p.Completed;}
  public static void NewRun(FloorProfile p){p.TimingVersion=1;p.Step=p.Checkpoint=p.RewardedFloor=p.Score=p.Falls=p.Perfects=p.PendingCoins=p.PerfectChain=p.RiskLevel=0;p.RunBanked=p.Completed=false;p.Elapsed=0;}
  public static bool Land(FloorProfile p,int floor,bool perfect){if(p.RunBanked||p.Completed||floor!=p.Step+1||floor>29)return false;p.Step=floor;if((floor+1)%5==0)p.Checkpoint=floor;
   if(floor>p.RewardedFloor){p.RewardedFloor=floor;p.PerfectChain=perfect?p.PerfectChain+1:0;if(perfect)p.Perfects++;int bonus=(floor+1)%7==0?3:1;p.PendingCoins+=(2+floor/5+(perfect?2+Math.Min(4,p.PerfectChain/3):0))*bonus*(1+p.RiskLevel);p.Score+=100+10*floor+(perfect?50:0);p.BestFloor=Math.Max(p.BestFloor,floor);p.BestScore=Math.Max(p.BestScore,p.Score);p.BestPerfects=Math.Max(p.BestPerfects,p.Perfects);}return true;}
  public static void Recover(FloorProfile p){p.Step=p.Checkpoint;p.Falls++;p.PendingCoins=p.PerfectChain=p.RiskLevel=0;}
  public static int Bank(FloorProfile p){if(p.RunBanked||((p.Step+1)%5!=0))return 0;int amount=p.PendingCoins;p.Crystals=Math.Min(100000000,p.Crystals+amount);p.PendingCoins=0;p.RunBanked=true;return amount;}
 }
}
