using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class CityArt
 {
  public static void Build(Transform parent){var sky=new Material(Shader.Find("Floor/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/SkyPanorama");RenderSettings.skybox=sky;ArtLifetime.Own(parent.gameObject,sky);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.44f,.34f,.54f);RenderSettings.ambientEquatorColor=new Color(.38f,.27f,.38f);RenderSettings.ambientGroundColor=new Color(.15f,.19f,.29f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.36f,.23f,.40f);RenderSettings.fogDensity=.004f;
   var random=new System.Random(261008);var graphite=MeshArt.Mat("CityGraphite",new Color(.28f,.30f,.44f),1);var cyan=MeshArt.Mat("CityCyan",new Color(.02f,.34f,.65f),-1,true);var magenta=MeshArt.Mat("CityPink",new Color(.62f,.015f,.27f),-1,true);var amber=MeshArt.Mat("CityAmber",new Color(.67f,.30f,.03f),-1,true);
   var root=new GameObject("NeonCity");root.transform.SetParent(parent,false);
   for(int i=0;i<64;i++){float sign=i%2==0?-1:1,x=sign*(21+(float)random.NextDouble()*35),z=-8+i*2.7f,height=10+(float)random.NextDouble()*40;var building=new GameObject("TowerFacade").transform;building.SetParent(root.transform,false);building.localPosition=new Vector3(x,-12,z);float w=3+(float)random.NextDouble()*4,d=3+(float)random.NextDouble()*5;
    MeshArt.Box(building,"SteppedCore",new Vector3(0,height*.5f,0),new Vector3(w,height,d),graphite);MeshArt.Box(building,"UpperCrown",new Vector3(0,height+1,0),new Vector3(w*.7f,2,d*.7f),graphite);MeshArt.Box(building,"Aerial",new Vector3(0,height+3,0),new Vector3(.10f,4,.10f),graphite);
    var neon=i%3==0?magenta:i%3==1?cyan:amber;MeshArt.Box(building,"VerticalSign",new Vector3(-sign*w*.51f,height*.6f,0),new Vector3(.035f,height*.35f,.10f),neon);
    for(int k=0;k<12;k++){float y=2+k*(height-3)/12;MeshArt.Box(building,"WindowBand",new Vector3(-sign*w*.51f,y,0),new Vector3(.025f,.06f,d*.7f),k%4==0?amber:neon);}
   }
   // Batched distant buildings have no colliders and never become landing surfaces.
   MeshArt.Batch(root);
  }
 }
}
