using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Decoration only: the original deck collider, transfer rules and camera are authoritative.
 public static class LiftArchitecture
 {
  public static readonly Color Ivory=new(.80f,.78f,.70f),Copper=new(.57f,.32f,.18f),Teal=new(.035f,.18f,.20f);
  static Material Finish(Transform owner,FloorArchitectureSurface surface)=>FloorArchitecturePalette.Get(owner,surface);
  public static void Cabin(Transform parent)
  {
   var root=new GameObject("CopperLiftCabin").transform;root.SetParent(parent,false);
   var copper=Finish(parent,FloorArchitectureSurface.Copper);var brass=Finish(parent,FloorArchitectureSurface.Brass);var teal=Finish(parent,FloorArchitectureSurface.Teal);var glow=Finish(parent,FloorArchitectureSurface.Lamp);
   // The narrow carriage keeps its substantial lower metal body below the original deck.
   MeshArt.Lathe(root,"CabinBaseDrum",new[]{-.30f,-.26f,-.21f,-.16f,-.10f,.06f,.12f,.41f,.47f,.52f,.57f,.59f},new[]{1.01f,1.06f,1.10f,1.12f,1.10f,1.10f,1.115f,1.115f,1.10f,1.12f,1.07f,1.04f},40,brass,new Vector3(1,1,.80f));
   MeshArt.Lathe(root,"CabinBaseCoping",new[]{.52f,.54f,.60f,.62f},new[]{1.12f,1.16f,1.16f,1.12f},40,copper,new Vector3(1,1,.80f));
   MeshArt.Lathe(root,"CabinUnderbody",new[]{-.54f,-.44f,-.30f},new[]{.29f,.64f,1.01f},32,teal,new Vector3(1,1,.80f));
   var crown=MeshArt.Lathe(root,"CabinCrown",new[]{0f,.025f,.055f,.08f,.20f,.235f,.275f,.295f,.275f,.025f,0f},new[]{1.06f,1.12f,1.13f,1.105f,1.105f,1.13f,1.13f,1.065f,.98f,.98f,1.06f},40,brass,new Vector3(1,1,.80f));crown.transform.localPosition=Vector3.up*3.43f;
   var roof=MeshArt.Lathe(root,"CabinDome",new[]{0f,.025f,.065f,.075f},new[]{1.045f,1.085f,.88f,.075f},40,brass,new Vector3(1,1,.80f));roof.transform.localPosition=Vector3.up*3.66f;
   MeshArt.Oval(root,"CabinRoofFinial",new Vector3(0,3.76f,0),new Vector3(.10f,.08f,.10f),brass,10);
   var lamp=MeshArt.Ring(root,"CabinCeilingGlow",1.02f,.035f,0,glow,40);lamp.transform.localScale=new Vector3(1,1,.80f);lamp.transform.localRotation=Quaternion.Euler(180,0,0);lamp.transform.localPosition=Vector3.up*3.42f;
   var diffuser=MeshArt.Ring(root,"CabinCeilingDiffuser",1.005f,.84f,0,glow,40);diffuser.transform.localScale=new Vector3(1,1,.80f);diffuser.transform.localRotation=Quaternion.Euler(180,0,0);diffuser.transform.localPosition=Vector3.up*3.415f;
   // A lit underside surface gives the cabin its warm ceiling without a new Light.
   var ceiling=new MaterialPropertyBlock();ceiling.SetColor("_EmissionColor",new Color(1.20f,.86f,.37f));diffuser.GetComponent<MeshRenderer>().SetPropertyBlock(ceiling);
   // Slender connected mullions leave the whole forward avatar/transfer opening clear.
   for(int side=-1;side<=1;side+=2){
    MeshArt.Box(root,"CabinRearPost",new Vector3(side*.85f,2.04f,.58f),new Vector3(.068f,2.88f,.068f),brass);
    MeshArt.Box(root,"CabinSideMullion",new Vector3(side*1.04f,2.03f,-.19f),new Vector3(.061f,2.86f,.061f),brass);
    MeshArt.Box(root,"CabinGuideShoe",new Vector3(side*.94f,2.16f,.69f),new Vector3(.15f,.23f,.16f),teal);
    MeshArt.Box(root,"CabinSideRail",new Vector3(side*1.06f,1.24f,0),new Vector3(.055f,.055f,.82f),brass);
    for(int z=0;z<6;z++)MeshArt.Box(root,"CabinBaluster",new Vector3(side*1.06f,.94f,-.35f+z*.14f),new Vector3(.027f,.55f,.027f),teal);
    MeshArt.Box(root,"CabinFrontHandrail",new Vector3(side*.79f,1.24f,-.43f),new Vector3(.36f,.055f,.055f),brass);
   }
   FloorArchitectureDetails.CabinGlass(root,Finish(parent,FloorArchitectureSurface.Glass));
   for(int panel=0;panel<12;panel++){
    float a=panel*Mathf.PI/6;var p=new Vector3(Mathf.Cos(a)*1.126f,.175f,Mathf.Sin(a)*.901f);
    var molding=MeshArt.Box(root,"DrumPanelMullion",p,new Vector3(.027f,.50f,.026f),copper);molding.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);
    var trim=MeshArt.Box(root,"CrownPanelMullion",new Vector3(Mathf.Cos(a)*1.127f,3.547f,Mathf.Sin(a)*.902f),new Vector3(.025f,.13f,.025f),copper);trim.transform.localRotation=molding.transform.localRotation;
   }
   MeshArt.Box(root,"CabinBackPlaque",new Vector3(0,1.99f,.78f),new Vector3(.48f,.59f,.035f),teal);MeshArt.Box(root,"CabinPlaqueMark",new Vector3(0,1.99f,.758f),new Vector3(.037f,.31f,.017f),brass);
  }
  public static void Bay(Transform parent,int floor)
  {
   var root=new GameObject("ArchitecturalFloorBay").transform;root.SetParent(parent,false);root.localPosition=new Vector3(4.75f,0,1.15f);
   var stone=Finish(parent,FloorArchitectureSurface.Stone);var trim=Finish(parent,FloorArchitectureSurface.Trim);var teal=Finish(parent,FloorArchitectureSurface.Teal);var brass=Finish(parent,FloorArchitectureSurface.Brass);var lamp=Finish(parent,FloorArchitectureSurface.Lamp);
   // Both the structural support and thin interior wall sit behind the visible door.
   FloorArchitectureDetails.BeveledBlock(root,"MasonryFacadeSupport",new Vector3(.45f,-4.65f,2.25f),new Vector3(1.60f,10f,1.70f),.045f,stone);
   FloorArchitectureDetails.BeveledBlock(root,"BayRearWall",new Vector3(0,2.10f,1.90f),new Vector3(2.75f,3.45f,.28f),.045f,stone);
   FloorArchitectureDetails.BeveledBlock(root,"BalconyPlinth",new Vector3(0,.08f,0),new Vector3(2.55f,.65f,2.7f),.045f,stone);FloorArchitectureDetails.BeveledBlock(root,"BalconyCornice",new Vector3(0,.43f,-.10f),new Vector3(2.74f,.14f,2.8f),.045f,trim);
   for(int side=-1;side<=1;side+=2){
    FloorArchitectureDetails.BeveledBlock(root,"FacadeColumn",new Vector3(side*1.10f,1.82f,.72f),new Vector3(.30f,2.81f,.42f),.045f,stone);
    FloorArchitectureDetails.BeveledBlock(root,"ColumnFoot",new Vector3(side*1.10f,.54f,.72f),new Vector3(.43f,.22f,.53f),.045f,trim);FloorArchitectureDetails.BeveledBlock(root,"ColumnCapital",new Vector3(side*1.10f,3.10f,.72f),new Vector3(.46f,.19f,.55f),.045f,trim);
    for(int flute=0;flute<3;flute++)MeshArt.Box(root,"PilasterFlute",new Vector3(side*1.10f+(flute-1)*.072f,1.81f,.496f),new Vector3(.022f,2.11f,.012f),trim);
    for(int row=0;row<4;row++)MeshArt.Box(root,"MasonryCourse",new Vector3(side*1.30f,.92f+row*.65f,1.20f),new Vector3(.25f,.022f,.39f),trim);
   }
   for(int segment=0;segment<13;segment++){
    FloorArchitectureDetails.ArchStone(root,"ArchVoussoir",segment*Mathf.PI/13+.009f,(segment+1)*Mathf.PI/13-.009f,.96f,1.28f,2.82f,.72f,.24f,trim);
    FloorArchitectureDetails.ArchStone(root,"ArchInnerMolding",segment*Mathf.PI/13,(segment+1)*Mathf.PI/13,.925f,.957f,2.82f,.462f,.025f,brass);
   }
   FloorArchitectureDetails.BeveledBlock(root,"BayCeilingCornice",new Vector3(0,4.13f,.99f),new Vector3(2.83f,.22f,1.27f),.045f,stone);MeshArt.Box(root,"CorniceDentilRail",new Vector3(0,3.99f,.38f),new Vector3(2.69f,.055f,.085f),trim);
   for(int tooth=0;tooth<10;tooth++)MeshArt.Box(root,"CorniceDentil",new Vector3(-1.19f+tooth*.265f,3.93f,.40f),new Vector3(.09f,.10f,.12f),trim);
   FloorArchitectureDetails.BeveledBlock(root,"RecessedTealDoor",new Vector3(0,1.68f,1.38f),new Vector3(1.28f,2.45f,.10f),.045f,teal);
   for(int side=-1;side<=1;side+=2)MeshArt.Box(root,"DoorFrame",new Vector3(side*.69f,1.68f,1.30f),new Vector3(.08f,2.57f,.12f),brass);
   MeshArt.Box(root,"DoorLintel",new Vector3(0,2.96f,1.30f),new Vector3(1.46f,.08f,.12f),brass);
   for(int row=0;row<2;row++)for(int side=-1;side<=1;side+=2){
    var p=new Vector3(side*.31f,1.02f+row*1.04f,1.319f);FloorArchitectureDetails.BeveledBlock(root,"DoorRecessedPanel",p,new Vector3(.48f,.81f,.025f),.045f,Finish(parent,FloorArchitectureSurface.Copper));
    MeshArt.Box(root,"DoorPanelInset",p+Vector3.back*.019f,new Vector3(.41f,.74f,.013f),teal);
   }
   MeshArt.Box(root,"DoorCenterMullion",new Vector3(0,1.68f,1.305f),new Vector3(.035f,2.41f,.035f),brass);MeshArt.Oval(root,"DoorBrassHandle",new Vector3(.13f,1.63f,1.275f),new Vector3(.045f,.19f,.045f),brass,8);
   var transom=new Vector3[20];var transomTriangles=new int[18*3];transom[0]=new Vector3(0,2.82f,1.53f);
   for(int i=0;i<=18;i++){float a=i*Mathf.PI/18;transom[i+1]=new Vector3(Mathf.Cos(a)*.91f,2.82f+Mathf.Sin(a)*.91f,1.53f);if(i<18){transomTriangles[i*3]=0;transomTriangles[i*3+1]=i+2;transomTriangles[i*3+2]=i+1;}}
   MeshArt.Mesh(root,"ArchedTransomRecess",transom,transomTriangles,null,teal);
   for(int ray=0;ray<5;ray++){float a=(ray+1)*Mathf.PI/6;var mullion=MeshArt.Box(root,"TransomFanMullion",new Vector3(Mathf.Cos(a)*.34f,2.85f+Mathf.Sin(a)*.34f,1.506f),new Vector3(.025f,.70f,.027f),brass);mullion.transform.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg-90);}
   MeshArt.Box(root,"BalconyHandrail",new Vector3(0,1.15f,-1.18f),new Vector3(2.45f,.049f,.055f),brass);for(int i=0;i<9;i++)MeshArt.Box(root,"BalconyBaluster",new Vector3(-1.12f+i*.28f,.81f,-1.18f),new Vector3(.027f,.61f,.027f),teal);
   MeshArt.Box(root,"FloorBanner",new Vector3(-1.26f,2.12f,.34f),new Vector3(.47f,.75f,.055f),teal);var label=new GameObject("ArchitecturalFloorNumber",typeof(TextMesh));label.transform.SetParent(root,false);label.transform.localPosition=new Vector3(-1.26f,2.12f,.305f);label.transform.localRotation=Quaternion.identity;var text=label.GetComponent<TextMesh>();text.text=(floor+1).ToString();text.anchor=TextAnchor.MiddleCenter;text.fontSize=36;text.characterSize=.12f;text.color=new Color(1,.85f,.56f);
   for(int side=-1;side<=1;side+=2){Lantern(root,new Vector3(side*.90f,2.02f,.32f),brass,lamp);Planter(root,parent,new Vector3(side*.98f,.52f,-.72f),floor+side);}
   var shade=Finish(parent,FloorArchitectureSurface.Leaf);var leafLight=Finish(parent,FloorArchitectureSurface.LeafLight);
   FloorArchitectureDetails.Ivy(root,new Vector3(1.26f,3.87f,.29f),1.64f,floor+19,shade,leafLight);FloorArchitectureDetails.Ivy(root,new Vector3(-1.14f,.47f,-1.19f),1.45f,floor+71,shade,leafLight);
  }
  static void Lantern(Transform root,Vector3 at,Material brass,Material lamp)
  {
   MeshArt.Box(root,"LanternWallMount",at+Vector3.forward*.11f,new Vector3(.17f,.31f,.075f),brass);MeshArt.Box(root,"LanternGlass",at+Vector3.back*.045f,new Vector3(.13f,.26f,.11f),lamp);
   for(int side=-1;side<=1;side+=2)MeshArt.Box(root,"LanternHousing",at+new Vector3(side*.084f,0,-.055f),new Vector3(.025f,.32f,.14f),brass);
   MeshArt.Lathe(root,"LanternCap",new[]{0f,.06f,.14f},new[]{.14f,.14f,.026f},8,brass,Vector3.one).transform.localPosition=at+Vector3.up*.16f;
   MeshArt.Box(root,"LanternBase",at+Vector3.down*.16f,new Vector3(.21f,.044f,.17f),brass);
  }
  public static void Rails(Transform parent)
  {
   var teal=Finish(parent,FloorArchitectureSurface.Teal);var brass=Finish(parent,FloorArchitectureSurface.Brass);
   for(int side=-1;side<=1;side+=2){MeshArt.Box(parent,"VerticalLiftRail",new Vector3(side*2.15f,1.75f,1.85f),new Vector3(.12f,6.2f,.17f),teal);MeshArt.Box(parent,"CopperRailGuide",new Vector3(side*2.15f,1.75f,1.748f),new Vector3(.036f,6.2f,.025f),brass);for(int k=0;k<6;k++)MeshArt.Box(parent,"RailMount",new Vector3(side*2.15f,-.9f+k*1.05f,1.94f),new Vector3(.23f,.09f,.23f),brass);}
  }
  static void Planter(Transform parent,Transform owner,Vector3 at,int seed)
  {
   var pot=MeshArt.Lathe(parent,"StonePlanter",new[]{0f,.06f,.32f,.39f},new[]{.17f,.19f,.25f,.28f},16,Finish(owner,FloorArchitectureSurface.Stone),Vector3.one);pot.transform.localPosition=at;
   FloorArchitectureDetails.Plant(parent,at,Finish(owner,FloorArchitectureSurface.Leaf),Finish(owner,FloorArchitectureSurface.LeafLight),seed);
  }
 }
}
