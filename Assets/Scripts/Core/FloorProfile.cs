using System;
using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Core
{
 [Serializable] public sealed class FloorProfile
 {
  public int Schema=1,Crystals,Realm,Step,Checkpoint,RewardedFloor,Score,BestFloor,BestScore,UnlockedRealm,Style,Falls,Perfects,DailyStreak,BestDailyStreak,BestPerfects;
  public int TimingVersion,PendingCoins,PerfectChain,RiskLevel;public bool RunBanked,TimingInFlight,CheckpointDecisionPending;
  public float Elapsed;public bool Completed,Challenge,Sound=true,Haptics=true,ReducedMotion,HighContrast;
  public string Language="de",DailyDay="",ChallengeDay="",RunDay="";
  public Monetization.CommerceProfile Commerce=new();public Monetization.RewardProfile Rewards=new();
  public bool[] Styles={true,false,false,false,false,false,false,false};public int[] Stars=new int[3];
  public void Normalize(){Crystals=Mathf.Clamp(Crystals,0,100000000);Score=Mathf.Clamp(Score,0,100000000);BestScore=Mathf.Clamp(Math.Max(Score,BestScore),0,100000000);BestFloor=Mathf.Clamp(Math.Max(BestFloor,RewardedFloor),0,29);UnlockedRealm=Mathf.Clamp(UnlockedRealm,0,2);Realm=Mathf.Clamp(Realm,0,UnlockedRealm);Step=Mathf.Clamp(Step,0,29);RewardedFloor=Mathf.Clamp(RewardedFloor,Step,29);Checkpoint=Mathf.Clamp(Checkpoint,0,Step);Checkpoint=Checkpoint<4?0:4+5*((Checkpoint-4)/5);Falls=Mathf.Max(0,Falls);Perfects=Mathf.Clamp(Perfects,0,29);Elapsed=float.IsNaN(Elapsed)||float.IsInfinity(Elapsed)?0:Mathf.Clamp(Elapsed,0,86400);BestDailyStreak=Mathf.Clamp(BestDailyStreak,0,7);BestPerfects=Mathf.Clamp(BestPerfects,0,29);DailyStreak=Mathf.Clamp(DailyStreak,0,7);
   if(Styles==null||Styles.Length!=8){var old=Styles;Styles=new bool[8];if(old!=null)Array.Copy(old,Styles,Math.Min(old.Length,8));}Styles[0]=true;
   if(Stars==null||Stars.Length!=3){var old=Stars;Stars=new int[3];if(old!=null)Array.Copy(old,Stars,Math.Min(old.Length,3));}for(int i=0;i<3;i++)Stars[i]=Mathf.Clamp(Stars[i],0,3);
   TimingVersion=Mathf.Clamp(TimingVersion,0,1);PendingCoins=Mathf.Clamp(PendingCoins,0,1000000);PerfectChain=Mathf.Clamp(PerfectChain,0,29);RiskLevel=Mathf.Clamp(RiskLevel,0,5);if(RunBanked)PendingCoins=0;
   Monetization.CommerceRules.Normalize(this);Style=Mathf.Clamp(Style,0,7);if(!Styles[Style])Style=0;Language=Language=="en"?"en":"de";DailyDay??="";ChallengeDay??="";RunDay??="";Completed=Completed&&Step==29;
  }
 }
}
