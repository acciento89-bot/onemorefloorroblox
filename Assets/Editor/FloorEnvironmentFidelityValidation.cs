#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorEnvironmentFidelityValidation
{
 static void Check(bool yes,string message){if(!yes)throw new InvalidOperationException("ENVIRONMENT_FIDELITY: "+message);}
 public static void DiagnoseShader(){var shader=Resources.Load<Shader>("FloorMasonry");var material=new Material(shader);try{UnityEditor.ShaderUtil.CompilePass(material,0,true);foreach(var message in UnityEditor.ShaderUtil.GetShaderMessages(shader))Debug.Log("FLOOR_MASONRY_DIAGNOSTIC line="+message.line+" "+message.message);}finally{UnityEngine.Object.DestroyImmediate(material);}}
 public static void ValidateSupport(){var root=new GameObject("ExitSupportFixture");try{PlatformArt.Build(root.transform,0,0);for(int i=0;i<=20;i++){float t=Kamilunavo.OneMoreFloor.Gameplay.TowerLiftMotion.ExitFraction*i/20f;var p=Quaternion.Inverse(Quaternion.Euler(0,-90,0))*new Vector3(Kamilunavo.OneMoreFloor.Gameplay.TowerLiftMotion.TowerX*t,0,Kamilunavo.OneMoreFloor.Gameplay.TowerLiftMotion.TowerZ*t);Check(root.GetComponentsInChildren<MeshFilter>().Any(f=>Blocks(f,p+Vector3.up*.64f,p+Vector3.up*.57f)),"actual departure and boarding walk stays on a visible tread until the unchanged jump phase t="+t);}Debug.Log("FLOOR_EXIT_SUPPORT_PASS");}finally{UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateBanner(){var root=new GameObject("BannerFacingFixture");try{LiftAtriumArt.Build(root.transform);var cloth=root.transform.Find("GrandAtriumArchitecture/AtriumLiftBanner").GetComponent<MeshFilter>().sharedMesh;Check(cloth.normals.All(n=>Vector3.Dot(n,Vector3.back)>.99f),"actual cloth faces the negative-Z atrium camera, rather than hiding behind its gold emblem");Debug.Log("FLOOR_ATRIUM_BANNER_PASS");}finally{UnityEngine.Object.DestroyImmediate(root);}}
 public static void Validate(){ValidateSupport();ValidateBanner();var root=new GameObject("EnvironmentFidelityFixture");try{
  LiftArchitecture.Bay(root.transform,0);var bay=root.transform.Find("ArchitecturalFloorBay");
  // Probe actual rendered triangles through the doorway, not labels or mesh names.
  foreach(float x in new[]{-.10f,0,.10f})foreach(float y in new[]{1.60f,2.15f}){
   var start=bay.TransformPoint(new Vector3(LiftArchitecture.FacadeOffset/LiftArchitecture.FacadeWidthScale+x,y,-.1f));var end=bay.TransformPoint(new Vector3(LiftArchitecture.FacadeOffset/LiftArchitecture.FacadeWidthScale+x,y,2.60f));
   Check(!bay.GetComponentsInChildren<MeshFilter>().Any(f=>Blocks(f,start,end)),"actual central entrance is open into its illuminated recess, rather than a closed flat board");
  }
  LiftArchitecture.Cabin(root.transform);LiftArchitecture.Rails(root.transform);
  var posts=root.GetComponentsInChildren<Renderer>().Where(r=>r.name=="CabinRearPost").ToArray();var guides=root.GetComponentsInChildren<Renderer>().Where(r=>r.name=="VerticalLiftRail").ToArray();Check(posts.Length==2&&guides.Length==2,"fixture contains both rear posts and real lift guides");
  foreach(var post in posts){var a=post.bounds.center;float closest=guides.Min(g=>Vector2.Distance(new Vector2(a.x,a.z),new Vector2(g.bounds.center.x,g.bounds.center.z)));Check(closest<.35f,"vertical guides mechanically meet the carriage instead of floating far beside it");}
  Check(root.GetComponentsInChildren<Collider>().Length==0,"environment presentation creates no collision authority");Debug.Log("FLOOR_ENVIRONMENT_FIDELITY_PASS actualConceptReview=REQUIRED");
 }finally{UnityEngine.Object.DestroyImmediate(root);}}
 static bool Blocks(MeshFilter filter,Vector3 begin,Vector3 end){var vertices=filter.sharedMesh.vertices;var indices=filter.sharedMesh.triangles;var ray=(end-begin).normalized;float distance=Vector3.Distance(begin,end);for(int i=0;i<indices.Length;i+=3){var a=filter.transform.TransformPoint(vertices[indices[i]]);var b=filter.transform.TransformPoint(vertices[indices[i+1]]);var c=filter.transform.TransformPoint(vertices[indices[i+2]]);var ab=b-a;var ac=c-a;var cross=Vector3.Cross(ray,ac);float det=Vector3.Dot(ab,cross);if(Mathf.Abs(det)<1e-8f)continue;float inv=1/det;var from=begin-a;float u=Vector3.Dot(from,cross)*inv;if(u<0||u>1)continue;var q=Vector3.Cross(from,ab);float v=Vector3.Dot(ray,q)*inv;if(v<0||u+v>1)continue;float t=Vector3.Dot(ac,q)*inv;if(t>.001f&&t<distance-.001f)return true;}return false;}
}
#endif
