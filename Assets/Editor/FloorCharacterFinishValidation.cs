#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorCharacterFinishValidation
{
 static void Check(bool yes,string message){if(!yes)throw new InvalidOperationException("CHARACTER_FINISH: "+message);}
 public static void Validate(){var host=new GameObject("CharacterFinishFixture");try{
  var runner=RunnerArt.Build(host.transform);var face=runner.Head.Find("FaceSkin").GetComponent<MeshFilter>().sharedMesh;var points=face.vertices;var normals=face.normals;int seamPairs=0;
  for(int a=0;a<points.Length;a++)for(int b=a+1;b<points.Length;b++)if((points[a]-points[b]).sqrMagnitude<1e-10f){seamPairs++;Check(Vector3.Dot(normals[a],normals[b])>.999f,"coincident face UV-seam vertices have continuous lighting rather than a dark cheek stripe");}
  Check(seamPairs>5,"fixture actually covers face UV seams");
  int eyePoints=0;float protrusion=0;foreach(var name in new[]{"EyeWhites","EyesAndExpression"})foreach(var p in runner.Head.Find(name).GetComponent<MeshFilter>().sharedMesh.vertices){if(Mathf.Abs(p.x)<.04f||Mathf.Abs(p.x)>.13f||p.y<-.03f||p.y>.065f)continue;float front=FaceFront(face,p);if(!float.IsFinite(front))continue;eyePoints++;protrusion=Mathf.Max(protrusion,p.z-front);}
  Check(eyePoints>50,"fixture probes actual lenses against the actual face triangles");Check(protrusion<=.020f,"eyes follow the face surface instead of protruding as detached-looking balls: "+protrusion);
  foreach(var name in new[]{"FaceSkin","SweptChestnutHair"})Check(runner.Head.Find(name).GetComponent<Renderer>().sharedMaterial.shader.name=="Floor/RunnerSurface","skin and hair receive the authored soft character finish");
  Check(runner.Hips.Find("TailoredLiftHoodie").GetComponent<Renderer>().sharedMaterial.shader.name=="Floor/RunnerSurface","cloth uses the same bounded character surface kit");
  foreach(var elbow in new[]{runner.LeftElbow,runner.RightElbow}){var sleeve=elbow.Find("WristBand");var bounds=sleeve.GetComponent<MeshFilter>().sharedMesh.bounds;Check(bounds.max.y>.02f&&bounds.min.y<-.23f,"Concept hoodie sleeves cover the forearm down to the glove, rather than exposing rolled-shirt bare arms");Check(sleeve.GetComponent<Renderer>().sharedMaterial==runner.Hoodie,"forearm sleeves use the actual selected paid or earned hoodie colour");}
  Check(host.GetComponentsInChildren<Collider>().Length==0,"character presentation adds no movement authority");Debug.Log("FLOOR_CHARACTER_FINISH_PASS actualConceptReview=REQUIRED");
 }finally{UnityEngine.Object.DestroyImmediate(host);}}
 static float FaceFront(Mesh face,Vector3 p){var points=face.vertices;var triangles=face.triangles;float front=float.NegativeInfinity;for(int i=0;i<triangles.Length;i+=3){var a=points[triangles[i]];var b=points[triangles[i+1]];var c=points[triangles[i+2]];var ab=b-a;var ac=c-a;var ap=p-a;float det=ab.x*ac.y-ab.y*ac.x;if(Mathf.Abs(det)<1e-9f)continue;float u=(ap.x*ac.y-ap.y*ac.x)/det,v=(ab.x*ap.y-ab.y*ap.x)/det;if(u>=-.00001f&&v>=-.00001f&&u+v<=1.00001f)front=Mathf.Max(front,a.z+ab.z*u+ac.z*v);}return front;}
}
#endif
