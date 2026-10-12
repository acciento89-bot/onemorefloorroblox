#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Rendering;
public static class FloorCharacterTintValidation
{
 public static void BuildAndValidate(){Validate();BuildAutomation.BuildMacPreview();}
 // Actual GPU regression: a single opaque cloth triangle must smoothly blend
 // the owned base colour and absolute pack accent, without a dark cutoff band.
 public static void Validate(){if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null)throw new InvalidOperationException("CHARACTER_TINT requires a real graphics device");
  var root=new GameObject("CharacterTintGpuFixture");Mesh mesh=null;Material material=null;RenderTexture target=null;Texture2D read=null;var ambient=RenderSettings.ambientMode;var colour=RenderSettings.ambientLight;
  try{
   RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=Color.white*.3f;root.transform.position=new Vector3(60000,60000,60000);
   var quad=new GameObject("AccentGradient",typeof(MeshFilter),typeof(MeshRenderer));quad.transform.SetParent(root.transform,false);quad.layer=31;
   mesh=new Mesh();mesh.vertices=new[]{new Vector3(-1,-1,0),new Vector3(1,-1,0),new Vector3(1,1,0),new Vector3(-1,1,0)};mesh.normals=new[]{Vector3.back,Vector3.back,Vector3.back,Vector3.back};mesh.uv=new[]{Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero};mesh.colors=new[]{Color.white,new Color(.055f,.20f,.21f,.5f),new Color(.055f,.20f,.21f,.5f),Color.white};mesh.triangles=new[]{0,2,1,0,3,2};mesh.RecalculateBounds();quad.GetComponent<MeshFilter>().sharedMesh=mesh;
   material=new Material(Resources.Load<Shader>("FloorRunnerSurface"));material.color=new Color(.29f,.20f,.115f);material.SetFloat("_SurfaceKind",0);material.SetFloat("_Glossiness",0);quad.GetComponent<Renderer>().sharedMaterial=material;quad.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
   var key=new GameObject("FixtureLight",typeof(Light));key.transform.SetParent(root.transform,false);var light=key.GetComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<31;light.intensity=.8f;
   var view=new GameObject("FixtureCamera",typeof(Camera));view.transform.SetParent(root.transform,false);view.transform.localPosition=new Vector3(0,0,-3);var camera=view.GetComponent<Camera>();camera.orthographic=true;camera.orthographicSize=.32f;camera.nearClipPlane=.1f;camera.farClipPlane=5;camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;
   target=new RenderTexture(256,64,16,RenderTextureFormat.ARGB32);target.Create();camera.targetTexture=target;camera.aspect=4;camera.Render();read=new Texture2D(256,64,TextureFormat.RGB24,false);var previous=RenderTexture.active;try{RenderTexture.active=target;read.ReadPixels(new Rect(0,0,256,64),0,0);read.Apply();}finally{RenderTexture.active=previous;}
   float jump=0,maxGreen=0;for(int x=100;x<225;x++){float a=read.GetPixel(x,32).g,b=read.GetPixel(x+1,32).g;jump=Mathf.Max(jump,Mathf.Abs(a-b));maxGreen=Mathf.Max(maxGreen,b);}
   if(maxGreen<.07f)throw new InvalidOperationException("CHARACTER_TINT fixture did not render a lit accent");if(jump>.025f)throw new InvalidOperationException("CHARACTER_TINT opaque pack tint has a hard interpolation cutoff: "+jump);Debug.Log("FLOOR_CHARACTER_TINT_GPU_PASS maxAdjacentGreenJump="+jump);
  }finally{RenderSettings.ambientMode=ambient;RenderSettings.ambientLight=colour;UnityEngine.Object.DestroyImmediate(root);if(mesh!=null)UnityEngine.Object.DestroyImmediate(mesh);if(material!=null)UnityEngine.Object.DestroyImmediate(material);if(target!=null){target.Release();UnityEngine.Object.DestroyImmediate(target);}if(read!=null)UnityEngine.Object.DestroyImmediate(read);}
 }
}
#endif
