using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class PlatformArt
 {
  public static StepMarker Build(Transform root,int index,int tower){
   bool checkpoint=(index+1)%5==0;var steel=FloorArchitecturePalette.Get(root,FloorArchitectureSurface.Deck);var side=FloorArchitecturePalette.Get(root,FloorArchitectureSurface.Copper);var gold=FloorArchitecturePalette.Get(root,FloorArchitectureSurface.Lamp);
   // Passenger floor is a shaped oval; timing authority retains its original collider.
   const int segments=64;var v=new Vector3[segments*3];var uv=new Vector2[v.Length];var triangles=new int[v.Length];
   for(int i=0;i<segments;i++){float a=i*Mathf.PI*2/segments,b=(i+1)*Mathf.PI*2/segments;int k=i*3;v[k]=new Vector3(0,.6f,0);v[k+1]=new Vector3(Mathf.Cos(b)*1.24f,.6f,Mathf.Sin(b)*1.02f);v[k+2]=new Vector3(Mathf.Cos(a)*1.24f,.6f,Mathf.Sin(a)*1.02f);for(int j=0;j<3;j++){triangles[k+j]=k+j;uv[k+j]=new Vector2(v[k+j].x/2.48f+.5f,v[k+j].z/2.04f+.5f);}}
   MeshArt.Mesh(root,"ChamferedDeck",v,triangles,uv,steel);
   // A narrow doorway tread preserves real visible foot support through the
   // unchanged .38 walk-out/boarding phase, despite the closer-fitting oval deck.
   FloorArchitectureDetails.BeveledBlock(root,"DoorwayThreshold",new Vector3(.16f,.575f,-1.18f),new Vector3(.92f,.05f,.58f),.007f,steel);
   var coping=MeshArt.Lathe(root,"DeckStoneCoping",new[]{.43f,.47f,.57f,.60f},new[]{1.18f,1.24f,1.24f,1.18f},64,side,new Vector3(1,1,.826f));
   var edge=MeshArt.Ring(root,"GoldEdge",1.24f,.035f,.605f,FloorArchitecturePalette.Get(root,FloorArchitectureSurface.Brass));edge.transform.localScale=new Vector3(1,1,.826f);
   for(int s=-1;s<=1;s+=2){MeshArt.Box(root,"PrecisionBaySide",new Vector3(s*.83f,.62f,0),new Vector3(.045f,.018f,1.60f),gold);MeshArt.Box(root,"PrecisionBayEnd",new Vector3(0,.62f,s*.80f),new Vector3(1.7f,.018f,.045f),gold);}
   // Narrow decorative inlays and compact carriage fittings retain their legacy names.
   var inset=FloorArchitecturePalette.Get(root,FloorArchitectureSurface.Teal);var alloy=FloorArchitecturePalette.Get(root,FloorArchitectureSurface.Brass);
   for(int sideSign=-1;sideSign<=1;sideSign+=2){
    MeshArt.Box(root,"ServiceInlay",new Vector3(sideSign*1.09f,.607f,0),new Vector3(.075f,.012f,.90f),alloy);
    for(int line=0;line<3;line++)MeshArt.Box(root,"InlayEngraving",new Vector3(sideSign*1.09f,.615f,-.3f+line*.3f),new Vector3(.09f,.006f,.009f),inset);
    MeshArt.Box(root,"UnderslungCarriage",new Vector3(sideSign*1.21f,-.34f,.26f),new Vector3(.30f,.24f,.81f),inset);
    for(int wheel=-1;wheel<=1;wheel+=2){var roller=MeshArt.Lathe(root,"CarriageRoller",new[]{-.5f,-.36f,.36f,.5f},new[]{.31f,.38f,.38f,.31f},12,alloy,new Vector3(.34f,.13f,.34f));roller.transform.localPosition=new Vector3(sideSign*1.22f,-.43f,.26f+wheel*.33f);roller.transform.localRotation=Quaternion.Euler(0,0,90);}
   }
   MeshArt.Box(root,"LiftControlHousing",new Vector3(1.10f,.74f,.55f),new Vector3(.24f,.30f,.16f),alloy);MeshArt.Box(root,"LiftControlDisplay",new Vector3(1.10f,.75f,.461f),new Vector3(.17f,.12f,.015f),inset);
   var contrast=new GameObject("RouteContrastRim");contrast.transform.SetParent(root,false);for(int s=-1;s<=1;s+=2)MeshArt.Box(contrast.transform,"ReadableBorder",new Vector3(s*1.40f,.62f,0),new Vector3(.06f,.03f,.75f),MeshArt.Mat("ContrastIvory",Color.white));contrast.SetActive(false);
   var surface=new GameObject("LandingSurface",typeof(BoxCollider),typeof(StepMarker));surface.transform.SetParent(root,false);var collider=surface.GetComponent<BoxCollider>();collider.center=new Vector3(0,.25f,0);collider.size=new Vector3(4.6f,.70f,3.6f);var marker=surface.GetComponent<StepMarker>();marker.Index=index;
   LiftArchitecture.Cabin(root);
   if(checkpoint){var ring=MeshArt.Ring(root,"CheckpointRing",.34f,.045f,0,gold);ring.transform.localPosition=new Vector3(0,2.72f,1.52f);ring.transform.localRotation=Quaternion.Euler(90,0,0);var back=MeshArt.Ring(root,"CheckpointRingBack",.34f,.045f,0,gold);back.transform.localPosition=ring.transform.localPosition;back.transform.localRotation=Quaternion.Euler(-90,0,0);}
   // Narrow the visible floor/fittings along with the actual cabin. The course
   // collider remains untouched; an original-width longer tread supports boots
   // until the existing .38 walk-out phase lifts the passenger off the deck.
   foreach(Transform part in root){if(part.name=="LandingSurface"||part.name=="CopperLiftCabin")continue;
    part.localPosition=Vector3.Scale(part.localPosition,LiftArchitecture.CabinRadialScale);part.localScale=Vector3.Scale(part.localScale,LiftArchitecture.CabinRadialScale);
   }
   var tread=root.Find("DoorwayThreshold");tread.localPosition=new Vector3(.16f,.575f,-.98f);tread.localScale=new Vector3(1,1,1.9f);
   return marker;
  }
 }
}
