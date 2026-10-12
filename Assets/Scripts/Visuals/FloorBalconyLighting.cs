using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Two bounded real room lamps follow occupied/next architecture. No per-floor
 // light explosion and no dependency from gameplay authority back into lighting.
 public sealed class FloorBalconyLighting:MonoBehaviour
 {
  FloorCourse _course;Light _occupied,_next;
  public void Initialize(FloorCourse course){_course=course;_occupied=Lamp("OccupiedBalconyLamp");_next=Lamp("NextBalconyLamp");}
  Light Lamp(string name){var l=new GameObject(name,typeof(Light)).GetComponent<Light>();l.transform.SetParent(transform,false);l.type=LightType.Point;l.range=3.8f;l.intensity=.72f;l.color=new Color(1,.68f,.30f);l.shadows=LightShadows.None;l.cullingMask=~(1<<31);l.renderMode=LightRenderMode.ForcePixel;l.enabled=false;return l;}
  void LateUpdate()
  {
   if(_course==null||_course.Profile==null||_course.Steps==null)return;
   Move(_occupied,_course.Height);Move(_next,_course.Height+1);
  }
  void Move(Light lamp,int index)
  {
   bool valid=index>=0&&index<_course.Steps.Count;lamp.enabled=valid;
   if(valid)lamp.transform.position=_course.Steps[index].transform.position+new Vector3(.70f,2.65f,1.30f);
  }
 }
}
