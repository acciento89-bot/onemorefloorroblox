using System;
using System.Globalization;
namespace Kamilunavo.OneMoreFloor.Core
{
 public static class FloorRules
 {
  public static string Day(DateTime date)=>date.ToUniversalTime().ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
  public static bool ClaimDaily(FloorProfile p,DateTime now){string day=Day(now);if(string.CompareOrdinal(day,p.DailyDay)<=0)return false;
   bool successive=DateTime.TryParseExact(p.DailyDay,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.AssumeUniversal|DateTimeStyles.AdjustToUniversal,out var old)&&old.Date.AddDays(1)==now.ToUniversalTime().Date;
   p.DailyStreak=successive?Math.Min(7,p.DailyStreak+1):1;p.BestDailyStreak=Math.Max(p.BestDailyStreak,p.DailyStreak);p.DailyDay=day;p.Crystals+=100+10*(p.DailyStreak-1);return true;}
  public static bool SelectStyle(FloorProfile p,int style){if(style<0||style>=8)return false;if(style>=4){if(!p.Styles[style])return false;p.Style=style;return true;}int[] price={0,75,150,250};if(!p.Styles[style]){if(p.Crystals<price[style])return false;p.Crystals-=price[style];p.Styles[style]=true;}p.Style=style;return true;}
  public static bool Land(FloorProfile p,int step,bool perfect){if(p.Completed||step!=p.Step+1||step>29)return false;p.Step=step;if((step+1)%5==0)p.Checkpoint=step;
   if(step>p.RewardedFloor){p.RewardedFloor=step;p.Score+=100+10*step;p.BestFloor=Math.Max(p.BestFloor,step);p.BestScore=Math.Max(p.BestScore,p.Score);p.Crystals+=1+step/5+(perfect?1:0);if(perfect)p.Perfects++;p.BestPerfects=Math.Max(p.BestPerfects,p.Perfects);}return true;}
  public static void Recover(FloorProfile p){p.Step=p.Checkpoint;p.Falls++;}
  public static int Complete(FloorProfile p,DateTime now){if(p.Step!=29)return 0;int stars=p.Falls>0?1:p.Elapsed<=240?3:2;if(p.Completed)return stars;p.Completed=true;
   if(p.Challenge){string day=Day(now);if(p.RunDay==day&&p.ChallengeDay!=day&&p.Perfects>=16){p.ChallengeDay=day;p.Crystals+=75;}}
   else{p.Stars[p.Realm]=Math.Max(p.Stars[p.Realm],stars);p.UnlockedRealm=Math.Max(p.UnlockedRealm,Math.Min(2,p.Realm+1));}return stars;}
 }
}
