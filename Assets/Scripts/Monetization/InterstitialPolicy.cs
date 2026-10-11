using System;
using System.Collections.Generic;
using System.Globalization;
namespace Kamilunavo.OneMoreFloor.Monetization
{
 // Session-only pacing; a successful daily round is deduplicated by the saved UTC day.
 public sealed class InterstitialPolicy
 {
  readonly HashSet<string> rounds=new HashSet<string>(),days=new HashSet<string>();
  readonly double sessionStart;
  public const int MinimumRounds=3;
  public const double MinimumSeconds=180;
  public int SuccessfulRounds{get;private set;}
  public int RoundsSinceShown{get;private set;}
  public double LastShownAt{get;private set;}=double.NegativeInfinity;
  public InterstitialPolicy(double startedAt){sessionStart=Finite(startedAt)?Math.Max(0,startedAt):0;}
  public bool TryCountSuccess(string roundToken,bool tutorial,bool daily,string day,string previousDaily,Action<string> persistDaily)
  {
   if(tutorial||string.IsNullOrWhiteSpace(roundToken)||rounds.Contains(roundToken))return false;
   if(daily){
    if(!DateTime.TryParseExact(day,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out _)||days.Contains(day)||string.CompareOrdinal(day,previousDaily??"")<=0||persistDaily==null)return false;
    try{persistDaily(day);}catch{return false;}
    days.Add(day);
   }
   rounds.Add(roundToken);SuccessfulRounds++;RoundsSinceShown++;return true;
  }
  public bool Eligible(double now,bool removeAds,bool tutorial,bool resultVisible,bool inFlight,bool foreground,bool providerBusy,bool consent,bool ready)
  {
   return Finite(now)&&SuccessfulRounds>=MinimumRounds&&RoundsSinceShown>=MinimumRounds&&now-sessionStart>=MinimumSeconds&&now-LastShownAt>=MinimumSeconds
    &&!removeAds&&!tutorial&&resultVisible&&!inFlight&&foreground&&!providerBusy&&consent&&ready;
  }
  // Any opened optional fullscreen delays the next forced ad without consuming earned rounds.
  public void DelayAfterVoluntaryAd(double now){if(!Finite(now)||now<sessionStart||now<LastShownAt)return;LastShownAt=now;}
  public void RecordImpression(double now){if(!Finite(now)||now<sessionStart)return;LastShownAt=now;RoundsSinceShown=0;}
  static bool Finite(double value)=>!double.IsNaN(value)&&!double.IsInfinity(value);
 }
 // A single native surface may own the game's pause/audio/input lock at a time.
 public sealed class CommercePresentationLock
 {
  object owner;
  public bool IsLocked=>owner!=null;
  public bool TryAcquire(object candidate){if(candidate==null||owner!=null)return false;owner=candidate;return true;}
  public bool Release(object candidate){if(candidate==null||!ReferenceEquals(owner,candidate))return false;owner=null;return true;}
 }
}
