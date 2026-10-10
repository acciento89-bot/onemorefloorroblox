using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class CityArt
 {
  // Foundations stay below the canal and the entire playable route. Decorative city
  // geometry never supplies a landing surface or enters the central lift corridor.
  private const float Foundation=-96f,CanalLevel=-72f;
  public static void Build(Transform parent)
  {
   var sky=new Material(Shader.Find("Floor/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/SkyPanorama");RenderSettings.skybox=sky;ArtLifetime.Own(parent.gameObject,sky);
   RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.44f,.50f,.56f);RenderSettings.ambientEquatorColor=new Color(.44f,.43f,.38f);RenderSettings.ambientGroundColor=new Color(.24f,.29f,.28f);
   RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.69f,.61f,.52f);RenderSettings.fogDensity=.005f;
   var stone=MeshArt.Mat("QuarterLimestone",new Color(.59f,.55f,.46f));
   var shade=MeshArt.Mat("QuarterSandstone",new Color(.47f,.46f,.40f));
   var trim=MeshArt.Mat("QuarterCornice",new Color(.68f,.63f,.53f));
   var copper=MeshArt.Mat("QuarterCopper",new Color(.43f,.28f,.17f));
   var teal=MeshArt.Mat("QuarterTealRoof",new Color(.17f,.30f,.29f));
   var glass=MeshArt.Mat("QuarterWindowGlass",new Color(.17f,.25f,.24f));
   var warmGlass=MeshArt.Mat("QuarterWarmWindow",new Color(.49f,.37f,.22f));
   var distant=MeshArt.Mat("QuarterDistantStone",new Color(.44f,.46f,.43f));
   var distantGlass=MeshArt.Mat("QuarterDistantGlass",new Color(.29f,.36f,.34f));
   var water=MeshArt.Mat("QuarterCanalWater",new Color(.21f,.34f,.32f));
   var root=new GameObject("CanalQuarter");root.transform.SetParent(parent,false);
   // A continuous lower city, rather than towers cut off just beneath the decks.
   MeshArt.Box(root.transform,"Canal",new Vector3(0,CanalLevel,110),new Vector3(46,.35f,320),water);
   for(int side=-1;side<=1;side+=2)
   {
    MeshArt.Box(root.transform,"StoneEmbankment",new Vector3(side*76,CanalLevel-1.5f,110),new Vector3(106,3,320),shade);
    MeshArt.Box(root.transform,"CanalParapet",new Vector3(side*24.5f,CanalLevel+1.2f,110),new Vector3(1.2f,2.4f,320),stone);
    MeshArt.Box(root.transform,"QuayCoping",new Vector3(side*24.5f,CanalLevel+2.5f,110),new Vector3(1.5f,.22f,320),trim);
   }
   var random=new System.Random(261010);
   // Low, broad masonry blocks frame the open corridor. Recessed windows face
   // both the canal and the camera; there are no luminous bands or vertical signs.
   for(int i=0;i<24;i++)
   {
    int side=i%2==0?-1:1;float z=12+(i/2)*15f;
    float width=7+(float)random.NextDouble()*4,depth=8+(float)random.NextDouble()*5;
    float x=side*(32+(i%6)*6+(float)random.NextDouble()*4);
    float roof=-7+z*.15f+(float)random.NextDouble()*11;
    var building=Root(root.transform,"CanalMasonry",new Vector3(x,0,z));
    Body(building,width,depth,roof,i%3==0?shade:stone,trim);
    Windows(building,width,depth,roof,side,glass,warmGlass,i);
    if(i%4==0)
    {
     MeshArt.Box(building,"DrumCornice",new Vector3(0,roof+1.1f,0),new Vector3(width*.69f,1.8f,depth*.64f),stone);
     var dome=MeshArt.Lathe(building,"CopperCupola",new[]{0f,.6f,2f,3.2f,3.7f},new[]{width*.36f,width*.36f,width*.29f,width*.13f,0f},16,copper,Vector3.one);
     dome.transform.localPosition=new Vector3(0,roof+2,0);
    }
    else if(i%4==1)
    {
     var roofMesh=MeshArt.Lathe(building,"TealMansard",new[]{0f,1.8f,2.1f},new[]{width*.49f,width*.29f,width*.29f},4,teal,new Vector3(1,1,depth/width),true);
     roofMesh.transform.localPosition=new Vector3(0,roof+.4f,0);roofMesh.transform.localRotation=Quaternion.Euler(0,45,0);
    }
    else
    {
     MeshArt.Box(building,"RoofTerrace",new Vector3(0,roof+.6f,0),new Vector3(width*.86f,.65f,depth*.86f),teal);
     MeshArt.Box(building,"RoofLantern",new Vector3(width*.18f,roof+1.8f,0),new Vector3(1.5f,1.8f,1.8f),copper);
    }
   }
   // A separate distant, subdued modern district supplies depth at upper floors.
   // Tall buildings are farther from the transfer than the masonry quarter.
   for(int i=0;i<12;i++)
   {
    int side=i%2==0?-1:1;float width=8+i%3*2,depth=10,roof=42+(i*11%5)*6;
    var tower=Root(root.transform,"DistantStoneTower",new Vector3(side*(66+i%4*12),0,92+i/2*21));
    Body(tower,width,depth,roof,distant,shade);
    MeshArt.Box(tower,"CopperSetback",new Vector3(0,roof+2.5f,0),new Vector3(width*.70f,5,depth*.7f),copper);
    for(int row=0;row<5;row++)for(int col=0;col<2;col++)
     MeshArt.Box(tower,"DistantWindowBay",new Vector3((col-.5f)*width*.44f,roof-5-row*6.5f,-depth*.5f-.025f),new Vector3(1.4f,2.5f,.08f),distantGlass);
   }
   for(int i=0;i<9;i++)
   {
    float width=7+i%3*2,depth=9,roof=42+(i*13%7)*5;
    var tower=Root(root.transform,"RearQuarterSilhouette",new Vector3(-76+i*19,0,208+i%3*15));
    Body(tower,width,depth,roof,distant,shade);
    if(i%3==0)
    {
     var spire=MeshArt.Lathe(tower,"RearTealSpire",new[]{0f,2f,7f},new[]{width*.40f,width*.32f,0f},8,teal,Vector3.one);
     spire.transform.localPosition=new Vector3(0,roof+.4f,0);
    }
    else MeshArt.Box(tower,"RearCopperCrown",new Vector3(0,roof+1.5f,0),new Vector3(width*.7f,3,depth*.7f),copper);
   }
   MeshArt.Batch(root);
  }
  private static Transform Root(Transform parent,string name,Vector3 position)
  {
   var root=new GameObject(name).transform;root.SetParent(parent,false);root.localPosition=position;return root;
  }
  private static void Body(Transform building,float width,float depth,float roof,Material stone,Material trim)
  {
   MeshArt.Box(building,"GroundedFacade",new Vector3(0,(Foundation+roof)*.5f,0),new Vector3(width,roof-Foundation,depth),stone);
   MeshArt.Box(building,"RoofCornice",new Vector3(0,roof,0),new Vector3(width+.65f,.45f,depth+.65f),trim);
   MeshArt.Box(building,"UpperStringCourse",new Vector3(0,roof-5,0),new Vector3(width+.28f,.28f,depth+.28f),trim);
   for(int side=-1;side<=1;side+=2)
    MeshArt.Box(building,"CornerPier",new Vector3(side*(width*.5f-.25f),(Foundation+roof)*.5f,-depth*.5f-.035f),new Vector3(.5f,roof-Foundation,.15f),trim);
  }
  private static void Windows(Transform building,float width,float depth,float roof,int side,Material glass,Material warm,int variation)
  {
   for(int row=0;row<4;row++)for(int col=0;col<3;col++)
   {
    float y=roof-3-row*4.5f,x=(col-1)*width*.27f;
    var material=(row+col+variation)%7==0?warm:glass;
    MeshArt.Box(building,"FrontWindowBay",new Vector3(x,y,-depth*.5f-.055f),new Vector3(1.05f,1.8f,.1f),material);
   }
   for(int row=0;row<4;row++)for(int col=0;col<2;col++)
    MeshArt.Box(building,"CanalWindowBay",new Vector3(-side*(width*.5f+.055f),roof-3-row*4.5f,(col-.5f)*depth*.43f),new Vector3(.1f,1.8f,1.25f),(row+col+variation)%7==0?warm:glass);
  }
 }
}
