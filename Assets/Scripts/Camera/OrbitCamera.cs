using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.CameraSystem
{
 public sealed class OrbitCamera:MonoBehaviour
 {
  public Transform Target;public FloorCourse Course;public float Distance=7.2f,Height=2.7f,Sensitivity=.15f;public float Yaw=>-18;
  private Camera _camera;private bool initialized;
  private void Awake()=>_camera=GetComponent<Camera>();
  private void LateUpdate(){if(Target==null)return;var rotation=Quaternion.Euler(27,-18,0);transform.rotation=rotation;
   Vector3 source=Course!=null&&Course.Steps.Count>0?Course.Steps[Course.Height].transform.position:Target.position;Vector3 next=Course!=null&&Course.Steps.Count>0?Course.Steps[Mathf.Min(29,Course.Height+1)].transform.position:source;
   var focus=(source+next)*.5f+Vector3.up*.8f;float aspect=Mathf.Max(.25f,_camera.aspect);float vertical=Mathf.Tan(_camera.fieldOfView*.5f*Mathf.Deg2Rad);float distance=Mathf.Max(11,5.3f/(vertical*aspect));
   var desired=focus-rotation*Vector3.forward*distance;transform.position=initialized?Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-9*Time.unscaledDeltaTime)):desired;initialized=true;
  }
 }
}
