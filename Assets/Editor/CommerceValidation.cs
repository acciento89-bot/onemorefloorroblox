#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Monetization;
using UnityEngine;
public static class CommerceValidation
{
 static int count;static void Check(bool ok,string label){count++;if(!ok)throw new Exception(label);}
 public static void Validate(){
 var p=new FloorProfile();p.Normalize();Check(p.Styles.Length==8&&p.Styles[0],"free style migration");
 Check(CommerceRules.FulfillPending(p,CommerceRules.Starter,"transaction1",_=>{}),"pending fulfillment");Check(p.Crystals==500&&p.Styles[4],"starter once");
 CommerceRules.FulfillPending(p,CommerceRules.Starter,"transaction1",_=>{});Check(p.Crystals==500,"transaction replay");
 CommerceRules.FulfillPending(p,CommerceRules.Starter,"transaction2",_=>{});Check(p.Crystals==500,"starter bonus never twice");
 p=new FloorProfile();p.Normalize();CommerceRules.RestoreEntitlement(p,CommerceRules.Starter);Check(p.Crystals==0&&p.Styles[4],"confirmed restoration never mints currency");
 CommerceRules.FulfillPending(p,CommerceRules.Starter,"afterrestore",_=>{});Check(p.Crystals==0,"restore plus delayed pending no mint");
 p=new FloorProfile();p.Normalize();try{CommerceRules.FulfillPending(p,CommerceRules.Starter,"failedsave",_=>throw new Exception("disk"));}catch(Exception){}Check(p.Crystals==0&&!p.Styles[4]&&p.Commerce.FulfilledTransactions.Count==0,"failed persistence rolls back atomic fulfillment");
 Check(!CommerceRules.FulfillPending(p,"unknown","x",_=>{})&&!CommerceRules.FulfillPending(p,CommerceRules.Starter,"",_=>{}),"unknown/missing ID rejected");
 CommerceRules.RestoreEntitlement(p,CommerceRules.Collection);p.Style=7;CommerceRules.ReconcileEntitlements(p,new HashSet<string>());Check(!p.Styles[7]&&p.Style==0,"revocation resets premium selection");
 Check(!FloorRules.SelectStyle(p,4),"premium cannot be bought with free currency");p.Crystals=100;Check(FloorRules.SelectStyle(p,1)&&p.Crystals==25,"earned style preserved");
 var now=new DateTime(2026,10,8,0,0,0,DateTimeKind.Utc);p=new FloorProfile();p.Normalize();
 Check(RewardRules.Fulfill(p,"view1",now,_=>{})&&p.Crystals==50,"completed video reward");Check(!RewardRules.Fulfill(p,"view1",now.AddMinutes(2),_=>{}),"video replay no reward");Check(!RewardRules.Fulfill(p,"cooldown",now.AddSeconds(10),_=>{}),"reward cooldown");
 for(int i=1;i<5;i++)Check(RewardRules.Fulfill(p,"view"+(i+1),now.AddMinutes(i*2),_=>{}),"daily reward slot");Check(!RewardRules.Fulfill(p,"overlimit",now.AddHours(1),_=>{}),"five videos per day");Check(RewardRules.Fulfill(p,"nextday",now.AddDays(1),_=>{}),"UTC day resets limit");
 int wallet=p.Crystals;try{RewardRules.Fulfill(p,"bad-save",now.AddDays(1).AddMinutes(2),_=>throw new Exception("disk"));}catch(Exception){}Check(p.Crystals==wallet&&!p.Rewards.Sessions.Contains("bad-save"),"reward persistence rollback");
 var generation=new AdLoadGeneration();var before=generation.Begin();generation.Invalidate();Check(!generation.IsCurrent(before),"privacy invalidates old loads");
 ValidateRecovery();Debug.Log("FLOOR_COMMERCE_PASS checks="+count);
 }
 static void ValidateRecovery(){var failures=new List<string>();
 var presentation=typeof(RewardedVideos).GetMethod("TryPresentConsent",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
 try{Check(presentation!=null,"delayed consent uses active-shop gate");var fixture=typeof(FloorValidation).GetMethod("Fixture",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);object[] args={null,null};var hud=(Kamilunavo.OneMoreFloor.UI.FloorHud)fixture.Invoke(null,args);var root=(GameObject)args[0];var canvas=(Canvas)args[1];try{hud.Course.Hud=hud;hud.Course.Store=root.AddComponent<StorePurchases>();hud.Course.Store.Initialize(hud.Course);var videos=root.AddComponent<RewardedVideos>();hud.Course.Videos=videos;videos.Initialize(hud.Course);typeof(RewardedVideos).GetField("_game",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(videos,hud.Course);int shown=0;bool opened=(bool)presentation.Invoke(videos,new object[]{(Action)(()=>shown++)});Check(!opened&&shown==0&&!videos.IsPresenting,"late consent after closed shop never presents");hud.ShowShop();opened=(bool)presentation.Invoke(videos,new object[]{(Action)(()=>shown++)});Check(opened&&shown==1&&videos.IsPresenting&&hud.Course.Paused,"active shop consent remains paused");hud.Close();Check(hud.ModalOpen,"native consent cannot unpause by closing shop");}finally{UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}catch(Exception e){failures.Add("CONSENT: "+e.Message);}
 try{var type=typeof(StorePurchases).Assembly.GetType("Kamilunavo.OneMoreFloor.Monetization.StoreReconnectGate");Check(type!=null,"store has bounded same-session reconnect gate");var gate=Activator.CreateInstance(type);var begin=type.GetMethod("TryBegin");var end=type.GetMethod("EndAttempt");var fail=type.GetMethod("Failed");Check((bool)begin.Invoke(gate,new object[]{0d}),"first connection starts");Check(!(bool)begin.Invoke(gate,new object[]{1d}),"duplicate connection rejected");fail.Invoke(gate,new object[]{1d});Check(!(bool)begin.Invoke(gate,new object[]{20d}),"failure does not release still-running connect");end.Invoke(gate,null);Check(!(bool)begin.Invoke(gate,new object[]{2d}),"retry cooldown bounded");Check((bool)begin.Invoke(gate,new object[]{7d}),"same-session retry after temporary failure");end.Invoke(gate,null);}catch(Exception e){failures.Add("STORE: "+e.Message);}
 if(failures.Count>0)throw new Exception(string.Join("; ",failures));
 }
}
#endif
