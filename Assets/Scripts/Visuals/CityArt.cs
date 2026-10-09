using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class CityArt
 {
  public static void Build(Transform parent){var sky=new Material(Shader.Find("Floor/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/SkyPanorama");RenderSettings.skybox=sky;ArtLifetime.Own(parent.gameObject,sky);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.36f,.44f,.58f);RenderSettings.ambientEquatorColor=new Color(.22f,.30f,.40f);RenderSettings.ambientGroundColor=new Color(.15f,.19f,.29f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.22f,.25f,.34f);RenderSettings.fogDensity=.003f;
   var random=new System.Random(261008);var graphite=MeshArt.Mat("CityGraphite",new Color(.28f,.30f,.44f),1);var cyan=MeshArt.Mat("CityCyan",new Color(.02f,.34f,.65f),-1,true);var magenta=MeshArt.Mat("CityPink",new Color(.62f,.015f,.27f),-1,true);var amber=MeshArt.Mat("CityAmber",new Color(.67f,.30f,.03f),-1,true);
   var root=new GameObject("NeonCity");root.transform.SetParent(parent,false);
   for(int i=0;i<64;i++){float sign=i%2==0?-1:1,x=sign*(21+(float)random.NextDouble()*35),z=-8+i*2.7f,height=10+(float)random.NextDouble()*40;var building=new GameObject("TowerFacade").transform;building.SetParent(root.transform,false);building.localPosition=new Vector3(x,-12,z);float w=3+(float)random.NextDouble()*4,d=3+(float)random.NextDouble()*5;
    MeshArt.Box(building,"SteppedCore",new Vector3(0,height*.5f,0),new Vector3(w,height,d),graphite);MeshArt.Box(building,"UpperCrown",new Vector3(0,height+1,0),new Vector3(w*.7f,2,d*.7f),graphite);MeshArt.Box(building,"Aerial",new Vector3(0,height+3,0),new Vector3(.10f,4,.10f),graphite);
    var neon=i%3==0?magenta:i%3==1?cyan:amber;MeshArt.Box(building,"VerticalSign",new Vector3(-sign*w*.51f,height*.6f,0),new Vector3(.035f,height*.35f,.10f),neon);
    for(int k=0;k<12;k++){float y=2+k*(height-3)/12;MeshArt.Box(building,"WindowBand",new Vector3(-sign*w*.51f,y,0),new Vector3(.025f,.06f,d*.7f),k%4==0?amber:neon);}
   }
   // The rear skyline supplies depth behind the route, well beyond the last playable deck.
   var distant=MeshArt.Mat("DistantCity",new Color(.13f,.20f,.31f),1);
   for(int i=0;i<19;i++){float height=16+(i*13%7)*6;var tower=new GameObject("RearSkyline").transform;tower.SetParent(root.transform,false);tower.localPosition=new Vector3(-63+i*7, -14,164+(i%4)*11);MeshArt.Box(tower,"FacetedTower",new Vector3(0,height*.5f,0),new Vector3(4.5f,height,5),distant);MeshArt.Box(tower,"CrownSetback",new Vector3(0,height+2,0),new Vector3(3,4,3.5f),distant);MeshArt.Box(tower,"CrownSignal",new Vector3(0,height+4.2f,0),new Vector3(2.8f,.09f,3.3f),i%3==0?amber:cyan);
    for(int row=0;row<10;row++)for(int col=0;col<3;col++)if((row+col+i)%3!=0)MeshArt.Box(tower,"RearWindow",new Vector3(-1.3f+col*1.3f,2+row*(height-4)/10,-2.52f),new Vector3(.48f,.13f,.025f),row%4==0?amber:cyan);
   }
   // Batched distant buildings have no colliders and never become landing surfaces.
   MeshArt.Batch(root);
  }
 }
}
