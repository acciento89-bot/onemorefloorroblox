using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 /// <summary>Authored lift elevation illustration, independent of menu aspect and route camera.</summary>
 [RequireComponent(typeof(CanvasRenderer))] public sealed class LiftTowerGraphic:MaskableGraphic
 {
  public int Tower;
  public static RectTransform Create(Transform parent,int tower){var go=new GameObject("LiftElevation",typeof(RectTransform),typeof(CanvasRenderer),typeof(LiftTowerGraphic));go.transform.SetParent(parent,false);var graphic=go.GetComponent<LiftTowerGraphic>();graphic.Tower=tower;graphic.raycastTarget=false;return (RectTransform)go.transform;}
  protected override void OnPopulateMesh(VertexHelper mesh){mesh.Clear();var r=rectTransform.rect;
   Vector2 P(float x,float y)=>new Vector2(r.xMin+x*r.width,r.yMin+y*r.height);
   void Quad(float x,float y,float w,float h,Color c){int s=mesh.currentVertCount;mesh.AddVert(P(x,y),c,Vector2.zero);mesh.AddVert(P(x+w,y),c,Vector2.zero);mesh.AddVert(P(x+w,y+h),c,Vector2.zero);mesh.AddVert(P(x,y+h),c,Vector2.zero);mesh.AddTriangle(s,s+1,s+2);mesh.AddTriangle(s,s+2,s+3);}
   void Poly(Color c,params Vector2[] p){int s=mesh.currentVertCount;foreach(var v in p)mesh.AddVert(P(v.x,v.y),c,Vector2.zero);for(int i=1;i<p.Length-1;i++)mesh.AddTriangle(s,s+i,s+i+1);}
   Color steel=new Color(.12f,.21f,.28f),shade=new Color(.06f,.13f,.19f),cyan=new Color(.12f,.8f,.88f),amber=Tower==1?new Color(.68f,.48f,1):Tower==2?new Color(.98f,.48f,.31f):new Color(1,.72f,.18f);
   for(int i=0;i<7;i++){float x=.015f+i*.14f,h=.10f+(i*7%5)*.046f;Quad(x,.05f,.09f,h,shade);for(int j=0;j<3;j++)Quad(x+.02f,.07f+j*.05f,.012f,.012f,steel);}
   Quad(.29f,.10f,.025f,.80f,steel);Quad(.72f,.10f,.025f,.80f,steel);Quad(.31f,.10f,.005f,.80f,cyan);Quad(.71f,.10f,.005f,.80f,cyan);
   for(int i=0;i<5;i++){float y=.16f+i*.15f,x=.16f+(i%2)*.11f;Poly(steel,new Vector2(x,y),new Vector2(x+.46f,y),new Vector2(x+.60f,y+.045f),new Vector2(x+.12f,y+.045f));Quad(x,y-.026f,.46f,.026f,shade);Quad(x,y,.46f,.009f,amber);Quad(x+.12f,y+.034f,.46f,.008f,cyan);Quad(x+.17f,y+.043f,.011f,.063f,steel);Quad(x+.44f,y+.043f,.011f,.063f,steel);}
   for(int i=0;i<8;i++){float t=i/7f;Quad(.43f+.11f*t,.22f+.15f*t+.045f*Mathf.Sin(t*Mathf.PI),.01f,.008f,cyan);}
   Quad(.47f,.213f,.04f,.065f,amber);Quad(.476f,.28f,.028f,.025f,new Color(.92f,.97f,1));
   Quad(.47f,.85f,.18f,.06f,shade);Quad(.485f,.868f,.15f,.012f,cyan);
  }
 }
}
