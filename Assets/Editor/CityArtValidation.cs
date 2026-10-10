#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;

// Independent environment contract; run before the rendered timing/upper-floor
// showcase. Pixel hierarchy and device frame cost still need player evidence.
public static class CityArtValidation
{
 public static void Validate()
 {
  int checks=0;
  void Check(bool valid,string message){checks++;if(!valid)throw new Exception("CITY_ART: "+message);}
  var root=new GameObject("CanalQuarterValidation");
  var sky=RenderSettings.skybox;var ambientMode=RenderSettings.ambientMode;
  var ambientSky=RenderSettings.ambientSkyColor;var ambientEquator=RenderSettings.ambientEquatorColor;var ambientGround=RenderSettings.ambientGroundColor;
  bool fog=RenderSettings.fog;var fogMode=RenderSettings.fogMode;var fogColor=RenderSettings.fogColor;float fogDensity=RenderSettings.fogDensity;
  try
  {
   CityArt.Build(root.transform);
   var quarter=root.transform.Find("CanalQuarter");Check(quarter!=null,"grounded canal quarter exists");
   Check(root.GetComponentsInChildren<Collider>().Length==0,"environment cannot alter transfer collision");
   var canal=quarter.Find("Canal");Check(canal!=null&&canal.localPosition.y<-60,"canal lies below the playable route");
   int bodies=0,windows=0,roofs=0;var materials=new HashSet<Material>();
   foreach(var node in quarter.GetComponentsInChildren<Transform>())
   {
    Check(node.name!="WindowBand"&&node.name!="VerticalSign"&&node.name!="CrownSignal","legacy neon stripe geometry absent");
    if(node.name=="GroundedFacade")
    {
     bodies++;Check(node.GetComponent<Renderer>().bounds.min.y<=canal.localPosition.y-20,"masonry foundation reaches beneath the low canal");
     var district=node.parent;Check(district.localPosition.z>=200||Mathf.Abs(district.localPosition.x)>24,"building leaves the central lift corridor open");
     Check(district.Find("RoofCornice")!=null,"building has an architectural roof termination");
    }
    if(node.name.EndsWith("WindowBay",StringComparison.Ordinal))windows++;
    if(node.name=="CopperCupola"||node.name=="TealMansard"||node.name=="RearTealSpire")roofs++;
   }
   Check(bodies>0&&windows>0&&windows<bodies*25,"sparse window bays replace dense neon grids");
   Check(roofs>0,"authored copper and teal roof silhouettes exist");
   foreach(var renderer in quarter.GetComponentsInChildren<Renderer>())
   {
    materials.Add(renderer.sharedMaterial);
    Check(!renderer.sharedMaterial.IsKeywordEnabled("_EMISSION"),"background surfaces cannot compete with the lift signals");
   }
   Check(materials.Count<=12,"city uses a small shared material palette");
   Check(RenderSettings.fogDensity>=.004f,"distant silhouettes retain atmospheric separation");
   Debug.Log("CITY_ART_EDITOR_PASS checks="+checks+" facades="+bodies+" windowBays="+windows+" sharedMaterials="+materials.Count);
  }
  finally
  {
   RenderSettings.skybox=sky;RenderSettings.ambientMode=ambientMode;RenderSettings.ambientSkyColor=ambientSky;RenderSettings.ambientEquatorColor=ambientEquator;RenderSettings.ambientGroundColor=ambientGround;
   RenderSettings.fog=fog;RenderSettings.fogMode=fogMode;RenderSettings.fogColor=fogColor;RenderSettings.fogDensity=fogDensity;
   UnityEngine.Object.DestroyImmediate(root);
  }
 }
}
#endif
