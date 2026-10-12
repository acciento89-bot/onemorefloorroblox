#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.CameraSystem;

// Projection is a necessary framing proof; screenshots must still establish visual quality.
public static class FloorCameraFramingValidation
{
 static int checks;
 static void Check(bool value,string label){checks++;if(!value)throw new InvalidOperationException("FLOOR_CAMERA: "+label);}
 public static void Validate()
 {
  checks=0;var rotation=OrbitCamera.FixedRotation;
  var source=Vector3.up*.65f;var next=new Vector3(0,2.35f,3.7f);var view=new Rect(.08f,.23f,.84f,.54f);
  foreach(float aspect in new[]{390f/844,375f/667}){
   var eye=Eye(rotation,58,aspect,source,next,source+Vector3.up*2.5f,view);
   float height=Project(source+Vector3.up*2.15f,eye,rotation,58,aspect).y-Project(source,eye,rotation,58,aspect).y;
   Debug.Log("FLOOR_CAMERA_PORTRAIT_ACTOR aspect="+aspect+" normalizedHeight="+height);
   Check(height>=.125f,"portrait runner occupies at least one eighth of screen height instead of the previous distant miniature");
  }
  Check(OrbitCamera.FixedPitch<=16&&OrbitCamera.FixedPitch>=8&&OrbitCamera.FixedYaw==-18,"lower fixed cabin view retains the existing yaw and never becomes orbit input");
  var poses=new[]{new Vector2(390f/844,.23f),new Vector2(375f/667,.23f),new Vector2(844f/390,.23f),new Vector2(667f/375,.43f),new Vector2(390f/844,.58f)};
  for(int realm=0;realm<3;realm++)foreach(int floor in new[]{0,4,14,28,29})foreach(float clock in new[]{0f,1.3f,5.7f,11.9f})foreach(var pose in poses){
   int target=Mathf.Min(29,floor+1);source=new Vector3(ElevatorRules.MotionX(floor,realm,260908,clock),floor*1.7f+.65f,floor*3.7f);next=new Vector3(ElevatorRules.MotionX(target,realm,260908,clock),target*1.7f+.65f,target*3.7f);
   view=new Rect(.08f,pose.y,.84f,.77f-pose.y);
   foreach(float transfer in new[]{0f,.25f,.5f,.75f,1f}){
    // Exact guided-flight formula. Destination is the fixed arrival line, not the moving bay.
    var destination=new Vector3(0,next.y,next.z);var start=source+Vector3.right*.9f;
    var actor=target==floor?start:Vector3.Lerp(start,destination,transfer)+Vector3.up*Mathf.Sin(transfer*Mathf.PI)*1.8f;
    var head=actor+Vector3.up*2.5f;var eye=Eye(rotation,58,pose.x,source,next,head,view);
    foreach(var center in new[]{source,next}){
     for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Visible(center+new Vector3(x*2.4f,0,z*1.9f),eye,rotation,58,pose.x,view,"complete authoritative landing footprint");
     for(int side=-1;side<=1;side+=2)Visible(center+new Vector3(side*1.18f,3.35f,1.1f),eye,rotation,58,pose.x,view,"both full cabin crowns");
     for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Visible(center+new Vector3(x*1.18f,-1.2f,z*1.1f),eye,rotation,58,pose.x,view,"authored carriage and lower underbody");
    }
    Visible(head,eye,rotation,58,pose.x,view,"guided actor head");Visible(actor,eye,rotation,58,pose.x,view,"guided actor feet");
    Visible((source+next)*.5f+Vector3.up*2.2f,eye,rotation,58,pose.x,view,"original fit arc envelope");
    Visible((source-Vector3.up*.65f+destination)*.5f+Vector3.up*2.2f,eye,rotation,58,pose.x,view,"actual course arc toward stationary arrival line");
    var focus=OrbitCamera.BestFocus(rotation,58,pose.x,source,next,head,view);
    Check(OrbitCamera.FitDistance(focus,rotation,58,pose.x,source,next,head,view)<=OrbitCamera.FitDistance((source+next)*.5f+Vector3.up*.8f,rotation,58,pose.x,source,next,head,view)+.0001f,"balanced perspective fit never retreats beyond the original focus with the same protected bounds");
   }
  }
  Debug.Log("FLOOR_CAMERA_FRAMING_PASS checks="+checks+" actualRenderedConceptReview=PENDING");
 }
 static Vector3 Eye(Quaternion rotation,float fov,float aspect,Vector3 source,Vector3 next,Vector3 head,Rect view){var focus=OrbitCamera.BestFocus(rotation,fov,aspect,source,next,head,view);float distance=OrbitCamera.FitDistance(focus,rotation,fov,aspect,source,next,head,view);return focus+rotation*Vector3.up*(distance*Mathf.Tan(fov*.5f*Mathf.Deg2Rad)*(1-2*view.center.y))-rotation*Vector3.forward*distance;}
 static Vector3 Project(Vector3 point,Vector3 eye,Quaternion rotation,float fov,float aspect){var local=Quaternion.Inverse(rotation)*(point-eye);float tangent=Mathf.Tan(fov*.5f*Mathf.Deg2Rad);return new Vector3(.5f+local.x/(local.z*tangent*aspect)*.5f,.5f+local.y/(local.z*tangent)*.5f,local.z);}
 static void Visible(Vector3 point,Vector3 eye,Quaternion rotation,float fov,float aspect,Rect view,string label){var projected=Project(point,eye,rotation,fov,aspect);Check(projected.z>.25f&&projected.x>=view.xMin-.00001f&&projected.x<=view.xMax+.00001f&&projected.y>=view.yMin-.00001f&&projected.y<=view.yMax+.00001f,label+" remains in its existing reserved viewport");}
}
#endif
