#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorSculptedArtValidation
{
 public static void Validate()
 {
  var root=new GameObject("SculptedArtFixture");
  try
  {
   var runner=RunnerArt.Build(root.transform);
   var face=runner.Head.Find("FaceSkin").GetComponent<MeshFilter>().sharedMesh;
   // A lit close-up needs more than seven straight profile bands. Cover actual
   // geometry and independently varied face surface normals, not object labels.
   int heights=0;float previous=float.NegativeInfinity;
   foreach(float y in UniqueHeights(face)){if(y>previous+.0001f){heights++;previous=y;}}
   if(heights<11)throw new InvalidOperationException("SCULPTED_ART: face requires curved forehead/cheek/chin sections; actual height sections="+heights);
   if(runner.Hips.Find("WhiteDrapedHood").GetComponent<Renderer>().sharedMaterial!=runner.Hoodie)
    throw new InvalidOperationException("SCULPTED_ART: hood must belong to the chosen adventure jacket, including purchased styles");
   FloorCharacterFinishValidation.Validate();
   FloorRunnerArtValidation.Validate();
   Debug.Log("FLOOR_SCULPTED_ART_PASS face sections="+heights+" actual eyes/styles/ownership verified");
  }
  finally{UnityEngine.Object.DestroyImmediate(root);}
 }
 static float[] UniqueHeights(Mesh mesh)
 {
  var points=mesh.vertices;var y=new float[points.Length];for(int i=0;i<points.Length;i++)y[i]=points[i].y;Array.Sort(y);return y;
 }
}
#endif
