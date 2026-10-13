using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.CameraSystem
{
 // Historical component name retained; the approved game has no orbit input.
 public sealed class OrbitCamera:MonoBehaviour
 {
  public const float FixedPitch=12,FixedYaw=-18;
  public static Quaternion FixedRotation=>Quaternion.Euler(FixedPitch,FixedYaw,0);
  public Transform Target;public FloorCourse Course;public float Distance=7.2f,Height=2.7f,Sensitivity=.15f;public float Yaw=>FixedYaw;
  private Camera _camera;
  Transform _framingCabin;Vector2 _cabinMin,_cabinMax;
  // Cache the actual silhouette once per rebuilt cabin; no mesh reads in LateUpdate.
  void CacheCabin(){if(_framingCabin==Course.Cabin)return;_framingCabin=Course.Cabin;var bounds=_framingCabin.GetComponent<CabinFramingBounds>();_cabinMin=bounds.Minimum;_cabinMax=bounds.Maximum;}
  private void Awake()=>_camera=GetComponent<Camera>();
  private void LateUpdate()=>FrameNow();
  public void FrameNow(){if(Target==null)return;if(_camera==null)_camera=GetComponent<Camera>();var rotation=FixedRotation;var source=Course!=null&&Course.Steps.Count>0?Course.SourcePosition:Target.position;var next=Course!=null&&Course.Steps.Count>0?Course.TargetPosition:source;var view=Course!=null?Course.GameplayViewport:new Rect(.08f,.23f,.84f,.54f);
   if(Course!=null&&Course.Cabin!=null){FrameTower(source,next,view);return;}_camera.orthographic=false;
   var focus=BestFocus(rotation,_camera.fieldOfView,_camera.aspect,source,next,Target.position+Vector3.up*2.5f,view);float distance=FitDistance(focus,rotation,_camera.fieldOfView,_camera.aspect,source,next,Target.position+Vector3.up*2.5f,view);float tangent=Mathf.Tan(_camera.fieldOfView*.5f*Mathf.Deg2Rad);var offset=rotation*Vector3.up*(distance*tangent*(1-2*view.center.y));transform.SetPositionAndRotation(focus+offset-rotation*Vector3.forward*distance,rotation);
  }
  void FrameTower(Vector3 source,Vector3 next,Rect view){var rotation=FixedRotation;var inverse=Quaternion.Inverse(rotation);var origin=(source+next)*.5f;float minX=float.PositiveInfinity,maxX=float.NegativeInfinity,minY=float.PositiveInfinity,maxY=float.NegativeInfinity;
   void Include(Vector3 point){var p=inverse*(point-origin);minX=Mathf.Min(minX,p.x);maxX=Mathf.Max(maxX,p.x);minY=Mathf.Min(minY,p.y);maxY=Mathf.Max(maxY,p.y);}
   CacheCabin();var cabinOffset=inverse*(source-origin);minX=_cabinMin.x+cabinOffset.x;maxX=_cabinMax.x+cabinOffset.x;minY=_cabinMin.y+cabinOffset.y;maxY=_cabinMax.y+cabinOffset.y;
   for(int kit=0;kit<2;kit++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Include((kit==0?source:next)+new Vector3(x*(kit==0?1.3f:1.4f),0,z<0?-1.55f:kit==0?1.55f:1.2f));
   Include(source+Vector3.up*3.15f);Include(next+Vector3.up*3.6f);
   Include(Target.position);Include(Target.position+Vector3.up*2.75f);float aspect=Mathf.Max(.2f,_camera.aspect),size=Mathf.Max(3,Mathf.Max((maxY-minY)/(2*Mathf.Max(.1f,view.height-.03f)),(maxX-minX)/(2*aspect*Mathf.Max(.1f,view.width-.03f))))+.12f;
   var focus=origin+rotation*new Vector3((minX+maxX)*.5f+(1-2*view.center.x)*size*aspect,(minY+maxY)*.5f+(1-2*view.center.y)*size,0);_camera.orthographic=true;_camera.orthographicSize=size;transform.SetPositionAndRotation(focus-rotation*Vector3.forward*24,rotation);
  }
  // Balance the perspective support planes, rather than only searching world height.
  // For each axis the feasible focus interval is [max(P-right*Z), min(P-left*Z)].
  // Its midpoint minimizes the necessary distance and retains every original bound.
  // One fixed 22-point pass uses no allocations or iterative zoom/rotation smoothing.
  public static Vector3 BestFocus(Quaternion rotation,float fov,float aspect,Vector3 source,Vector3 next,Vector3 head,Rect view){var midpoint=(source+next)*.5f+Vector3.up*.8f;var inverse=Quaternion.Inverse(rotation);float tangent=Mathf.Tan(fov*.5f*Mathf.Deg2Rad),horizontal=tangent*Mathf.Max(.2f,aspect)*Mathf.Min(.5f-view.xMin,view.xMax-.5f)*2;float bottom=(2*(view.yMin+.015f)-1)*tangent,top=(2*(view.yMax-.015f)-1)*tangent;var support=new Vector4(float.PositiveInfinity,float.NegativeInfinity,float.PositiveInfinity,float.NegativeInfinity);
   for(int deck=0;deck<2;deck++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Support(inverse*((deck==0?source:next)+new Vector3(x*2.4f,0,z*1.9f)-midpoint),horizontal,bottom,top,ref support);
   for(int deck=0;deck<2;deck++)for(int side=-1;side<=1;side+=2)Support(inverse*((deck==0?source:next)+new Vector3(side*1.18f,3.35f,1.1f)-midpoint),horizontal,bottom,top,ref support);
   for(int deck=0;deck<2;deck++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Support(inverse*((deck==0?source:next)+new Vector3(x*1.18f,-1.2f,z*1.1f)-midpoint),horizontal,bottom,top,ref support);
   Support(inverse*(head-midpoint),horizontal,bottom,top,ref support);Support(inverse*((source+next)*.5f+Vector3.up*2.2f-midpoint),horizontal,bottom,top,ref support);
   return midpoint+rotation*new Vector3((support.x+support.y)*.5f,(support.z+support.w)*.5f,0);
  }
  private static void Support(Vector3 point,float horizontal,float bottom,float top,ref Vector4 support){support.x=Mathf.Min(support.x,point.x+horizontal*point.z);support.y=Mathf.Max(support.y,point.x-horizontal*point.z);support.z=Mathf.Min(support.z,point.y-bottom*point.z);support.w=Mathf.Max(support.w,point.y-top*point.z);}
  public static float FitDistance(Vector3 focus,Quaternion rotation,float fov,float aspect,Vector3 source,Vector3 next,Vector3 head,Rect? viewport=null){var view=viewport??new Rect(.08f,.23f,.84f,.54f);float distance=5.5f,tangent=Mathf.Tan(fov*.5f*Mathf.Deg2Rad);var inverse=Quaternion.Inverse(rotation);
   for(int deck=0;deck<2;deck++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)FitPoint(inverse*((deck==0?source:next)+new Vector3(x*2.4f,0,z*1.9f)-focus),tangent,aspect,view,ref distance);
   for(int deck=0;deck<2;deck++)for(int side=-1;side<=1;side+=2)FitPoint(inverse*((deck==0?source:next)+new Vector3(side*1.18f,3.35f,1.1f)-focus),tangent,aspect,view,ref distance);
   // The lower eye now protects the authored -.54m underbody as well as both deck tops.
   for(int deck=0;deck<2;deck++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)FitPoint(inverse*((deck==0?source:next)+new Vector3(x*1.18f,-1.2f,z*1.1f)-focus),tangent,aspect,view,ref distance);
   FitPoint(inverse*(head-focus),tangent,aspect,view,ref distance);FitPoint(inverse*((source+next)*.5f+Vector3.up*2.2f-focus),tangent,aspect,view,ref distance);return distance+.12f;
  }
  private static void FitPoint(Vector3 point,float tangent,float aspect,Rect view,ref float distance){float center=view.center.y,top=view.yMax-.015f,bottom=view.yMin+.015f;distance=Mathf.Max(distance,Mathf.Abs(point.x)/(tangent*Mathf.Max(.2f,aspect)*Mathf.Min(.5f-view.xMin,view.xMax-.5f)*2)-point.z);distance=Mathf.Max(distance,(point.y/tangent-(2*top-1)*point.z)/(2*(top-center)));distance=Mathf.Max(distance,(-point.y/tangent+(2*bottom-1)*point.z)/(2*(center-bottom)));}
 }
}
