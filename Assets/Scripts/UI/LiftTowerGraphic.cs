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
   Color steel=new Color(.23f,.34f,.43f),shade=new Color(.025f,.065f,.10f),cyan=new Color(.15f,.82f,.96f),amber=Tower==1?new Color(.80f,.57f,1):Tower==2?new Color(.98f,.65f,.28f):new Color(1,.75f,.20f);
   // Backplate, guide channels and cross-bracing turn the lift into a deliberate industrial silhouette.
   Poly(shade,new Vector2(.19f,.08f),new Vector2(.78f,.08f),new Vector2(.78f,.94f),new Vector2(.31f,.94f));
   for(int side=0;side<2;side++){float x=side==0?.29f:.74f;Quad(x,.08f,.046f,.84f,steel);Quad(x+.012f,.08f,.009f,.84f,cyan);for(int j=0;j<8;j++){float y=.11f+j*.1f;Quad(x-.015f,y,.075f,.012f,shade);Quad(x+.009f,y+.002f,.012f,.007f,amber);}}
   for(int j=0;j<6;j++){float y=.14f+j*.13f;Poly(steel,new Vector2(.34f,y),new Vector2(.35f,y),new Vector2(.74f,y+.13f),new Vector2(.73f,y+.13f));Quad(.35f,y+.01f,.38f,.006f,steel);}
   for(int i=0;i<5;i++){float y=.15f+i*.15f,x=.11f+(i%2)*.10f;Poly(shade,new Vector2(x,y-.025f),new Vector2(x+.49f,y-.025f),new Vector2(x+.65f,y+.031f),new Vector2(x+.65f,y+.07f),new Vector2(x+.14f,y+.07f));Poly(steel,new Vector2(x,y),new Vector2(x+.48f,y),new Vector2(x+.64f,y+.057f),new Vector2(x+.14f,y+.057f));Quad(x,y-.022f,.48f,.022f,shade);Quad(x,y,.48f,.009f,amber);Quad(x+.14f,y+.048f,.49f,.008f,cyan);
    for(int rib=0;rib<6;rib++)Quad(x+.025f+rib*.075f,y-.020f,.015f,.013f,steel);
    Poly(cyan,new Vector2(x+.21f,y+.022f),new Vector2(x+.38f,y+.022f),new Vector2(x+.43f,y+.039f),new Vector2(x+.26f,y+.039f));
    Quad(x+.43f,y+.055f,.052f,.076f,shade);Quad(x+.443f,y+.081f,.030f,.025f,cyan);
   }
   for(int i=0;i<10;i++){float t=i/9f;Quad(.41f+.16f*t,.23f+.13f*t+.045f*Mathf.Sin(t*Mathf.PI),.012f,.008f,amber);}
   Quad(.43f,.212f,.042f,.061f,amber);Quad(.436f,.274f,.030f,.027f,new Color(.94f,.98f,1));
   Quad(.38f,.895f,.30f,.055f,shade);Quad(.40f,.919f,.25f,.013f,cyan);Quad(.46f,.953f,.14f,.012f,amber);
  }
 }
}
