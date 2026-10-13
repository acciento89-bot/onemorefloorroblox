#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Kamilunavo.OneMoreFloor.Visuals;
namespace Kamilunavo.OneMoreFloor.QA
{
 // Pixel evidence from the actual URP player, isolated from the real profile.
 public static class FloorConceptRenderQa
 {
  static void Require(bool value,string message){if(!value)throw new InvalidOperationException("CONCEPT_GPU: "+message);}
  public static void Validate(string output)
  {
   var root=new GameObject("ConceptGpuProbe");root.transform.position=new Vector3(80000,0,80000);
   var previousSun=RenderSettings.sun;bool fog=RenderSettings.fog;var renderer=Resources.Load<UniversalRendererData>("Rendering/ConceptRenderer");var previousCopy=renderer.copyDepthMode;
   Texture2D read=null;RenderTexture target=null;Material material=null;Texture2D stripes=null;
   try
   {
    RenderSettings.fog=false;
    var camera=root.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=1;camera.nearClipPlane=.1f;camera.farClipPlane=260;camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;
    FloorConceptLighting.ConfigureCamera(camera);var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=false;
    target=new RenderTexture(256,256,24,RenderTextureFormat.ARGBHalf);target.Create();camera.targetTexture=target;camera.aspect=1;read=new Texture2D(256,256,TextureFormat.RGBA32,false,true);
    material=new Material(Resources.Load<Shader>("FloorSurface"));material.color=new Color(.4f,.4f,.4f);material.SetFloat("_Glossiness",0);material.SetColor("_EmissionColor",Color.black);
    GameObject Quad(string name,Vector3 position,float width,float height)
    {
     var mesh=new Mesh{vertices=new[]{new Vector3(-width/2,-height/2,0),new Vector3(width/2,-height/2,0),new Vector3(width/2,height/2,0),new Vector3(-width/2,height/2,0)},normals=new[]{Vector3.back,Vector3.back,Vector3.back,Vector3.back},uv=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},triangles=new[]{0,2,1,0,3,2}};
     var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root.transform,false);go.transform.localPosition=position;go.layer=31;go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<Renderer>().sharedMaterial=material;ArtLifetime.Own(go,mesh);return go;
    }
    void Render(string name=null)
    {
     FloorConceptLighting.RenderPreview(camera,target);var previous=RenderTexture.active;
     try{RenderTexture.active=target;read.ReadPixels(new Rect(0,0,256,256),0,0);read.Apply();}finally{RenderTexture.active=previous;}
     if(name!=null)File.WriteAllBytes(Path.Combine(output,name+".png"),read.EncodeToPNG());
    }
    var plane=Quad("LightProbe",new Vector3(0,0,8),1,1);
    var sunObject=new GameObject("ProbeSun",typeof(Light));sunObject.transform.SetParent(root.transform,false);var sun=sunObject.GetComponent<Light>();sun.type=LightType.Directional;sun.intensity=1;sun.shadows=LightShadows.None;sun.enabled=false;sun.cullingMask=1<<31;RenderSettings.sun=sun;
    var point=FloorConceptLighting.CabinLamp(root.transform);point.transform.localPosition=new Vector3(0,0,6);point.cullingMask=1<<31;point.enabled=false;
    Render();var baseline=read.GetPixel(128,128);
    sun.cullingMask=~(1<<31);sun.enabled=true;Render();var excluded=read.GetPixel(128,128);
    float maskDelta=Mathf.Max(Mathf.Abs(excluded.r-baseline.r),Mathf.Abs(excluded.g-baseline.g));Require(maskDelta<.015f,"excluded main-light layer must not add direct light");
    sun.cullingMask=1<<31;Render();var included=read.GetPixel(128,128);Require(included.r-baseline.r>.08f,"included main-light control really illuminates the probe");
    sun.enabled=false;point.enabled=true;Render("point-light-on");var warm=read.GetPixel(128,128);float warmR=warm.r-baseline.r,warmB=warm.b-baseline.b;
    Require(warmR>.03f&&warmR>warmB*1.35f,"real additional Point Light must add warm illumination to the lit surface");point.enabled=false;
    // Actual shipping materials, six captured faces and orthographic response;
    // a resource reference alone cannot prove the shader consumes the cube.
    float reflectionGain=0,glassGain=0;
    foreach(var surface in new[]{FloorArchitectureSurface.Brass,FloorArchitectureSurface.Glass}){
     var reflective=FloorArchitecturePalette.Get(root.transform,surface);Require(reflective.GetTexture("_ReflectionCube")!=null,"shipping reflection resource bound");
     plane.GetComponent<Renderer>().sharedMaterial=reflective;float strength=reflective.GetFloat("_ReflectionStrength");
     reflective.SetFloat("_ReflectionStrength",0);Render();var flat=read.GetPixel(128,128);
     reflective.SetFloat("_ReflectionStrength",strength);Render(surface==FloorArchitectureSurface.Brass?"original-brass-reflection":"original-glass-reflection");var captured=read.GetPixel(128,128);
     float delta=Mathf.Max(Mathf.Abs(captured.r-flat.r),Mathf.Abs(captured.g-flat.g),Mathf.Abs(captured.b-flat.b));Require(delta>.002f,"actual baked cube must visibly affect "+surface);
     if(surface==FloorArchitectureSurface.Brass)reflectionGain=delta;else glassGain=delta;
     // Orthographic view direction is invariant under matching object/eye translation.
     camera.transform.position+=Vector3.right*.6f;Render();var shifted=read.GetPixel(128,128);
     Require(Mathf.Max(Mathf.Abs(shifted.r-captured.r),Mathf.Abs(shifted.g-captured.g),Mathf.Abs(shifted.b-captured.b))<.01f,"orthographic reflection invariant for "+surface);
     camera.transform.position-=Vector3.right*.6f;
    }
    plane.GetComponent<Renderer>().sharedMaterial=material;
    plane.transform.localScale=new Vector3(.2f,.2f,1);material.color=Color.black;material.SetColor("_EmissionColor",new Color(5,3,.7f));Render();float withoutBloom=read.GetPixel(145,128).r;
    data.renderPostProcessing=true;Render("bloom-on");float withBloom=read.GetPixel(145,128).r;Require(withBloom>withoutBloom+.001f,"URP bloom must illuminate pixels outside the emissive geometry");
    plane.SetActive(false);sun.enabled=true;material.SetColor("_EmissionColor",Color.black);material.color=Color.white;
    stripes=new Texture2D(64,2,TextureFormat.RGB24,false){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};for(int x=0;x<64;x++)for(int y=0;y<2;y++)stripes.SetPixel(x,y,x%2==0?Color.black:Color.white);stripes.Apply();material.mainTexture=stripes;
    Quad("NearFocusProbe",new Vector3(-.60f,0,20),.8f,.8f);Quad("FarFocusProbe",new Vector3(.60f,0,180),.8f,.8f);
    FloorOrthographicFocus focus=null;foreach(var feature in renderer.rendererFeatures)if(feature is FloorOrthographicFocus value)focus=value;Require(focus!=null,"orthographic focus feature exists");
    float Energy(int first,int last){float sum=0;for(int x=first;x<last;x++)sum+=Mathf.Abs(read.GetPixel(x+1,128).g-read.GetPixel(x,128).g);return sum/(last-first);}
    focus.SetActive(false);Render("focus-off");float nearBefore=Energy(16,75),farBefore=Energy(181,240);
    renderer.copyDepthMode=CopyDepthMode.ForcePrepass;renderer.SetDirty();focus.SetActive(true);Render("focus-on-forced-depth-prepass");float nearAfter=Energy(16,75),farAfter=Energy(181,240);
    Require(nearBefore>.03f&&farBefore>.03f,"both real near/far patterns must render a meaningful contrast control");
    Require(nearAfter/nearBefore>.90f,"forced depth prepass must preserve sharp opaque foreground");Require(farAfter/farBefore<.90f,"orthographic distant geometry must visibly soften");
    File.WriteAllText(Path.Combine(output,"CONCEPT-GPU-PASS.txt"),"CONCEPT_GPU_PASS maskDelta="+maskDelta+" warmPointRedGain="+warmR+" bloomHaloGain="+(withBloom-withoutBloom)+" nearSharpRatio="+(nearAfter/nearBefore)+" farSoftRatio="+(farAfter/farBefore)+" forcedOpaqueDepthPrepass=true originalBrassReflectionDelta="+reflectionGain+" originalGlassReflectionDelta="+glassGain);
    Debug.Log("FLOOR_CONCEPT_GPU_PASS");
   }
   finally
   {
    RenderSettings.sun=previousSun;RenderSettings.fog=fog;renderer.copyDepthMode=previousCopy;renderer.SetDirty();foreach(var feature in renderer.rendererFeatures)if(feature is FloorOrthographicFocus)feature.SetActive(true);
    if(target!=null){target.Release();UnityEngine.Object.Destroy(target);}if(read!=null)UnityEngine.Object.Destroy(read);if(material!=null)UnityEngine.Object.Destroy(material);if(stripes!=null)UnityEngine.Object.Destroy(stripes);root.SetActive(false);UnityEngine.Object.Destroy(root);
   }
  }
 }
}
#endif
