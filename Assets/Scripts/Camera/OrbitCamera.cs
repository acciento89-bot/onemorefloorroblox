using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.CameraSystem
{
 // Historical component name retained; the approved game has no orbit input.
 public sealed class OrbitCamera:MonoBehaviour
 {
  public Transform Target;public FloorCourse Course;public float Distance=7.2f,Height=2.7f,Sensitivity=.15f;public float Yaw=>-18;
  private Camera _camera;
  private void Awake()=>_camera=GetComponent<Camera>();
  private void LateUpdate()=>FrameNow();
  public void FrameNow(){if(Target==null)return;if(_camera==null)_camera=GetComponent<Camera>();var rotation=Quaternion.Euler(27,-18,0);var source=Course!=null&&Course.Steps.Count>0?Course.Steps[Course.Height].transform.position+Vector3.up*.65f:Target.position;var next=Course!=null&&Course.Steps.Count>0?Course.Steps[Mathf.Min(29,Course.Height+1)].transform.position+Vector3.up*.65f:source;var focus=(source+next)*.5f+Vector3.up*.8f;var view=Course!=null?Course.GameplayViewport:new Rect(.08f,.23f,.84f,.54f);
   float distance=FitDistance(focus,rotation,_camera.fieldOfView,_camera.aspect,source,next,Target.position+Vector3.up*2.5f,view);float tangent=Mathf.Tan(_camera.fieldOfView*.5f*Mathf.Deg2Rad);var offset=rotation*Vector3.up*(distance*tangent*(1-2*view.center.y));transform.SetPositionAndRotation(focus+offset-rotation*Vector3.forward*distance,rotation);
  }
  public static float FitDistance(Vector3 focus,Quaternion rotation,float fov,float aspect,Vector3 source,Vector3 next,Vector3 head,Rect? viewport=null){var view=viewport??new Rect(.08f,.23f,.84f,.54f);float distance=8,tangent=Mathf.Tan(fov*.5f*Mathf.Deg2Rad);var inverse=Quaternion.Inverse(rotation);
   for(int deck=0;deck<2;deck++)for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)FitPoint(inverse*((deck==0?source:next)+new Vector3(x*2.4f,0,z*1.9f)-focus),tangent,aspect,view,ref distance);
   for(int deck=0;deck<2;deck++)for(int side=-1;side<=1;side+=2)FitPoint(inverse*((deck==0?source:next)+new Vector3(side*2.05f,3.35f,1.1f)-focus),tangent,aspect,view,ref distance);
   FitPoint(inverse*(head-focus),tangent,aspect,view,ref distance);FitPoint(inverse*((source+next)*.5f+Vector3.up*2.2f-focus),tangent,aspect,view,ref distance);return distance+.12f;
  }
  private static void FitPoint(Vector3 point,float tangent,float aspect,Rect view,ref float distance){float center=view.center.y,top=view.yMax-.015f,bottom=view.yMin+.015f;distance=Mathf.Max(distance,Mathf.Abs(point.x)/(tangent*Mathf.Max(.2f,aspect)*Mathf.Min(.5f-view.xMin,view.xMax-.5f)*2)-point.z);distance=Mathf.Max(distance,(point.y/tangent-(2*top-1)*point.z)/(2*(top-center)));distance=Mathf.Max(distance,(-point.y/tangent+(2*bottom-1)*point.z)/(2*(center-bottom)));}
 }
}
