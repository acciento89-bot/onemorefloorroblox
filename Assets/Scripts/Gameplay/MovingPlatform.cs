using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Gameplay
{
 public sealed class MovingPlatform:MonoBehaviour
 {
  public Vector3 Axis=Vector3.right;public float Distance=.7f,Speed=.8f;private Vector3 _origin;private float _clock;public Vector3 Delta{get;private set;}
  private void Awake(){_origin=transform.position;}
  public void Tick(float dt){var old=transform.position;_clock+=Mathf.Max(0,dt);transform.position=_origin+Axis*Mathf.Sin(_clock*Speed)*Distance;Delta=transform.position-old;}
 }
}
