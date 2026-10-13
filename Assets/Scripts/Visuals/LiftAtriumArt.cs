using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 // The lobby is assembled from the same owned architectural finishes as the course.
 // Its open portal leaves the original chapter environment visible beyond the room.
 public static class LiftAtriumArt
 {
  public static void Build(Transform owner)
  {
   var assets=FloorAuthoredArtSet.Runtime;if(assets!=null){assets.CreateKit(assets.Atrium,owner,"GrandAtriumArchitecture");return;}
   var room=new GameObject("GrandAtriumArchitecture").transform;room.SetParent(owner,false);
   Material Surface(FloorArchitectureSurface kind)=>FloorArchitecturePalette.Get(owner,kind);
   var stone=Surface(FloorArchitectureSurface.Stone);var trim=Surface(FloorArchitectureSurface.Trim);var brass=Surface(FloorArchitectureSurface.Brass);var teal=Surface(FloorArchitectureSurface.Teal);
   const float center=.4f,spring=3.55f,radius=2.65f,depth=4.6f;
   for(int side=-1;side<=1;side+=2){float x=center+side*(radius+.19f);
    FloorArchitectureDetails.BeveledBlock(room,"AtriumPier",new Vector3(x,1.83f,depth),new Vector3(.78f,3.31f,.92f),.065f,stone);
    FloorArchitectureDetails.BeveledBlock(room,"AtriumPierFoot",new Vector3(x,.36f,depth-.04f),new Vector3(1.02f,.36f,1.12f),.055f,trim);
    FloorArchitectureDetails.BeveledBlock(room,"AtriumPierCapital",new Vector3(x,3.44f,depth-.04f),new Vector3(1.08f,.26f,1.15f),.055f,trim);
    var column=MeshArt.Lathe(room,"TurnedAtriumColumn",new[]{.5f,.61f,.69f,.82f,3.05f,3.18f,3.25f,3.34f},new[]{.32f,.32f,.25f,.19f,.19f,.25f,.29f,.29f},20,trim,Vector3.one);column.transform.localPosition=new Vector3(x,0,depth-.66f);
    var flutes=new RunnerMeshBuilder(room,"AtriumColumnFluting",brass);
    for(int flute=0;flute<7;flute++){float angle=Mathf.PI+flute*Mathf.PI/6;var p=new Vector3(x+Mathf.Cos(angle)*.193f,0,depth-.66f+Mathf.Sin(angle)*.193f);flutes.Stitch(p+Vector3.up*.91f,p+Vector3.up*2.98f,.008f);}flutes.Finish();
    FloorArchitectureDetails.BeveledBlock(room,"SideGalleryWall",new Vector3(x+side*1.0f,3.4f,depth+.14f),new Vector3(1.7f,6.6f,.54f),.06f,stone);
    FloorArchitectureDetails.BeveledBlock(room,"GalleryWainscot",new Vector3(x+side*.85f,1.08f,depth-.24f),new Vector3(1.65f,1.6f,.20f),.04f,trim);
    FloorArchitectureDetails.BeveledBlock(room,"GalleryInsetPanel",new Vector3(x+side*.85f,1.11f,depth-.36f),new Vector3(1.28f,1.19f,.08f),.04f,teal);
    MeshArt.Box(room,"GalleryGoldChairRail",new Vector3(x+side*.86f,1.91f,depth-.38f),new Vector3(1.74f,.075f,.09f),brass);
    var planter=MeshArt.Lathe(room,"AtriumStoneUrn",new[]{0f,.10f,.15f,.40f,.60f,.64f},new[]{.31f,.35f,.22f,.34f,.46f,.48f},20,trim,Vector3.one);planter.transform.localPosition=new Vector3(x-side*.28f,.19f,depth-1.55f);
    FloorArchitectureDetails.Plant(room,new Vector3(x-side*.28f,.64f,depth-1.55f),Surface(FloorArchitectureSurface.Leaf),Surface(FloorArchitectureSurface.LeafLight),side+29);
    FloorArchitectureDetails.Ivy(room,new Vector3(x-side*.12f,3.34f,depth-.81f),1.87f,side+71,Surface(FloorArchitectureSurface.Leaf),Surface(FloorArchitectureSurface.LeafLight));
   }
   for(int i=0;i<23;i++){
    float start=i*Mathf.PI/23+.003f,end=(i+1)*Mathf.PI/23-.003f;
    var arch=FloorArchitectureDetails.ArchStone(room,"GrandPortalVoussoir",start,end,radius,radius+.50f,spring,depth,.42f,trim);arch.transform.localPosition=Vector3.right*center;
    var gold=FloorArchitectureDetails.ArchStone(room,"PortalGildedMolding",start,end,radius-.045f,radius+.006f,spring,depth-.46f,.03f,brass);gold.transform.localPosition=Vector3.right*center;
    var inner=FloorArchitectureDetails.ArchStone(room,"DeepPortalSoffit",start,end,radius-.022f,radius+.018f,spring,depth+.49f,.49f,stone);inner.transform.localPosition=Vector3.right*center;
   }
   FloorArchitectureDetails.BeveledBlock(room,"PortalKeystone",new Vector3(center,spring+radius+.20f,depth-.51f),new Vector3(.42f,.59f,.18f),.06f,brass);
   FloorArchitectureDetails.BeveledBlock(room,"AtriumEntablature",new Vector3(center,6.94f,depth),new Vector3(11.8f,.49f,1.08f),.065f,trim);
   MeshArt.Box(room,"GildedCorniceRibbon",new Vector3(center,6.65f,depth-.56f),new Vector3(11.8f,.07f,.09f),brass);
   for(int tooth=0;tooth<31;tooth++)FloorArchitectureDetails.BeveledBlock(room,"AtriumCorniceDentil",new Vector3(center-5.63f+tooth*.375f,6.53f,depth-.54f),new Vector3(.12f,.19f,.22f),.018f,trim);
   // Long cloth and an original modeled lift emblem establish the lobby's identity.
   var cloth=MeshArt.Mesh(room,"AtriumLiftBanner",new[]{new Vector3(-.32f,.9f,0),new Vector3(.32f,.9f,0),new Vector3(.32f,-.72f,0),new Vector3(0,-1.02f,0),new Vector3(-.32f,-.72f,0)},new[]{0,1,3,1,2,3,0,3,4},null,teal);cloth.transform.localPosition=new Vector3(-2.76f,4.70f,2.60f);
   var emblem=new RunnerMeshBuilder(room,"AtriumBannerGilding",brass);var anchor=cloth.transform.localPosition+Vector3.back*.018f;
   foreach(int side in new[]{-1,1}){emblem.Stitch(anchor+new Vector3(side*.27f,.82f,0),anchor+new Vector3(side*.27f,-.70f,0),.012f);emblem.Stitch(anchor+new Vector3(side*.27f,-.70f,0),anchor+new Vector3(0,-.94f,0),.012f);emblem.Stitch(anchor+new Vector3(side*.12f,-.38f,-.014f),anchor+new Vector3(side*.12f,.20f,-.014f),.014f);}
   emblem.Stitch(anchor+new Vector3(-.27f,.82f,0),anchor+new Vector3(.27f,.82f,0),.012f);emblem.Stitch(anchor+new Vector3(-.16f,-.41f,-.014f),anchor+new Vector3(.16f,-.41f,-.014f),.017f);emblem.Stitch(anchor+new Vector3(-.16f,.22f,-.014f),anchor+new Vector3(.16f,.22f,-.014f),.017f);
   var bannerArch=new Vector3[13];var sizes=new float[13];for(int i=0;i<13;i++){float a=i*Mathf.PI/12;bannerArch[i]=anchor+new Vector3(Mathf.Cos(a)*.15f,.22f+Mathf.Sin(a)*.15f,-.014f);sizes[i]=.014f;}emblem.Sweep(bannerArch,sizes,sizes,5);emblem.Finish();
   MeshArt.Box(room,"AtriumContinuousMarbleBase",new Vector3(2.4f,.165f,-4.8f),new Vector3(18.4f,.024f,22.7f),stone);
   // Geometric stone inlays make the floor a room surface rather than a flat banner.
   for(int row=-10;row<5;row++)for(int col=0;col<12;col++){
    var tile=MeshArt.Box(room,"AtriumMarbleTile",new Vector3(-5.4f+col*1.6f,.185f,-.3f+row*1.5f),new Vector3(1.57f,.012f,1.47f),(row+col)%2==0?trim:stone);
    FloorArchitectureDetails.SurfaceMode(tile,.25f);
    if(row<3&&col%2==0){var mark=MeshArt.Box(room,"MarbleDiamondInlay",tile.transform.localPosition+Vector3.up*.012f,new Vector3(.21f,.011f,.21f),teal);mark.transform.localRotation=Quaternion.Euler(0,45,0);}
   }
  }
 }
}
