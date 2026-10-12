using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Shared authored carriage and tower surfaces; FloorCourse owns gameplay colliders.
 public static class LiftArchitecture
 {
  public const float FacadeWidthScale=1.65f,FacadeOffset=.72f;
  public static readonly Color Ivory=new(.80f,.78f,.70f),Copper=new(.57f,.32f,.18f),Teal=new(.035f,.18f,.20f);
  static Material Finish(Transform owner,FloorArchitectureSurface surface)=>FloorArchitecturePalette.Get(owner,surface);
  public static void Cabin(Transform parent)
  {
   var root=new GameObject("CopperLiftCabin").transform;root.SetParent(parent,false);
   var copper=Finish(parent,FloorArchitectureSurface.Copper);var brass=Finish(parent,FloorArchitectureSurface.Brass);var teal=Finish(parent,FloorArchitectureSurface.Teal);var glow=Finish(parent,FloorArchitectureSurface.Lamp);
   // The narrow carriage keeps its substantial lower metal body below the original deck.
   CabinAuthoredGeometry.Profile(root,"CabinBaseDrum",new[]{new Vector2(.86f,-.30f),new Vector2(1.01f,-.30f),new Vector2(1.085f,-.255f),new Vector2(1.12f,-.19f),new Vector2(1.105f,-.12f),new Vector2(1.10f,.08f),new Vector2(1.115f,.13f),new Vector2(1.115f,.41f),new Vector2(1.10f,.46f),new Vector2(1.12f,.51f),new Vector2(1.09f,.565f),new Vector2(1.035f,.59f),new Vector2(.86f,.59f)},0,360,48,brass,Vector3.zero);
   MeshArt.Lathe(root,"CabinBaseCoping",new[]{.52f,.54f,.60f,.62f},new[]{1.12f,1.16f,1.16f,1.12f},40,copper,new Vector3(1,1,.80f));
   MeshArt.Lathe(root,"CabinUnderbody",new[]{-.54f,-.44f,-.30f},new[]{.29f,.64f,1.01f},32,teal,new Vector3(1,1,.80f));
   CabinAuthoredGeometry.Profile(root,"CabinCrown",new[]{new Vector2(.94f,0),new Vector2(1.06f,0),new Vector2(1.115f,.025f),new Vector2(1.14f,.065f),new Vector2(1.11f,.09f),new Vector2(1.105f,.205f),new Vector2(1.135f,.23f),new Vector2(1.14f,.265f),new Vector2(1.085f,.295f),new Vector2(.98f,.295f),new Vector2(.94f,.265f)},0,360,48,brass,Vector3.up*3.43f);
   CabinAuthoredGeometry.Profile(root,"CabinLanternDrum",new[]{new Vector2(.97f,0),new Vector2(1.09f,0),new Vector2(1.115f,.035f),new Vector2(1.08f,.075f),new Vector2(1.08f,.18f),new Vector2(1.115f,.205f),new Vector2(.97f,.205f)},0,360,24,brass,Vector3.up*3.255f);
   CabinAuthoredGeometry.Profile(root,"CabinDome",new[]{new Vector2(0,0),new Vector2(1.04f,0),new Vector2(1.085f,.015f),new Vector2(1.10f,.033f),new Vector2(1.04f,.05f),new Vector2(.90f,.064f),new Vector2(.30f,.075f),new Vector2(0,.075f)},0,360,48,brass,Vector3.up*3.66f);
   MeshArt.Oval(root,"CabinRoofFinial",new Vector3(0,3.76f,0),new Vector3(.10f,.08f,.10f),brass,10);
   // The Concept carriage has a central drive collar and machined roof bands.
   // This shallow rearward mechanism leaves the existing open head space intact.
   CabinAuthoredGeometry.Profile(root,"CabinDriveCollar",new[]{new Vector2(.25f,0),new Vector2(.42f,0),new Vector2(.44f,.025f),new Vector2(.40f,.05f),new Vector2(.36f,.05f),new Vector2(.36f,.15f),new Vector2(.33f,.18f),new Vector2(.25f,.18f)},0,360,32,teal,new Vector3(0,3.74f,.69f));
   CabinAuthoredGeometry.Profile(root,"CabinDriveGilding",new[]{new Vector2(.37f,0),new Vector2(.405f,0),new Vector2(.419f,.012f),new Vector2(.405f,.033f),new Vector2(.37f,.033f)},0,360,32,brass,new Vector3(0,3.82f,.69f));
   // Low shaped brass body now rises above the broad deck instead of disappearing below it.
   // The 110-degree front entrance is open; no panel crosses the runner/transfer centre.
   CabinAuthoredGeometry.Profile(root,"CabinPassengerSkirt",new[]{new Vector2(1.045f,.665f),new Vector2(1.085f,.665f),new Vector2(1.13f,.70f),new Vector2(1.135f,.745f),new Vector2(1.11f,.78f),new Vector2(1.105f,1.03f),new Vector2(1.13f,1.065f),new Vector2(1.115f,1.10f),new Vector2(1.065f,1.115f),new Vector2(1.04f,1.075f),new Vector2(1.04f,.71f)},-35,215,48,brass,Vector3.zero);
   var railSection=new Vector2[10];for(int i=0;i<railSection.Length;i++){float a=i*Mathf.PI*2/railSection.Length;railSection[i]=new Vector2(1.075f+Mathf.Cos(a)*.035f,1.24f+Mathf.Sin(a)*.035f);}CabinAuthoredGeometry.Profile(root,"CabinCurvedPassengerRail",railSection,-35,215,48,brass,Vector3.zero);
   CabinAuthoredGeometry.Rivets(root,copper);
   CabinAuthoredGeometry.Profile(root,"CabinCeilingGlow",new[]{new Vector2(.97f,-.015f),new Vector2(1.085f,-.015f),new Vector2(1.085f,.015f),new Vector2(.97f,.015f)},0,360,40,glow,Vector3.up*3.42f);
   CabinAuthoredGeometry.Profile(root,"CabinLanternLightBand",new[]{new Vector2(1.095f,0),new Vector2(1.126f,0),new Vector2(1.126f,.033f),new Vector2(1.095f,.033f)},0,360,24,glow,Vector3.up*3.275f);
   var diffuser=MeshArt.Ring(root,"CabinCeilingDiffuser",1.005f,.18f,0,glow,40);diffuser.transform.localScale=new Vector3(1,1,.80f);diffuser.transform.localRotation=Quaternion.Euler(180,0,0);diffuser.transform.localPosition=Vector3.up*3.415f;
   // The emissive diffuser and a real overhead Point Light illuminate the passenger.
   FloorConceptLighting.CabinLamp(root);
   var ceiling=new MaterialPropertyBlock();ceiling.SetColor("_EmissionColor",new Color(2.6f,1.8f,.65f));diffuser.GetComponent<MeshRenderer>().SetPropertyBlock(ceiling);
   // Turned posts have rounded shoulders and broad feet; they remain at the existing side/rear positions.
   for(int side=-1;side<=1;side+=2){
    var postSection=new[]{new Vector2(0,-1.44f),new Vector2(.053f,-1.44f),new Vector2(.053f,-1.36f),new Vector2(.039f,-1.31f),new Vector2(.039f,1.30f),new Vector2(.050f,1.35f),new Vector2(.050f,1.44f),new Vector2(0,1.44f)};
    CabinAuthoredGeometry.Profile(root,"CabinRearPost",postSection,0,360,16,brass,new Vector3(side*.85f,2.04f,.58f));
    CabinAuthoredGeometry.Profile(root,"CabinSideMullion",postSection,0,360,16,brass,new Vector3(side*1.04f,2.03f,-.19f));
    MeshArt.Box(root,"CabinGuideShoe",new Vector3(side*.94f,2.16f,.69f),new Vector3(.15f,.23f,.16f),teal);
    for(int z=0;z<3;z++)CabinAuthoredGeometry.Profile(root,"CabinBaluster",new[]{new Vector2(0,-.12f),new Vector2(.014f,-.12f),new Vector2(.014f,.12f),new Vector2(0,.12f)},0,360,10,brass,new Vector3(side*1.06f,1.12f,-.08f+z*.22f));
   }
   FloorArchitectureDetails.CabinGlass(root,Finish(parent,FloorArchitectureSurface.Glass));
   LiftMetalOrnaments.Cabin(root,brass,copper);MeshArt.Box(root,"CabinBackPlaque",new Vector3(0,1.99f,.78f),new Vector3(.48f,.59f,.035f),teal);MeshArt.Box(root,"CabinPlaqueMark",new Vector3(0,1.99f,.758f),new Vector3(.037f,.31f,.017f),brass);
  }
  public static void LandingDeck(Transform parent,bool highContrast=false)
  {
   FloorArchitectureDetails.BeveledBlock(parent,"TowerLandingDeck",new Vector3(0,.575f,-.20f),new Vector3(2.55f,.05f,2.70f),.008f,Finish(parent,FloorArchitectureSurface.Deck));var glow=Finish(parent,FloorArchitectureSurface.Lamp);
   var contrast=new GameObject("RouteContrastRim");contrast.transform.SetParent(parent,false);var white=MeshArt.Mat("ContrastIvory",Color.white);
   for(int side=-1;side<=1;side+=2){MeshArt.Box(parent,"TowerLandingGoldRim",new Vector3(side*1.255f,.607f,-.2f),new Vector3(.035f,.012f,2.67f),glow);MeshArt.Box(parent,"TowerLandingGoldRim",new Vector3(0,.607f,-.20f+side*1.335f),new Vector3(2.55f,.012f,.035f),glow);MeshArt.Box(contrast.transform,"ReadableBorder",new Vector3(side*1.255f,.627f,-.2f),new Vector3(.06f,.025f,2.67f),white);MeshArt.Box(contrast.transform,"ReadableBorder",new Vector3(0,.627f,-.20f+side*1.335f),new Vector3(2.55f,.025f,.06f),white);}contrast.SetActive(highContrast);
  }
  public static void Bay(Transform parent,int floor)
  {
   var root=new GameObject("ArchitecturalFloorBay").transform;root.SetParent(parent,false);root.localPosition=new Vector3(4.75f,0,1.15f);
   var stone=Finish(parent,FloorArchitectureSurface.Stone);var trim=Finish(parent,FloorArchitectureSurface.Trim);var teal=Finish(parent,FloorArchitectureSurface.Teal);var brass=Finish(parent,FloorArchitectureSurface.Brass);var lamp=Finish(parent,FloorArchitectureSurface.Lamp);
   // Both the structural support and thin interior wall sit behind the visible door.
   FloorArchitectureDetails.BeveledBlock(root,"MasonryFacadeSupport",new Vector3(.45f,-1.83f,3.60f),new Vector3(1.60f,4.6f,1.70f),.045f,stone);
   FloorArchitectureDetails.BeveledBlock(root,"BayRearWall",new Vector3(0,2.10f,2.90f),new Vector3(2.75f,3.45f,.28f),.045f,stone);
   FloorArchitectureDetails.BeveledBlock(root,"BalconyPlinth",new Vector3(0,.08f,0),new Vector3(2.55f,.65f,2.7f),.045f,stone);FloorArchitectureDetails.BeveledBlock(root,"BalconyCornice",new Vector3(0,.43f,-.10f),new Vector3(2.74f,.14f,2.8f),.045f,trim);
   for(int side=-1;side<=1;side+=2){
    FloorArchitectureDetails.BeveledBlock(root,"FacadeColumn",new Vector3(side*1.10f,1.82f,.72f),new Vector3(.30f,2.81f,.42f),.045f,stone);
    FloorArchitectureDetails.BeveledBlock(root,"ColumnFoot",new Vector3(side*1.10f,.54f,.72f),new Vector3(.43f,.22f,.53f),.045f,trim);FloorArchitectureDetails.BeveledBlock(root,"ColumnCapital",new Vector3(side*1.10f,3.10f,.72f),new Vector3(.46f,.19f,.55f),.045f,trim);
    for(int flute=0;flute<3;flute++)MeshArt.Box(root,"PilasterFlute",new Vector3(side*1.10f+(flute-1)*.072f,1.81f,.496f),new Vector3(.022f,2.11f,.012f),trim);
    for(int row=0;row<4;row++)MeshArt.Box(root,"MasonryCourse",new Vector3(side*1.30f,.92f+row*.65f,1.20f),new Vector3(.25f,.022f,.39f),trim);
   }
   for(int segment=0;segment<13;segment++){
    FloorArchitectureDetails.ArchStone(root,"ArchVoussoir",segment*Mathf.PI/13+.009f,(segment+1)*Mathf.PI/13-.009f,.96f,1.28f,2.82f,.72f,.24f,trim);
    FloorArchitectureDetails.ArchStone(root,"BayVaultedSoffit",segment*Mathf.PI/13,(segment+1)*Mathf.PI/13,.960f,1.02f,2.82f,1.70f,.94f,stone);
    FloorArchitectureDetails.ArchStone(root,"ArchInnerMolding",segment*Mathf.PI/13,(segment+1)*Mathf.PI/13,.925f,.957f,2.82f,.462f,.025f,brass);
   }
   FloorArchitectureDetails.BeveledBlock(root,"BayCeilingCornice",new Vector3(0,4.13f,.99f),new Vector3(2.83f,.22f,1.27f),.045f,stone);MeshArt.Box(root,"CorniceDentilRail",new Vector3(0,3.99f,.38f),new Vector3(2.69f,.055f,.085f),trim);
   for(int tooth=0;tooth<10;tooth++)MeshArt.Box(root,"CorniceDentil",new Vector3(-1.19f+tooth*.265f,3.93f,.40f),new Vector3(.09f,.10f,.12f),trim);
   TowerEntranceArt.Build(root,Finish(parent,FloorArchitectureSurface.DoorWood),brass);
   var transom=new Vector3[20];var transomTriangles=new int[18*3];transom[0]=new Vector3(0,2.82f,1.53f);
   for(int i=0;i<=18;i++){float a=i*Mathf.PI/18;transom[i+1]=new Vector3(Mathf.Cos(a)*.91f,2.82f+Mathf.Sin(a)*.91f,1.53f);if(i<18){transomTriangles[i*3]=0;transomTriangles[i*3+1]=i+2;transomTriangles[i*3+2]=i+1;}}
   var fan=MeshArt.Mesh(root,"ArchedTransomRecess",transom,transomTriangles,null,Finish(parent,FloorArchitectureSurface.DoorWood));FloorArchitectureDetails.SurfaceMode(fan,.5f);
   for(int ray=0;ray<5;ray++){float a=(ray+1)*Mathf.PI/6;var mullion=MeshArt.Box(root,"TransomFanMullion",new Vector3(Mathf.Cos(a)*.34f,2.85f+Mathf.Sin(a)*.34f,1.506f),new Vector3(.025f,.70f,.027f),brass);mullion.transform.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg-90);}
   MeshArt.Box(root,"BalconyHandrail",new Vector3(0,1.15f,-1.18f),new Vector3(2.45f,.049f,.055f),brass);for(int i=0;i<9;i++)MeshArt.Box(root,"BalconyBaluster",new Vector3(-1.12f+i*.28f,.81f,-1.18f),new Vector3(.027f,.61f,.027f),teal);
   var bannerPoints=new[]{new Vector3(-.29f,.62f,0),new Vector3(.29f,.62f,0),new Vector3(.29f,-.43f,0),new Vector3(0,-.70f,.018f),new Vector3(-.29f,-.43f,0)};var banner=MeshArt.Mesh(root,"FloorBanner",bannerPoints,new[]{0,1,3,1,2,3,0,3,4},null,teal);var edgeVertices=new Vector3[20];var edgeTriangles=new int[30];for(int edge=0;edge<5;edge++){int next=(edge+1)%5;var a=bannerPoints[edge];var b=bannerPoints[next];edgeVertices[edge*4]=a+Vector3.back*.008f;edgeVertices[edge*4+1]=b+Vector3.back*.008f;edgeVertices[edge*4+2]=Vector3.Scale(b,new Vector3(.94f,.97f,1))+Vector3.back*.01f;edgeVertices[edge*4+3]=Vector3.Scale(a,new Vector3(.94f,.97f,1))+Vector3.back*.01f;int k=edge*4;for(int triangle=0;triangle<6;triangle++)edgeTriangles[edge*6+triangle]=k+new[]{0,1,2,0,2,3}[triangle];}var stitched=MeshArt.Mesh(root,"BannerGoldBorder",edgeVertices,edgeTriangles,null,brass);stitched.transform.localPosition=new Vector3(-.68f,2.53f,.34f);banner.transform.localPosition=new Vector3(-.68f,2.53f,.34f);MeshArt.Box(root,"BannerBrassRail",new Vector3(-.68f,3.18f,.34f),new Vector3(.64f,.04f,.06f),brass);var label=new GameObject("ArchitecturalFloorNumber",typeof(TextMesh));label.transform.SetParent(root,false);label.transform.localPosition=new Vector3(-.68f,2.53f,.305f);label.transform.localRotation=Quaternion.identity;var text=label.GetComponent<TextMesh>();text.text=(floor+1).ToString();text.anchor=TextAnchor.MiddleCenter;text.fontSize=36;text.characterSize=.18f;text.color=new Color(1,.85f,.56f);
   for(int side=-1;side<=1;side+=2){Lantern(root,new Vector3(side*.90f,2.02f,.32f),brass,lamp);Planter(root,parent,new Vector3(side*.98f,.52f,-.72f),floor+side);}
   for(int side=-1;side<=1;side+=2){FloorArchitectureDetails.BeveledBlock(root,"BalconyCarvedCorbel",new Vector3(side*.92f,-.43f,.18f),new Vector3(.24f,.56f,1.65f),.045f,trim);FloorArchitectureDetails.BeveledBlock(root,"CorbelCap",new Vector3(side*.92f,-.18f,-.12f),new Vector3(.33f,.14f,2.01f),.028f,trim);}
   // A continuous right-hand residence mass gives the balcony a real building.
   // It is scenery beside the unchanged landing, not a second target collider.
   MeshArt.Box(root,"ResidentialTowerWing",new Vector3(2.75f,2.34f,1.40f),new Vector3(2.9f,4.6f,3.20f),stone);
   MeshArt.Box(root,"ResidenceBeltCornice",new Vector3(2.75f,4.02f,-.28f),new Vector3(2.95f,.16f,.38f),trim);
   MeshArt.Box(root,"ResidenceLowerCornice",new Vector3(2.75f,.46f,-.28f),new Vector3(2.95f,.12f,.38f),trim);
   for(int side=-1;side<=1;side+=2)MeshArt.Box(root,"VaultedEntrySideWall",new Vector3(side*.95f,1.70f,1.66f),new Vector3(.16f,2.3f,1.7f),stone);
   MeshArt.Box(root,"EntryMarbleThreshold",new Vector3(0,.57f,1.63f),new Vector3(1.84f,.09f,1.75f),trim);
   var interior=MeshArt.Box(root,"InteriorMarbleFloor",new Vector3(0,.57f,2.01f),new Vector3(1.84f,.07f,1.60f),trim);FloorArchitectureDetails.SurfaceMode(interior,.25f);
   var ceiling=MeshArt.Ring(root,"EntryCeilingLamp",.27f,.23f,0,lamp,16);ceiling.transform.localRotation=Quaternion.Euler(180,0,0);ceiling.transform.localPosition=new Vector3(0,3.38f,1.95f);
   foreach(Transform part in root)if(part.name.StartsWith("Door")||part.name=="RecessedTealDoor"||part.name=="ArchedTransomRecess"||part.name=="TransomFanMullion")part.localPosition+=Vector3.forward;
   LiftMetalOrnaments.Balcony(root,teal,brass);var shade=Finish(parent,FloorArchitectureSurface.Leaf);var leafLight=Finish(parent,FloorArchitectureSurface.LeafLight);
   FloorArchitectureDetails.Ivy(root,new Vector3(1.26f,3.87f,.29f),1.64f,floor+19,shade,leafLight);FloorArchitectureDetails.Ivy(root,new Vector3(-1.14f,.47f,-1.19f),1.45f,floor+71,shade,leafLight);
   // Expand the entrance toward the right, preserving the left jump edge and
   // authoritative narrow landing. The wider facade is foreground scenery.
   foreach(Transform part in root){part.localPosition+=Vector3.right*(FacadeOffset/FacadeWidthScale);if(part.GetComponent<TextMesh>()!=null)part.localScale=new Vector3(1/FacadeWidthScale,1,1);}root.localScale=new Vector3(FacadeWidthScale,1,1);
  }
  static void Lantern(Transform root,Vector3 at,Material brass,Material lamp)
  {
   var halo=MeshArt.Mesh(root,"WarmLanternHalo",new[]{new Vector3(-.42f,-.52f,0),new Vector3(.42f,-.52f,0),new Vector3(.42f,.52f,0),new Vector3(-.42f,.52f,0)},new[]{0,2,1,0,3,2},new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},Finish(root,FloorArchitectureSurface.Halo));halo.transform.localPosition=at+Vector3.forward*.12f;
   MeshArt.Box(root,"LanternWallMount",at+Vector3.forward*.11f,new Vector3(.17f,.31f,.075f),brass);MeshArt.Box(root,"LanternGlass",at+Vector3.back*.045f,new Vector3(.13f,.26f,.11f),lamp);
   for(int side=-1;side<=1;side+=2)MeshArt.Box(root,"LanternHousing",at+new Vector3(side*.084f,0,-.055f),new Vector3(.025f,.32f,.14f),brass);
   MeshArt.Lathe(root,"LanternCap",new[]{0f,.06f,.14f},new[]{.14f,.14f,.026f},8,brass,Vector3.one).transform.localPosition=at+Vector3.up*.16f;
   MeshArt.Box(root,"LanternBase",at+Vector3.down*.16f,new Vector3(.21f,.044f,.17f),brass);
  }
  public static void Rails(Transform parent)
  {
   var teal=Finish(parent,FloorArchitectureSurface.Teal);var brass=Finish(parent,FloorArchitectureSurface.Brass);
   var guides=new GameObject("CopperLiftGuides").transform;guides.SetParent(parent,false);LiftGuideArt.Build(guides,-3,8,teal,brass,"VerticalLiftRail");
  }
  static void Planter(Transform parent,Transform owner,Vector3 at,int seed)
  {
   var pot=MeshArt.Lathe(parent,"StonePlanter",new[]{0f,.06f,.32f,.39f},new[]{.17f,.19f,.25f,.28f},16,Finish(owner,FloorArchitectureSurface.Stone),Vector3.one);pot.transform.localPosition=at;
   FloorArchitectureDetails.Plant(parent,at,Finish(owner,FloorArchitectureSurface.Leaf),Finish(owner,FloorArchitectureSurface.LeafLight),seed);
  }
 }
}
