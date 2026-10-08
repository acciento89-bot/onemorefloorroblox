#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
public static class FloorValidation
{
 static int checks;static void Check(bool yes,string name){checks++;if(!yes)throw new Exception(name);}
 public static void ValidateAll(){ValidateRules();}
 public static void ValidateRules(){
 var day=new DateTime(2026,10,8,0,0,0,DateTimeKind.Utc);var p=new FloorProfile();
 Check(FloorRules.ClaimDaily(p,day)&&p.Crystals==100,"daily first");Check(!FloorRules.ClaimDaily(p,day)&&!FloorRules.ClaimDaily(p,day.AddDays(-1)),"daily replay/time reversal");
 for(int i=1;i<9;i++)FloorRules.ClaimDaily(p,day.AddDays(i));Check(p.DailyStreak==7&&p.Crystals==1230,"UTC streak7 cap");
 p=new FloorProfile();Check(!FloorRules.Land(p,2,true),"skipped floor blocked");
 for(int i=1;i<=7;i++){Check(FloorRules.Land(p,i,true),"ordered floor");Check(!FloorRules.Land(p,i,true),"duplicate floor");}
 Check(p.Checkpoint==4&&p.RewardedFloor==7&&p.Step==7,"floor5 checkpoint");int coins=p.Crystals,score=p.Score,perfects=p.Perfects;
 FloorRules.Recover(p);Check(p.Step==4&&p.Falls==1,"immediate saved checkpoint");
 for(int i=5;i<=7;i++)Check(FloorRules.Land(p,i,true),"replay course advances");Check(p.Crystals==coins&&p.Score==score&&p.Perfects==perfects,"replay never mints score coins perfects");
 for(int i=8;i<=29;i++)Check(FloorRules.Land(p,i,true),"tower30");Check(p.Checkpoint==29&&p.Step==29&&p.Score==7250&&p.Crystals==133&&p.Perfects==29,"all30 floors rewards");
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
