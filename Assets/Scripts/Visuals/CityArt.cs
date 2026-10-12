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
   var sky=new Material(Shader.Find("Floor/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/CanalSunset-20261012")??Resources.Load<Texture2D>("Art/SkyPanorama");RenderSettings.skybox=sky;ArtLifetime.Own(parent.gameObject,sky);
   RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.52f,.48f,.41f);RenderSettings.ambientEquatorColor=new Color(.29f,.28f,.24f);RenderSettings.ambientGroundColor=new Color(.14f,.19f,.17f);
   RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.84f,.64f,.49f);RenderSettings.fogDensity=.005f;
   var stone=FloorArchitecturePalette.Get(parent,FloorArchitectureSurface.CityStone);
   var shade=FloorArchitecturePalette.Get(parent,FloorArchitectureSurface.CityShade);
   var trim=FloorArchitecturePalette.Get(parent,FloorArchitectureSurface.CityTrim);
   var copper=FloorArchitecturePalette.Get(parent,FloorArchitectureSurface.Copper);
   var foliage=FloorArchitecturePalette.Get(parent,FloorArchitectureSurface.Leaf);var leafLight=FloorArchitecturePalette.Get(parent,FloorArchitectureSurface.LeafLight);
   var teal=MeshArt.Mat("QuarterTealRoof",new Color(.17f,.30f,.29f));
   var glass=MeshArt.Mat("QuarterWindowGlass",new Color(.17f,.25f,.24f));
   var warmGlass=MeshArt.Mat("QuarterWarmWindow",new Color(.49f,.37f,.22f));
   var distant=MeshArt.Mat("QuarterDistantStone",new Color(.44f,.46f,.43f));
   var distantGlass=MeshArt.Mat("QuarterDistantGlass",new Color(.29f,.36f,.34f));
   var water=MeshArt.Mat("QuarterCanalWater",new Color(.21f,.34f,.32f));
   var root=new GameObject("CanalQuarter");root.transform.SetParent(parent,false);root.transform.localPosition=Vector3.down*65;
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
    // Lower foreground roofs expose the sunset and layered city beneath the lift.
    float roof=-66+z*.025f+(float)random.NextDouble()*5;
    var building=Root(root.transform,"CanalMasonry",new Vector3(x,0,z));
    Body(building,width,depth,roof,i%3==0?shade:stone,trim);
    Windows(building,width,depth,roof,side,glass,warmGlass,trim,i);
    if(i<12&&i%3==0)FloorArchitectureDetails.Ivy(building,new Vector3(width*.39f,roof-.6f,-depth*.5f-.20f),2.1f,i+31,foliage,leafLight);
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
   // A lower historic skyline exposes actual roof terminations below the lift.
   // Bell chambers remain below the initial runner rather than filling the sky with piers.
   for(int i=0;i<12;i++)
   {
    int side=i%2==0?-1:1;float width=4.4f+i%3*.7f,depth=5.1f,roof=-64+(i*11%5)*2;
    var tower=Root(root.transform,"DistantStoneTower",new Vector3(side*(66+i%4*12),0,92+i/2*21));
    Body(tower,width,depth,roof,distant,shade);
    FloorArchitectureDetails.BeveledBlock(tower,"BellTowerPediment",new Vector3(0,roof+.7f,0),new Vector3(width+.5f,.6f,depth+.5f),.10f,trim);
    var bell=Root(tower,"HistoricBellChamber",new Vector3(0,roof+1,0));
    for(int edge=-1;edge<=1;edge+=2)FloorArchitectureDetails.BeveledBlock(bell,"BellChamberPier",new Vector3(edge*width*.32f,2.1f,-depth*.28f),new Vector3(.42f,4.2f,.48f),.08f,stone);
    for(int segment=0;segment<9;segment++)FloorArchitectureDetails.ArchStone(bell,"BellChamberArch",segment*Mathf.PI/9,(segment+1)*Mathf.PI/9,width*.23f,width*.30f,3.75f,-depth*.29f,.22f,trim);
    FloorArchitectureDetails.BeveledBlock(bell,"BellChamberCornice",new Vector3(0,5.25f,0),new Vector3(width*.91f,.35f,depth*.91f),.07f,trim);
    var bellRoof=MeshArt.Lathe(bell,"BellTowerCopperSpire",new[]{0f,.40f,1.55f,4.1f},new[]{width*.48f,width*.48f,width*.31f,.06f},12,copper,Vector3.one);bellRoof.transform.localPosition=Vector3.up*5.45f;
    for(int row=0;row<5;row++)for(int col=0;col<2;col++)
     MeshArt.Box(tower,"DistantWindowBay",new Vector3((col-.5f)*width*.44f,roof-5-row*6.5f,-depth*.5f-.025f),new Vector3(1.4f,2.5f,.08f),distantGlass);
   }
   for(int i=0;i<9;i++)
   {
    float width=4.8f+i%3*1.1f,depth=6.2f,roof=-66+(i*13%7)*2;
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
  public static void ApplyChapter(int chapter){var sunObject=GameObject.Find("Sun");if(sunObject!=null){var sun=sunObject.GetComponent<Light>();sun.intensity=chapter==1?.58f:chapter==2?1.12f:1.25f;sun.color=chapter==1?new Color(.69f,.78f,1):chapter==2?new Color(1,.97f,.89f):new Color(1,.84f,.63f);}var sky=RenderSettings.skybox;if(sky==null||sky.shader.name!="Floor/Sky")return;string art=chapter==1?"Art/CanalNeon-20261012":chapter==2?"Art/CanalDay-20261012":"Art/CanalSunset-20261012";var texture=Resources.Load<Texture2D>(art);if(texture!=null)sky.mainTexture=texture;RenderSettings.fogColor=chapter==1?new Color(.16f,.23f,.34f):chapter==2?new Color(.73f,.81f,.82f):new Color(.84f,.64f,.49f);RenderSettings.ambientSkyColor=chapter==1?new Color(.28f,.35f,.49f):chapter==2?new Color(.57f,.63f,.67f):new Color(.52f,.48f,.41f);}
  private static Transform Root(Transform parent,string name,Vector3 position)
  {
   var root=new GameObject(name).transform;root.SetParent(parent,false);root.localPosition=position;return root;
  }
  private static void Body(Transform building,float width,float depth,float roof,Material stone,Material trim)
  {
   FloorArchitectureDetails.BeveledBlock(building,"GroundedFacade",new Vector3(0,(Foundation+roof)*.5f,0),new Vector3(width,roof-Foundation,depth),.12f,stone);
   FloorArchitectureDetails.BeveledBlock(building,"RoofCornice",new Vector3(0,roof,0),new Vector3(width+.65f,.45f,depth+.65f),.12f,trim);
   FloorArchitectureDetails.BeveledBlock(building,"UpperStringCourse",new Vector3(0,roof-5,0),new Vector3(width+.28f,.28f,depth+.28f),.12f,trim);
   FloorArchitectureDetails.BeveledBlock(building,"QuayMasonryPlinth",new Vector3(0,CanalLevel+1.15f,0),new Vector3(width+.40f,2.3f,depth+.40f),.13f,trim);
   for(int side=-1;side<=1;side+=2)
    FloorArchitectureDetails.BeveledBlock(building,"CornerPier",new Vector3(side*(width*.5f-.25f),(Foundation+roof)*.5f,-depth*.5f-.035f),new Vector3(.5f,roof-Foundation,.15f),.12f,trim);
  }
  private static void Windows(Transform building,float width,float depth,float roof,int side,Material glass,Material warm,Material trim,int variation)
  {
   for(int row=0;row<4;row++)for(int col=0;col<3;col++)
   {
    float y=roof-3-row*4.5f,x=(col-1)*width*.27f;
    var material=(row+col+variation)%7==0?warm:glass;
    bool arched=variation<8&&row<2;
    MeshArt.Box(building,"FrontWindowBay",new Vector3(x,y-(arched?.10f:0),-depth*.5f-.055f),new Vector3(1.05f,arched?1.50f:1.8f,.1f),material);
    if(!arched){
     for(int edge=-1;edge<=1;edge+=2)FloorArchitectureDetails.BeveledBlock(building,"SquareWindowReveal",new Vector3(x+edge*.59f,y,-depth*.5f-.19f),new Vector3(.14f,1.99f,.25f),.025f,trim);
     FloorArchitectureDetails.BeveledBlock(building,"WindowSill",new Vector3(x,y-.96f,-depth*.5f-.21f),new Vector3(1.42f,.14f,.36f),.025f,trim);
     MeshArt.Box(building,"WindowTransom",new Vector3(x,y+.89f,-depth*.5f-.15f),new Vector3(1.22f,.08f,.15f),trim);
    }
    if(arched){
     for(int segment=0;segment<7;segment++){var arch=FloorArchitectureDetails.ArchStone(building,"CityArchedLintel",segment*Mathf.PI/7,(segment+1)*Mathf.PI/7,.53f,.67f,y+.65f,-depth*.5f-.18f,.12f,trim);arch.transform.localPosition=Vector3.right*x;}
     var fan=new Vector3[14];var triangles=new int[36];fan[0]=new Vector3(x,y+.65f,-depth*.5f-.11f);
     for(int k=0;k<=12;k++){float a=k*Mathf.PI/12;fan[k+1]=new Vector3(x+Mathf.Cos(a)*.53f,y+.65f+Mathf.Sin(a)*.53f,-depth*.5f-.11f);if(k<12){triangles[k*3]=0;triangles[k*3+1]=k+2;triangles[k*3+2]=k+1;}}
     MeshArt.Mesh(building,"CityTransomGlass",fan,triangles,null,material);
     for(int edge=-1;edge<=1;edge+=2)MeshArt.Box(building,"WindowStoneReveal",new Vector3(x+edge*.59f,y-.12f,-depth*.5f-.15f),new Vector3(.12f,1.56f,.18f),trim);
     MeshArt.Box(building,"WindowSill",new Vector3(x,y-.94f,-depth*.5f-.17f),new Vector3(1.42f,.14f,.28f),trim);
    }
   }
   for(int row=0;row<4;row++)for(int col=0;col<2;col++)
    MeshArt.Box(building,"CanalWindowBay",new Vector3(-side*(width*.5f+.055f),roof-3-row*4.5f,(col-.5f)*depth*.43f),new Vector3(.1f,1.8f,1.25f),(row+col+variation)%7==0?warm:glass);
  }
 }
}
