using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class PlatformArt
 {
  public static StepMarker Build(Transform root,int index,int tower){
   bool checkpoint=(index+1)%5==0;var steel=MeshArt.Mat("SteelDeck",new Color(.77f,.74f,.65f));var side=MeshArt.Mat("SteelRib",new Color(.45f,.29f,.19f));var gold=MeshArt.Mat("RouteGold",new Color(1,.58f,.06f),-1,true);var cyan=MeshArt.Mat("CircuitCyan",new Color(.035f,.25f,.27f),-1,true);
   // Chamfered octagonal plan with stepped industrial body. Collision remains a simple inset box.
   var outline=new[]{new Vector2(-2.1f,-1.9f),new Vector2(2.1f,-1.9f),new Vector2(2.4f,-1.6f),new Vector2(2.4f,1.6f),new Vector2(2.1f,1.9f),new Vector2(-2.1f,1.9f),new Vector2(-2.4f,1.6f),new Vector2(-2.4f,-1.6f)};
   var v=new Vector3[48];var uv=new Vector2[48];var triangles=new int[48];int n=0;
   for(int i=0;i<8;i++){int j=(i+1)%8;var a=new Vector3(outline[i].x,.6f,outline[i].y);var b=new Vector3(outline[j].x,.6f,outline[j].y);v[n]=new Vector3(0,.6f,0);v[n+1]=b;v[n+2]=a;for(int k=0;k<3;k++){uv[n+k]=new Vector2(v[n+k].x/4.8f+.5f,v[n+k].z/3.8f+.5f);triangles[n+k]=n+k;}n+=3;}
   System.Array.Resize(ref v,n);System.Array.Resize(ref uv,n);System.Array.Resize(ref triangles,n);MeshArt.Mesh(root,"ChamferedDeck",v,triangles,uv,steel);
   for(int i=0;i<8;i++){var a=outline[i];var b=outline[(i+1)%8];var mid=(a+b)*.5f;var d=b-a;float angle=-Mathf.Atan2(d.y,d.x)*Mathf.Rad2Deg;var wall=MeshArt.Box(root,"ArmouredSide",new Vector3(mid.x,.15f,mid.y),new Vector3(d.magnitude,.82f,.12f),side);wall.transform.localRotation=Quaternion.Euler(0,angle,0);var edge=MeshArt.Box(root,"GoldEdge",new Vector3(mid.x,.58f,mid.y),new Vector3(d.magnitude,.065f,.085f),gold);edge.transform.localRotation=wall.transform.localRotation;var under=MeshArt.Box(root,"CyanUnderside",new Vector3(mid.x,-.20f,mid.y),new Vector3(d.magnitude,.04f,.075f),cyan);under.transform.localRotation=wall.transform.localRotation;}
   for(int s=-1;s<=1;s+=2)for(int z=-1;z<=1;z+=2){MeshArt.Box(root,"CornerBracket",new Vector3(s*1.99f,.61f,z*1.52f),new Vector3(.24f,.035f,.24f),side);MeshArt.Oval(root,"Fastener",new Vector3(s*1.99f,.64f,z*1.52f),new Vector3(.075f,.04f,.075f),gold);}
   for(int s=-1;s<=1;s+=2){MeshArt.Box(root,"PrecisionBaySide",new Vector3(s*.83f,.62f,0),new Vector3(.045f,.018f,1.60f),gold);MeshArt.Box(root,"PrecisionBayEnd",new Vector3(0,.62f,s*.80f),new Vector3(1.7f,.018f,.045f),gold);}
   // Insets and machinery stay below the landing surface; only LandingSurface owns collision.
   var inset=MeshArt.Mat("DeckInlay",new Color(.07f,.21f,.22f));var alloy=MeshArt.Mat("LiftAlloy",new Color(.81f,.53f,.24f));
   for(int sideSign=-1;sideSign<=1;sideSign+=2){MeshArt.Box(root,"ServiceInlay",new Vector3(sideSign*1.52f,.612f,0),new Vector3(.60f,.018f,2.55f),inset);for(int slot=0;slot<7;slot++)MeshArt.Box(root,"DeckVent",new Vector3(sideSign*1.52f,.626f,-.90f+slot*.30f),new Vector3(.42f,.012f,.055f),alloy);
    for(int tick=0;tick<5;tick++){var stripe=MeshArt.Box(root,"SafetyChevron",new Vector3(sideSign*(.50f+tick*.22f),.626f,-1.64f),new Vector3(.075f,.012f,.23f),gold);stripe.transform.localRotation=Quaternion.Euler(0,sideSign*28,0);}
    MeshArt.Box(root,"UnderslungCarriage",new Vector3(sideSign*1.72f,-.28f,0),new Vector3(.54f,.32f,1.45f),inset);MeshArt.Box(root,"CarriagePower",new Vector3(sideSign*2.01f,-.27f,0),new Vector3(.024f,.095f,.84f),cyan);
    for(int wheel=-1;wheel<=1;wheel+=2){var roller=MeshArt.Lathe(root,"CarriageRoller",new[]{-.5f,-.36f,.36f,.5f},new[]{.31f,.38f,.38f,.31f},10,alloy,new Vector3(.48f,.18f,.48f));roller.transform.localPosition=new Vector3(sideSign*1.77f,-.43f,wheel*.56f);roller.transform.localRotation=Quaternion.Euler(0,0,90);}
   }
   MeshArt.Box(root,"LiftControlHousing",new Vector3(1.95f,.28f,1.65f),new Vector3(.48f,.52f,.24f),inset);MeshArt.Box(root,"LiftControlDisplay",new Vector3(1.95f,.32f,1.516f),new Vector3(.30f,.18f,.018f),cyan);
   var contrast=new GameObject("RouteContrastRim");contrast.transform.SetParent(root,false);for(int s=-1;s<=1;s+=2)MeshArt.Box(contrast.transform,"ReadableBorder",new Vector3(s*2.13f,.62f,0),new Vector3(.06f,.03f,3.1f),MeshArt.Mat("ContrastIvory",Color.white));contrast.SetActive(false);
   var surface=new GameObject("LandingSurface",typeof(BoxCollider),typeof(StepMarker));surface.transform.SetParent(root,false);var collider=surface.GetComponent<BoxCollider>();collider.center=new Vector3(0,.25f,0);collider.size=new Vector3(4.6f,.70f,3.6f);var marker=surface.GetComponent<StepMarker>();marker.Index=index;
   LiftArchitecture.Cabin(root);
   if(checkpoint){var ring=MeshArt.Ring(root,"CheckpointRing",.34f,.045f,0,gold);ring.transform.localPosition=new Vector3(0,2.72f,1.52f);ring.transform.localRotation=Quaternion.Euler(90,0,0);var back=MeshArt.Ring(root,"CheckpointRingBack",.34f,.045f,0,gold);back.transform.localPosition=ring.transform.localPosition;back.transform.localRotation=Quaternion.Euler(-90,0,0);}
   return marker;
  }
 }
}
