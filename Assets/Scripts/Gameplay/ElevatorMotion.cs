using UnityEngine;
using Kamilunavo.OneMoreFloor.Core;
namespace Kamilunavo.OneMoreFloor.Gameplay
{
 public sealed class ElevatorMotion:MonoBehaviour
 {
  public Transform Rail;public int Floor,Realm,Seed;public Vector3 Origin;public Vector3 At(float clock)=>Origin+Vector3.right*ElevatorRules.MotionX(Floor,Realm,Seed,clock);
  public void Tick(float clock){transform.position=At(clock);if(Rail!=null)Rail.localPosition=Vector3.right*(Origin.x-transform.position.x);}
 }
}
