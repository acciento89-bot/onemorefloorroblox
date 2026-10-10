using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Presentation kit only. LandingSurface and ElevatorMotion remain the gameplay authority.
 public static class LiftArchitecture
 {
  public static readonly Color Ivory=new(.80f,.78f,.70f),Copper=new(.57f,.32f,.18f),Teal=new(.035f,.18f,.20f);
  public static void Cabin(Transform parent){var root=new GameObject("CopperLiftCabin").transform;root.SetParent(parent,false);var copper=MeshArt.Mat("CabinCopper",Copper);var brass=MeshArt.Mat("CabinBrass",new Color(.73f,.52f,.28f));var teal=MeshArt.Mat("CabinTeal",Teal);var glow=MeshArt.Mat("CabinWarmLamp",new Color(1,.70f,.24f),-1,true);
   // Open crown, rather than a solid roof, keeps the runner and next transfer visible.
   foreach(float y in new[]{.48f,.76f,3.20f,3.48f}){var ring=MeshArt.Lathe(root,"CabinCrown",new[]{-.09f,-.06f,.06f,.09f},new[]{1.96f,2.02f,2.02f,1.96f},32,copper,new Vector3(1,1,.80f));ring.transform.localPosition=new Vector3(0,y,0);}
   var lamp=MeshArt.Ring(root,"CabinCeilingGlow",1.91f,.08f,3.18f,glow,32);lamp.transform.localScale=new Vector3(1,1,.80f);
   for(int side=-1;side<=1;side+=2){MeshArt.Box(root,"CabinRearPost",new Vector3(side*1.72f,1.97f,.92f),new Vector3(.10f,2.66f,.10f),brass);MeshArt.Box(root,"CabinGuideShoe",new Vector3(side*1.82f,2.2f,1.15f),new Vector3(.25f,.42f,.24f),teal);MeshArt.Box(root,"CabinSideRail",new Vector3(side*1.92f,1.26f,0),new Vector3(.075f,.075f,2.5f),brass);
    for(int z=0;z<6;z++)MeshArt.Box(root,"CabinBaluster",new Vector3(side*1.92f,.96f,-1.2f+z*.48f),new Vector3(.04f,.59f,.04f),teal);
    // The front has a wide transfer opening and no decoration across the timing bay.
    MeshArt.Box(root,"CabinFrontHandrail",new Vector3(side*1.54f,1.26f,-1.27f),new Vector3(.75f,.075f,.075f),brass);
   }
   var crown=MeshArt.Lathe(root,"CabinDome",new[]{0f,.13f,.32f,.43f},new[]{.78f,.70f,.38f,.12f},24,teal,new Vector3(1,1,.8f));crown.transform.localPosition=new Vector3(0,3.50f,0);
   MeshArt.Box(root,"CabinBackPlaque",new Vector3(0,2.12f,1.54f),new Vector3(.55f,.70f,.04f),teal);MeshArt.Box(root,"CabinPlaqueMark",new Vector3(0,2.12f,1.51f),new Vector3(.055f,.38f,.025f),brass);
  }
  public static void Bay(Transform parent,int floor){var root=new GameObject("ArchitecturalFloorBay").transform;root.SetParent(parent,false);var stone=MeshArt.Mat("FacadeIvory",Ivory);var trim=MeshArt.Mat("FacadeTrim",new Color(.86f,.83f,.74f));var teal=MeshArt.Mat("FacadeTeal",Teal);var brass=MeshArt.Mat("FacadeCopper",Copper);var amber=MeshArt.Mat("FacadeLantern",new Color(1,.62f,.15f),-1,true);
   // Side wing stays beyond the +/-2.4m playable footprint. The open arch faces the route.
   root.localPosition=new Vector3(4.75f,0,1.15f);
   MeshArt.Box(root,"MasonryFacadeSupport",new Vector3(.45f,-4.65f,.52f),new Vector3(1.60f,10f,3.7f),stone);
   MeshArt.Box(root,"BalconyPlinth",new Vector3(0,.08f,0),new Vector3(2.55f,.65f,2.7f),stone);MeshArt.Box(root,"BalconyCornice",new Vector3(0,.43f,-.10f),new Vector3(2.74f,.14f,2.8f),trim);
   for(int side=-1;side<=1;side+=2){MeshArt.Box(root,"FacadeColumn",new Vector3(side*1.1f,1.92f,.72f),new Vector3(.29f,3.0f,.42f),stone);for(float y=.65f;y<3.4f;y+=.65f)MeshArt.Box(root,"ColumnMasonryJoint",new Vector3(side*1.1f,y,.49f),new Vector3(.32f,.035f,.03f),trim);MeshArt.Box(root,"ColumnCapital",new Vector3(side*1.1f,3.30f,.72f),new Vector3(.48f,.18f,.58f),trim);}
   for(int segment=0;segment<12;segment++){float a=(segment+.5f)*Mathf.PI/12;var voussoir=MeshArt.Box(root,"ArchVoussoir",new Vector3(Mathf.Cos(a)*1.09f,2.93f+Mathf.Sin(a)*.88f,.72f),new Vector3(.33f,.30f,.48f),trim);voussoir.transform.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg-90);}
   MeshArt.Box(root,"BayCeilingCornice",new Vector3(0,3.91f,.85f),new Vector3(2.72f,.24f,1.3f),stone);MeshArt.Box(root,"RecessedTealDoor",new Vector3(0,1.68f,1.38f),new Vector3(1.28f,2.45f,.10f),teal);
   MeshArt.Box(root,"BalconyHandrail",new Vector3(0,1.15f,-1.18f),new Vector3(2.45f,.075f,.075f),brass);for(int i=0;i<9;i++)MeshArt.Box(root,"BalconyBaluster",new Vector3(-1.12f+i*.28f,.81f,-1.18f),new Vector3(.045f,.61f,.045f),teal);
   MeshArt.Box(root,"FloorBanner",new Vector3(.52f,2.45f,1.30f),new Vector3(.63f,.85f,.06f),teal);var label=new GameObject("ArchitecturalFloorNumber",typeof(TextMesh));label.transform.SetParent(root,false);label.transform.localPosition=new Vector3(.52f,2.45f,1.25f);label.transform.localRotation=Quaternion.identity;var text=label.GetComponent<TextMesh>();text.text=(floor+1).ToString();text.anchor=TextAnchor.MiddleCenter;text.fontSize=36;text.characterSize=.12f;text.color=new Color(1,.85f,.56f);
   for(int side=-1;side<=1;side+=2){MeshArt.Box(root,"LanternHousing",new Vector3(side*.93f,2.02f,.33f),new Vector3(.24f,.38f,.24f),brass);MeshArt.Box(root,"LanternGlass",new Vector3(side*.93f,2.02f,.18f),new Vector3(.14f,.25f,.055f),amber);Planter(root,new Vector3(side*.96f,.53f,-.76f));}
  }
  public static void Rails(Transform parent){var teal=MeshArt.Mat("FacadeTeal",Teal);var brass=MeshArt.Mat("CabinBrass",new Color(.73f,.52f,.28f));for(int side=-1;side<=1;side+=2){MeshArt.Box(parent,"VerticalLiftRail",new Vector3(side*2.15f,1.75f,1.85f),new Vector3(.14f,6.2f,.18f),teal);MeshArt.Box(parent,"CopperRailGuide",new Vector3(side*2.15f,1.75f,1.73f),new Vector3(.045f,6.2f,.035f),brass);for(int k=0;k<6;k++)MeshArt.Box(parent,"RailMount",new Vector3(side*2.15f,-.9f+k*1.05f,1.94f),new Vector3(.32f,.12f,.29f),brass);}}
  static void Planter(Transform parent,Vector3 at){var pot=MeshArt.Lathe(parent,"StonePlanter",new[]{0f,.08f,.42f,.48f},new[]{.17f,.19f,.25f,.28f},12,MeshArt.Mat("PlanterStone",new Color(.67f,.52f,.32f)),Vector3.one);pot.transform.localPosition=at;var leaf=MeshArt.Mat("LobbyFoliage",new Color(.12f,.28f,.14f));for(int i=0;i<5;i++){var sprig=MeshArt.Oval(parent,"PlantLeaf",at+new Vector3(Mathf.Sin(i*2.4f)*.22f,.55f+i*.045f,Mathf.Cos(i*2.4f)*.22f),new Vector3(.24f,.42f,.12f),leaf,8);sprig.transform.localRotation=Quaternion.Euler(i*11,i*137,25);}}
 }
}
