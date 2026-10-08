using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class PlatformArt
 {
  public static StepMarker Build(Transform root,int index,int tower){
   bool checkpoint=(index+1)%5==0;var steel=MeshArt.Mat("SteelDeck",new Color(.48f,.50f,.56f),0);var side=MeshArt.Mat("SteelRib",new Color(.16f,.19f,.27f),1);var gold=MeshArt.Mat("RouteGold",new Color(1,.58f,.06f),-1,true);var cyan=MeshArt.Mat("CircuitCyan",new Color(.015f,.65f,.95f),-1,true);
   // Chamfered octagonal plan with stepped industrial body. Collision remains a simple inset box.
   var outline=new[]{new Vector2(-2.1f,-1.9f),new Vector2(2.1f,-1.9f),new Vector2(2.4f,-1.6f),new Vector2(2.4f,1.6f),new Vector2(2.1f,1.9f),new Vector2(-2.1f,1.9f),new Vector2(-2.4f,1.6f),new Vector2(-2.4f,-1.6f)};
   var v=new Vector3[48];var uv=new Vector2[48];var triangles=new int[48];int n=0;
   for(int i=0;i<8;i++){int j=(i+1)%8;var a=new Vector3(outline[i].x,.6f,outline[i].y);var b=new Vector3(outline[j].x,.6f,outline[j].y);v[n]=new Vector3(0,.6f,0);v[n+1]=b;v[n+2]=a;for(int k=0;k<3;k++){uv[n+k]=new Vector2(v[n+k].x/4.8f+.5f,v[n+k].z/3.8f+.5f);triangles[n+k]=n+k;}n+=3;}
   System.Array.Resize(ref v,n);System.Array.Resize(ref uv,n);System.Array.Resize(ref triangles,n);MeshArt.Mesh(root,"ChamferedDeck",v,triangles,uv,steel);
   for(int i=0;i<8;i++){var a=outline[i];var b=outline[(i+1)%8];var mid=(a+b)*.5f;var d=b-a;float angle=-Mathf.Atan2(d.y,d.x)*Mathf.Rad2Deg;var wall=MeshArt.Box(root,"ArmouredSide",new Vector3(mid.x,.15f,mid.y),new Vector3(d.magnitude,.82f,.12f),side);wall.transform.localRotation=Quaternion.Euler(0,angle,0);var edge=MeshArt.Box(root,"GoldEdge",new Vector3(mid.x,.58f,mid.y),new Vector3(d.magnitude,.065f,.085f),gold);edge.transform.localRotation=wall.transform.localRotation;var under=MeshArt.Box(root,"CyanUnderside",new Vector3(mid.x,-.20f,mid.y),new Vector3(d.magnitude,.04f,.075f),cyan);under.transform.localRotation=wall.transform.localRotation;}
   for(int s=-1;s<=1;s+=2)for(int z=-1;z<=1;z+=2){MeshArt.Box(root,"CornerBracket",new Vector3(s*1.99f,.61f,z*1.52f),new Vector3(.24f,.035f,.24f),side);MeshArt.Oval(root,"Fastener",new Vector3(s*1.99f,.64f,z*1.52f),new Vector3(.075f,.04f,.075f),gold);}
   for(int s=-1;s<=1;s+=2){MeshArt.Box(root,"PrecisionBaySide",new Vector3(s*.83f,.62f,0),new Vector3(.045f,.018f,1.60f),gold);MeshArt.Box(root,"PrecisionBayEnd",new Vector3(0,.62f,s*.80f),new Vector3(1.7f,.018f,.045f),gold);}
   var contrast=new GameObject("RouteContrastRim");contrast.transform.SetParent(root,false);for(int s=-1;s<=1;s+=2)MeshArt.Box(contrast.transform,"ReadableBorder",new Vector3(s*2.13f,.62f,0),new Vector3(.06f,.03f,3.1f),MeshArt.Mat("ContrastIvory",Color.white));contrast.SetActive(false);
   var surface=new GameObject("LandingSurface",typeof(BoxCollider),typeof(StepMarker));surface.transform.SetParent(root,false);var collider=surface.GetComponent<BoxCollider>();collider.center=new Vector3(0,.25f,0);collider.size=new Vector3(4.6f,.70f,3.6f);var marker=surface.GetComponent<StepMarker>();marker.Index=index;
   if(checkpoint){var ring=MeshArt.Ring(root,"CheckpointRing",1.2f,.09f,0,gold);ring.transform.localPosition=new Vector3(0,2.05f,.35f);ring.transform.localRotation=Quaternion.Euler(90,0,0);var back=MeshArt.Ring(root,"CheckpointRingBack",1.2f,.09f,0,gold);back.transform.localPosition=ring.transform.localPosition;back.transform.localRotation=Quaternion.Euler(-90,0,0);}
   return marker;
  }
 }
}
