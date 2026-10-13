#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using Kamilunavo.OneMoreFloor.Visuals;
// Capture only original editable art, outside the artist's layer31. A single
// portable resource is shipped; no probe/capture camera runs on the phone.
public static class FloorReflectionAuthoring
{
 static GameObject _root;static Camera _camera;static RenderTexture _target;static Material _sky;
 static Light[] _priorLights;static bool[] _priorEnabled;static AmbientMode _ambientMode;static Color _ambient;static SphericalHarmonicsL2 _ambientProbe;
 static Light _sun;static Material _priorSky;static bool _priorFog,_priorAsync;static int _face;static string _output;
 public static void Begin(string output)
 {
  if(_root!=null)throw new InvalidOperationException("Original reflection bake already active");
  _output=output;_sun=RenderSettings.sun;_priorSky=RenderSettings.skybox;_priorFog=RenderSettings.fog;_priorAsync=ShaderUtil.allowAsyncCompilation;
  _ambientMode=RenderSettings.ambientMode;_ambient=RenderSettings.ambientLight;_ambientProbe=RenderSettings.ambientProbe;
  _priorLights=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);_priorEnabled=new bool[_priorLights.Length];
  for(int i=0;i<_priorLights.Length;i++){_priorEnabled[i]=_priorLights[i].enabled;_priorLights[i].enabled=false;}
  RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.32f,.29f,.25f);var controlled=new SphericalHarmonicsL2();controlled.AddAmbientLight(RenderSettings.ambientLight);RenderSettings.ambientProbe=controlled;
  try{
   _root=new GameObject("OriginalAtriumReflectionAuthoring"){hideFlags=HideFlags.DontSave};
   var original=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/SculptedConcept/GrandLobby.prefab");
   var art=UnityEngine.Object.Instantiate(original,_root.transform);SetLayer(art.transform);
   var copies=new Dictionary<Material,Material>();foreach(var renderer in art.GetComponentsInChildren<Renderer>()){
    var slots=renderer.sharedMaterials;for(int i=0;i<slots.Length;i++)if(slots[i]!=null&&slots[i].HasProperty("_ReflectionStrength")){
     if(!copies.TryGetValue(slots[i],out var copy)){copy=UnityEngine.Object.Instantiate(slots[i]);copy.SetFloat("_ReflectionStrength",0);copies.Add(slots[i],copy);ArtLifetime.Own(_root,copy);}slots[i]=copy;
    }renderer.sharedMaterials=slots;
   }
   var key=new GameObject("ReflectionSun",typeof(Light));key.transform.SetParent(_root.transform,false);key.transform.rotation=Quaternion.Euler(32,-35,0);
   var light=key.GetComponent<Light>();light.type=LightType.Directional;light.color=new Color(1,.88f,.69f);light.intensity=1.05f;light.cullingMask=1<<30;RenderSettings.sun=light;
   _sky=new Material(Resources.Load<Shader>("FloorSky")){mainTexture=Resources.Load<Texture2D>("Art/CanalSunset-20261012")};RenderSettings.skybox=_sky;RenderSettings.fog=false;ShaderUtil.allowAsyncCompilation=false;
   foreach(var renderer in art.GetComponentsInChildren<Renderer>())foreach(var material in renderer.sharedMaterials)if(material!=null)ShaderUtil.CompilePass(material,0,true);
   var eye=new GameObject("OriginalReflectionEye",typeof(Camera));eye.transform.SetParent(_root.transform,false);_camera=eye.GetComponent<Camera>();_camera.enabled=false;_camera.transform.position=new Vector3(0,2,-1);_camera.cullingMask=1<<30;_camera.clearFlags=CameraClearFlags.Skybox;_camera.fieldOfView=90;_camera.aspect=1;_camera.nearClipPlane=.05f;_camera.farClipPlane=40;_camera.allowHDR=true;_camera.GetUniversalAdditionalCameraData().renderPostProcessing=false;eye.AddComponent<Skybox>().material=_sky;
   _target=new RenderTexture(128,128,24,RenderTextureFormat.ARGBHalf);_target.Create();_face=0;EditorApplication.update+=Poll;
  }catch{Release();throw;}
 }
 static void SetLayer(Transform node){node.gameObject.layer=30;var light=node.GetComponent<Light>();if(light!=null){light.cullingMask=1<<30;light.enabled=true;}foreach(Transform child in node)SetLayer(child);}
 static void Poll()
 {
  // Six actual URP camera renders in the Editor. RenderProbe returned -1 in
  // this Editor configuration; use supported SingleCameraRequest explicitly.
  try{
   var cube=new Cubemap(128,GraphicsFormat.R8G8B8A8_UNorm,TextureCreationFlags.MipChain){name="OriginalAtriumReflection",filterMode=FilterMode.Trilinear,wrapMode=TextureWrapMode.Clamp};
   var pixels=new Texture2D(128,128,TextureFormat.RGBAFloat,false,true);var previous=RenderTexture.active;
   try{for(_face=0;_face<6;_face++){
    var directions=new[]{Vector3.right,Vector3.left,Vector3.up,Vector3.down,Vector3.forward,Vector3.back};
    var ups=new[]{Vector3.up,Vector3.up,Vector3.back,Vector3.forward,Vector3.up,Vector3.up};
    _camera.transform.rotation=Quaternion.LookRotation(directions[_face],ups[_face]);FloorConceptLighting.RenderPreview(_camera,_target);
    RenderTexture.active=_target;pixels.ReadPixels(new Rect(0,0,128,128),0,0);pixels.Apply();var colors=pixels.GetPixels();
    for(int i=0;i<colors.Length;i++){var p=colors[i];colors[i]=new Color(p.r/(1+p.r*.38f),p.g/(1+p.g*.38f),p.b/(1+p.b*.38f),1);}
    cube.SetPixels(colors,(CubemapFace)_face);
    var preview=new Texture2D(128,128,TextureFormat.RGBA32,false);try{var encoded=new Color[colors.Length];for(int i=0;i<colors.Length;i++)encoded[i]=colors[i].gamma;preview.SetPixels(encoded);preview.Apply();File.WriteAllBytes(_output+"/reflection-face"+_face+".png",preview.EncodeToPNG());}finally{UnityEngine.Object.DestroyImmediate(preview);}
   }}catch{UnityEngine.Object.DestroyImmediate(cube);throw;}finally{RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(pixels);}
   cube.Apply(true,false);const string path="Assets/Resources/Rendering/CapturedAtriumReflection.asset";Directory.CreateDirectory("Assets/Resources/Rendering");AssetDatabase.Refresh();
   var existing=AssetDatabase.LoadAssetAtPath<Cubemap>(path);if(existing!=null){EditorUtility.CopySerialized(cube,existing);UnityEngine.Object.DestroyImmediate(cube);EditorUtility.SetDirty(existing);}else AssetDatabase.CreateAsset(cube,path);
   AssetDatabase.SaveAssets();File.WriteAllText(_output+"/REFLECTION-BAKE-PASS.txt","Native Unity URP camera renders: six HDR128 original lobby faces; portable RGBA32 tonemapped once, filtered mipmaps. No runtime capture.\n");Debug.Log("FLOOR_NATIVE_REFLECTION_BAKE_PASS");
  }catch(Exception error){Debug.LogException(error);File.WriteAllText(_output+"/reflection-bake-failure.txt",error.ToString());}
  finally{Release();}
 }
 static void Release(){EditorApplication.update-=Poll;RenderSettings.ambientMode=_ambientMode;RenderSettings.ambientLight=_ambient;RenderSettings.ambientProbe=_ambientProbe;for(int i=0;i<_priorLights.Length;i++)if(_priorLights[i]!=null)_priorLights[i].enabled=_priorEnabled[i];RenderSettings.sun=_sun;RenderSettings.skybox=_priorSky;RenderSettings.fog=_priorFog;ShaderUtil.allowAsyncCompilation=_priorAsync;if(_root!=null)UnityEngine.Object.DestroyImmediate(_root);if(_target!=null){_target.Release();UnityEngine.Object.DestroyImmediate(_target);}if(_sky!=null)UnityEngine.Object.DestroyImmediate(_sky);_root=null;_camera=null;_target=null;_sky=null;}
}
#endif
