using UnityEngine;
namespace Kamilunavo.OneMoreFloor.CameraSystem
{
 // Fixed-view silhouette captured from original vertices before Unity static
 // batching replaces child meshes with shared root-space buffers.
 public sealed class CabinFramingBounds:MonoBehaviour
 {
  public Vector2 Minimum{get;private set;}public Vector2 Maximum{get;private set;}
  public static void Capture(GameObject root)
  {
   var bounds=root.AddComponent<CabinFramingBounds>();var minimum=Vector2.one*float.PositiveInfinity;var maximum=Vector2.one*float.NegativeInfinity;
   var inverse=Quaternion.Inverse(OrbitCamera.FixedRotation);var source=root.transform.position+Vector3.up*.65f;
   foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true)){
    if(filter.sharedMesh==null)continue;foreach(var vertex in filter.sharedMesh.vertices){var p=inverse*(filter.transform.TransformPoint(vertex)-source);minimum=Vector2.Min(minimum,new Vector2(p.x,p.y));maximum=Vector2.Max(maximum,new Vector2(p.x,p.y));}
   }
   bounds.Minimum=minimum;bounds.Maximum=maximum;
  }
 }
}
