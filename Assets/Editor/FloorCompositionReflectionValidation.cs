#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System.Reflection;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.CameraSystem;
using Kamilunavo.OneMoreFloor.UI;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorCompositionReflectionValidation
{
 public static void ValidateFloor()
 {
  var root=new GameObject("OriginalFloorRayCoverageFixture",typeof(RectTransform));
  try{
   var rect=(RectTransform)root.transform;
   foreach(var size in new[]{new Vector2(430,932),new Vector2(932,430)}){
    rect.sizeDelta=size;
    var pane=LiftScenePreview.Create(rect,0,true,0);var preview=pane.GetComponent<LiftScenePreview>();
    try{
     preview.PresentAtrium(rect);
     // Exercise the persisted shipping room, not only its procedural fallback.
     var original=preview.SceneRoot.transform.Find("GrandAtriumArchitecture");UnityEngine.Object.DestroyImmediate(original.gameObject);
     var set=Resources.Load<FloorAuthoredArtSet>("Models/ConceptArtSet");
     var room=UnityEngine.Object.Instantiate(set.Atrium,preview.SceneRoot.transform,false);
     var camera=preview.SceneRoot.GetComponentInChildren<Camera>();
     foreach(float x in new[]{.05f,.5f,.95f}){
      var ray=camera.ViewportPointToRay(new Vector3(x,.006f,0));
      if(!HitsMarble(room,ray,preview.SceneRoot.transform.position.y))
       throw new InvalidOperationException("FLOOR_COMPOSITION: actual shipping marble geometry does not cover lower lobby ray "+size+" x="+x);
     }
    }finally{UnityEngine.Object.DestroyImmediate(pane.gameObject);}
   }
   Debug.Log("FLOOR_LOBBY_RAY_PASS actual bottom viewport rays intersect original marble in both orientations");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
 }
 static bool HitsMarble(GameObject room,Ray ray,float originY)
 {
  foreach(var filter in room.GetComponentsInChildren<MeshFilter>()){
   var mesh=filter.sharedMesh;var vertices=mesh.vertices;var triangles=mesh.triangles;
   for(int i=0;i<triangles.Length;i+=3){
    var a=filter.transform.TransformPoint(vertices[triangles[i]]);var b=filter.transform.TransformPoint(vertices[triangles[i+1]]);var c=filter.transform.TransformPoint(vertices[triangles[i+2]]);
    if(a.y<originY+.14f||a.y>originY+.21f||Mathf.Abs(a.y-b.y)>.0001f||Mathf.Abs(a.y-c.y)>.0001f)continue;
    var edge=b-a;var second=c-a;var cross=Vector3.Cross(ray.direction,second);float det=Vector3.Dot(edge,cross);if(Mathf.Abs(det)<1e-8f)continue;
    var from=ray.origin-a;float u=Vector3.Dot(from,cross)/det;if(u<0||u>1)continue;var q=Vector3.Cross(from,edge);float v=Vector3.Dot(ray.direction,q)/det;
    if(v>=0&&u+v<=1&&Vector3.Dot(second,q)/det>0)return true;
   }
  }return false;
 }
 public static void ValidateReflection()
 {
  var cube=Resources.Load<Cubemap>("Rendering/CapturedAtriumReflection");
  if(cube==null)throw new InvalidOperationException("FLOOR_REFLECTION: original native scene reflection cube is absent from shipping Resources");
  if(cube.graphicsFormat!=GraphicsFormat.R8G8B8A8_UNorm||cube.width>128||cube.mipmapCount<5)throw new InvalidOperationException("FLOOR_REFLECTION: bounded filtered cubemap required");
  float minimum=float.PositiveInfinity,maximum=0;int samples=0;
  for(int face=0;face<6;face++){float faceMin=float.PositiveInfinity,faceMax=0;var pixels=cube.GetPixels((CubemapFace)face);if(pixels.Length!=128*128)throw new InvalidOperationException("Incomplete original capture face");foreach(var pixel in pixels){
   float value=(pixel.r+pixel.g+pixel.b)/3;
   if(!float.IsFinite(value))throw new InvalidOperationException("FLOOR_REFLECTION: nonfinite capture data");
   minimum=Mathf.Min(minimum,value);maximum=Mathf.Max(maximum,value);faceMin=Mathf.Min(faceMin,value);faceMax=Mathf.Max(faceMax,value);samples++;
  }
   if(faceMax-faceMin<.03f||faceMax<.04f)throw new InvalidOperationException("FLOOR_REFLECTION: blank/flat original capture face "+face);
  }
  if(samples<90000||maximum-minimum<.15f)throw new InvalidOperationException("FLOOR_REFLECTION: six complete nonflat original scene faces required");
  var root=new GameObject("ActualReflectionPaletteFixture");
  try{foreach(var surface in new[]{FloorArchitectureSurface.Brass,FloorArchitectureSurface.Glass}){
   var material=FloorArchitecturePalette.Get(root.transform,surface);
   if(material.GetTexture("_ReflectionCube")!=cube||material.GetFloat("_ReflectionStrength")<=0)
    throw new InvalidOperationException("FLOOR_REFLECTION: actual shipping palette does not consume original baked reflection "+surface);
  }}finally{UnityEngine.Object.DestroyImmediate(root);}
  Debug.Log("FLOOR_REFLECTION_ASSET_PASS six original scene faces / mipmaps / actual palette consumption");
 }
 public static void ValidateFraming()
 {
  var sunObject=GameObject.Find("Sun");var sun=sunObject!=null?sunObject.GetComponent<Light>():null;
  float intensity=sun!=null?sun.intensity:0;var rotation=sun!=null?sun.transform.rotation:Quaternion.identity;var colour=sun!=null?sun.color:Color.white;
  var sky=RenderSettings.skybox;bool textured=sky!=null&&sky.HasProperty("_MainTex");var texture=textured?sky.mainTexture:null;var fog=RenderSettings.fogColor;var ambient=RenderSettings.ambientSkyColor;
  var root=new GameObject("ActualOrthographicCabinFixture");var eye=new GameObject("ActualFrameTowerEye",typeof(Camera),typeof(OrbitCamera));
  try{
   var course=root.AddComponent<FloorCourse>();var profile=new FloorProfile();typeof(FloorCourse).GetField("<Profile>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(course,profile);
   course.Player=new GameObject("ActualActorEnvelope").transform;course.Player.SetParent(root.transform,false);
   typeof(FloorCourse).GetMethod("BuildRoute",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(course,null);
   var frame=eye.GetComponent<OrbitCamera>();frame.Target=course.Player;frame.Course=course;var camera=eye.GetComponent<Camera>();
   typeof(OrbitCamera).GetField("_camera",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(frame,camera);
   var method=typeof(OrbitCamera).GetMethod("FrameTower",BindingFlags.Instance|BindingFlags.NonPublic);int samples=0;
   foreach(float aspect in new[]{430f/932,375f/667,667f/375,932f/430})foreach(var viewport in new[]{new Rect(.08f,.23f,.84f,.54f),new Rect(.08f,.58f,.84f,.19f)})
   foreach(int floor in new[]{0,4,14,28})foreach(float arc in new[]{0f,.25f,.5f,.75f,1f}){
    profile.Step=floor;course.Cabin.position=Vector3.up*(floor*TowerLiftMotion.FloorSpacing+.25f);var source=course.SourcePosition;var destination=course.TargetPosition;
    course.Player.position=Vector3.Lerp(source,destination,arc)+Vector3.up*Mathf.Sin(arc*Mathf.PI)*TowerLiftMotion.JumpArc;camera.aspect=aspect;method.Invoke(frame,new object[]{source,destination,viewport});
    void Visible(Vector3 point){var p=camera.WorldToViewportPoint(point);if(p.z<=0||!viewport.Contains(new Vector2(p.x,p.y)))throw new InvalidOperationException("Actual orthographic transfer bounds clipped aspect="+aspect+" floor="+floor+" arc="+arc+" point="+p);}
    foreach(var center in new[]{source,destination})for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)Visible(center+new Vector3(x*(center==source?1.3f:1.4f),0,z<0?-1.55f:center==source?1.55f:1.2f));
    Visible(course.Player.position);Visible(course.Player.position+Vector3.up*2.75f);Visible(source+Vector3.up*3.15f);Visible(destination+Vector3.up*3.6f);
    var silhouette=course.Cabin.GetComponent<CabinFramingBounds>();foreach(float x in new[]{silhouette.Minimum.x,silhouette.Maximum.x})foreach(float y in new[]{silhouette.Minimum.y,silhouette.Maximum.y})Visible(source+OrbitCamera.FixedRotation*new Vector3(x,y,0));
    if(!camera.orthographic||Quaternion.Angle(camera.transform.rotation,OrbitCamera.FixedRotation)>.001f)throw new InvalidOperationException("Fixed orthographic authority changed");samples++;
   }
   Debug.Log("FLOOR_ACTUAL_ORTHOGRAPHIC_PASS ride/air/landing envelopes="+samples+" original pre-batch silhouette; functional proof only");
  }finally{UnityEngine.Object.DestroyImmediate(eye);UnityEngine.Object.DestroyImmediate(root);if(sun!=null){sun.intensity=intensity;sun.transform.rotation=rotation;sun.color=colour;}if(textured)sky.mainTexture=texture;RenderSettings.fogColor=fog;RenderSettings.ambientSkyColor=ambient;}
 }
 public static void Validate(){ValidateFloor();ValidateReflection();ValidateFraming();}
}
#endif
