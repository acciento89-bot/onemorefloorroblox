#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
using Kamilunavo.OneMoreFloor.CameraSystem;
namespace Kamilunavo.OneMoreFloor.Editor
{
 // Geometry visibility proof uses the shipping camera fit and real render triangles.
 // Root must still compare actual player captures with the approved three-view board.
 public static class FloorCabinFidelityValidation
 {
  static int checks;
  static void Require(bool value,string message){checks++;if(!value)throw new InvalidOperationException("Floor cabin fidelity: "+message);}
  public static void Validate()
  {
   checks=0;var failures=new List<string>();
   foreach(var gate in new Action[]{ValidateCabin,ValidateRunner})try{gate();}catch(InvalidOperationException error){failures.Add(error.Message);Debug.Log("FLOOR_FIDELITY_SUBGATE_FAIL "+error.Message);}
   if(failures.Count>0)throw new InvalidOperationException(string.Join("; ",failures));
   Debug.Log("FLOOR_CABIN_FIDELITY_PASS checks="+checks);
  }
  public static void ValidateCabin()
  {
   var host=new GameObject("CabinFidelityFixture");var meshes=new HashSet<Mesh>();var materials=new HashSet<Material>();var textures=new HashSet<Texture>();
   try{
    PlatformArt.Build(host.transform,0,0);var cabin=host.transform.Find("CopperLiftCabin");
    var crown=cabin.Find("CabinCrown").GetComponent<MeshFilter>().sharedMesh.bounds;
    Require(crown.size.x<=2.5f&&crown.size.x>=1.8f,"cabin crown is a slim passenger-scale metal drum instead of the four-metre mushroom");
    Require(crown.size.y<=.30f,"upper crown has a restrained horizontal brass profile");
    var roof=cabin.Find("CabinDome").GetComponent<MeshFilter>().sharedMesh.bounds;
    Require(roof.size.y<=.13f&&roof.size.x<=2.35f,"roof closure is shallow metal rather than a brown hemispherical cap");
    var baseDrum=cabin.Find("CabinBaseDrum").GetComponent<MeshFilter>().sharedMesh.bounds;
    Require(baseDrum.size.x<=2.5f&&baseDrum.size.y>=.75f&&baseDrum.size.y<=.95f,"lower carriage is narrow with a substantial shaped metal body");
    Require(baseDrum.max.y<=.60f&&baseDrum.min.y<=-.15f,"lower carriage depth stays below the authoritative landing deck");
    var diffuser=cabin.Find("CabinCeilingDiffuser");Require(diffuser!=null,"visible ceiling includes a broad warm luminous diffuser");
    var diffuserBounds=diffuser.GetComponent<MeshFilter>().sharedMesh.bounds;Require(diffuserBounds.size.x>=1.65f&&diffuserBounds.size.z>=1.15f,"ceiling glow occupies a real broad surface");
    var ceilingMaterial=diffuser.GetComponent<MeshRenderer>().sharedMaterial;var properties=new MaterialPropertyBlock();diffuser.GetComponent<Renderer>().GetPropertyBlock(properties);var emission=properties.GetColor("_EmissionColor");if(emission.maxColorComponent==0)emission=ceilingMaterial.GetColor("_EmissionColor");Require(emission.r>=.85f&&emission.g>.45f&&emission.b<emission.r*.6f,"ceiling emits a restrained warm light without new scene lights");
    var brass=cabin.Find("CabinCrown").GetComponent<MeshRenderer>().sharedMaterial;Require(brass.GetFloat("_Metallic")>=.60f&&brass.GetFloat("_Glossiness")>=.45f,"brass retains a reflective authored finish");
    Require(host.GetComponentsInChildren<Light>(true).Length==0,"ceiling lighting adds no mobile real-time light");
    var collisions=host.GetComponentsInChildren<Collider>(true);Require(collisions.Length==1&&collisions[0] is BoxCollider,"only the authoritative landing collider remains");var collision=(BoxCollider)collisions[0];Require(collision.center==new Vector3(0,.25f,0)&&collision.size==new Vector3(4.6f,.70f,3.6f),"exact original landing center and size remain");
    Require(host.transform.Find("ServiceInlay")!=null&&host.transform.Find("CarriageRoller")!=null&&host.transform.Find("LiftControlDisplay")!=null&&host.transform.Find("PrecisionBaySide")!=null,"named original inlays/carriage/arrival cues remain");
    var runner=RunnerArt.Build(host.transform);runner.transform.localPosition=Vector3.up*.65f;
    foreach(float aspect in new[]{540f/960,960f/540})foreach(float relativeX in new[]{-.95f,0,.95f}){
     runner.transform.localPosition=new Vector3(relativeX,.65f,0);var camera=GameCamera(aspect,runner.transform.position);var face=runner.Head.Find("FaceSkin").GetComponent<MeshFilter>().sharedMesh.bounds;int visible=0,total=0;
     for(int y=0;y<5;y++)for(int x=-1;x<=1;x++){
      var sample=runner.Head.TransformPoint(new Vector3(face.center.x+x*face.extents.x*.55f,Mathf.Lerp(face.min.y,face.max.y,(y+.5f)/5),face.center.z));total++;if(!RoofBlocks(cabin,camera,sample))visible++;
     }
     Require(visible==total,"fixed-camera head remains unobscured by the real canopy triangles aspect="+aspect+" offset="+relativeX+" visible="+visible+"/"+total);
    }
    int vertices=0,renderers=0;foreach(var filter in cabin.GetComponentsInChildren<MeshFilter>(true)){vertices+=filter.sharedMesh.vertexCount;ValidateMesh(filter.sharedMesh);}foreach(var renderer in cabin.GetComponentsInChildren<MeshRenderer>(true))if(renderer.enabled)renderers++;
    Require(vertices<8500&&renderers<=80,"cabin geometry/render budget remains bounded before existing static batching");
    Collect(host,meshes,materials,textures);UnityEngine.Object.DestroyImmediate(host);Disposed(meshes,materials,textures);
    Debug.Log("FLOOR_SLIM_CABIN_BUDGET vertices="+vertices+" rawRenderers="+renderers);
   }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);}
  }
  public static void ValidateRunner()
  {
   var host=new GameObject("AdventurerFidelityFixture");var meshes=new HashSet<Mesh>();var materials=new HashSet<Material>();var textures=new HashSet<Texture>();
   try{
    var runner=RunnerArt.Build(host.transform);runner.Style(0);var c=runner.Hoodie.color;
    Require(c.g>=.23f&&c.b>=.23f&&c.r<c.g*.45f,"approved default outfit reads as teal adventure clothing");
    var bundle=runner.Hips.Find("CargoWaist").GetComponent<MeshFilter>().sharedMesh;bool found=false;Bounds pack=default;int frontStrapVertices=0;
    foreach(var vertex in bundle.vertices){if(vertex.z<-.20f&&vertex.y>.12f){if(!found){pack=new Bounds(vertex,Vector3.zero);found=true;}else pack.Encapsulate(vertex);}if(vertex.z>.15f&&vertex.y>.30f&&vertex.y<.57f&&Mathf.Abs(vertex.x)>.07f)frontStrapVertices++;}
    Require(found&&pack.size.x>.29f&&pack.size.y>.39f&&pack.min.z<-.32f,"backpack has actual shoulder-to-waist volume, flap and side silhouette");
    Require(frontStrapVertices>30,"brown shoulder straps extend over the visible teal chest");
    Require(runner.Hips.Find("WhiteDrapedHood")!=null&&runner.LeftArm.Find("LeftElbow")!=null&&runner.LeftLeg.Find("LeftKnee")!=null,"white hood and articulated public animation bindings remain");
    Require(host.GetComponentsInChildren<Collider>(true).Length==0,"runner art never changes controller physics");
    int vertices=0;foreach(var filter in host.GetComponentsInChildren<MeshFilter>(true)){vertices+=filter.sharedMesh.vertexCount;ValidateMesh(filter.sharedMesh);}Require(vertices<10000,"backpack stays within the original character geometry budget");
    var colors=new HashSet<string>();for(int style=0;style<8;style++){runner.Style(style);Require(colors.Add(runner.Hoodie.color.ToString("R")),"eight existing style IDs remain visually distinct");int active=0;foreach(var renderer in host.GetComponentsInChildren<MeshRenderer>(true))if(renderer.gameObject.activeInHierarchy&&renderer.enabled)active++;Require(active<=32,"backpack reuses existing character batches including premium crests");for(int crest=4;crest<8;crest++){var node=runner.Hips.Find("PremiumCrest"+crest);Require(node.gameObject.activeSelf==(style==crest),"premium crest selection is retained");Require(node.localPosition.z<pack.min.z,"premium embroidery remains visible on the outside of the pack");}}
    Collect(host,meshes,materials,textures);Require(materials.Count<=8,"backpack reuses the existing eight owned materials");UnityEngine.Object.DestroyImmediate(host);Disposed(meshes,materials,textures);
    Debug.Log("FLOOR_ADVENTURE_RUNNER_BUDGET vertices="+vertices+" materials="+materials.Count);
   }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);}
  }
  static Vector3 GameCamera(float aspect,Vector3 player)
  {
   var source=Vector3.up*.65f;var next=new Vector3(0,2.35f,3.7f);var rotation=OrbitCamera.FixedRotation;var view=new Rect(.08f,.23f,.84f,.54f);const float fov=58;var focus=OrbitCamera.BestFocus(rotation,fov,aspect,source,next,player+Vector3.up*2.5f,view);
   float distance=OrbitCamera.FitDistance(focus,rotation,fov,aspect,source,next,player+Vector3.up*2.5f,view);float tangent=Mathf.Tan(fov*.5f*Mathf.Deg2Rad);return focus+rotation*Vector3.up*(distance*tangent*(1-2*view.center.y))-rotation*Vector3.forward*distance;
  }
  static bool RoofBlocks(Transform cabin,Vector3 eye,Vector3 point)
  {
   Vector3 direction=(point-eye).normalized;float limit=Vector3.Distance(point,eye);
   foreach(var filter in cabin.GetComponentsInChildren<MeshFilter>(true)){
    if(filter.transform.TransformPoint(filter.sharedMesh.bounds.center).y<3.1f)continue;var vertices=filter.sharedMesh.vertices;var indices=filter.sharedMesh.triangles;
    for(int i=0;i<indices.Length;i+=3){var a=filter.transform.TransformPoint(vertices[indices[i]]);var b=filter.transform.TransformPoint(vertices[indices[i+1]]);var c=filter.transform.TransformPoint(vertices[indices[i+2]]);var edge=b-a;var cross=Vector3.Cross(direction,c-a);float determinant=Vector3.Dot(edge,cross);if(Mathf.Abs(determinant)<1e-7f)continue;float inverse=1/determinant;var offset=eye-a;float u=Vector3.Dot(offset,cross)*inverse;if(u<0||u>1)continue;var q=Vector3.Cross(offset,edge);float v=Vector3.Dot(direction,q)*inverse;if(v<0||u+v>1)continue;float t=Vector3.Dot(c-a,q)*inverse;if(t>.0001f&&t<limit-.0001f)return true;}
   }return false;
  }
  static void ValidateMesh(Mesh mesh){var v=mesh.vertices;foreach(var p in v)Require(float.IsFinite(p.x)&&float.IsFinite(p.y)&&float.IsFinite(p.z),"finite art coordinates");foreach(var n in mesh.normals)Require(float.IsFinite(n.x)&&float.IsFinite(n.y)&&float.IsFinite(n.z)&&n.sqrMagnitude>.5f,"valid art normals");var triangles=mesh.triangles;for(int i=0;i<triangles.Length;i+=3){Require(triangles[i]>=0&&triangles[i+1]>=0&&triangles[i+2]>=0&&triangles[i]<v.Length&&triangles[i+1]<v.Length&&triangles[i+2]<v.Length,"bounded triangle indices");Require(Vector3.Cross(v[triangles[i+1]]-v[triangles[i]],v[triangles[i+2]]-v[triangles[i]]).sqrMagnitude>1e-12f,"no degenerate art triangles");}}
  static void Collect(GameObject host,HashSet<Mesh> meshes,HashSet<Material> materials,HashSet<Texture> textures){foreach(var filter in host.GetComponentsInChildren<MeshFilter>(true))meshes.Add(filter.sharedMesh);foreach(var renderer in host.GetComponentsInChildren<MeshRenderer>(true))foreach(var material in renderer.sharedMaterials)if(material.name.StartsWith("FloorArchitecture",StringComparison.Ordinal)||material.name.StartsWith("LiftRunner",StringComparison.Ordinal)||material.name.StartsWith("Runner",StringComparison.Ordinal)){materials.Add(material);if(material.mainTexture!=null)textures.Add(material.mainTexture);}}
  static void Disposed(HashSet<Mesh> meshes,HashSet<Material> materials,HashSet<Texture> textures){foreach(var mesh in meshes)Require(mesh==null,"owned generated mesh released");foreach(var material in materials)Require(material==null,"owned generated material released");foreach(var texture in textures)Require(texture==null,"owned generated detail texture released");}
 }
}
#endif
