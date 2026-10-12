using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.Core;

namespace Kamilunavo.OneMoreFloor.UI
{
 // Owned vector artwork for the Concept's brass-framed landing instrument.
 // Its windows and needle are positioned by FloorHud from the live forecast.
 public sealed class FloorTimingInstrument:MaskableGraphic
 {
  public enum Face{Frame,Track,Landing,Perfect,Needle,Lift,Tower,Transfer}
  public Face Artwork;
  // Calibrate the instrument around the real safe/perfect intervals. This makes
  // the narrow physical timing window readable without changing hit authority.
  public static float Position(float error,float window){if(!float.IsFinite(error)||!float.IsFinite(window)||window<=0)return .5f;float absolute=Mathf.Abs(error),perfect=window*.24f;int hit=ElevatorRules.Hit(error,0,window);float distance=hit==2?Mathf.Min(.06f,absolute/perfect*.06f):hit==1?.06f+Mathf.Max(.00001f,(absolute-perfect)/(window-perfect)*.12f):.18f+Mathf.Max(.00001f,Mathf.Min(1,(absolute-window)/(window*4))*.31f);return .5f+Mathf.Sign(error)*distance;}
  public static FloorTimingInstrument Add(Transform parent,string name,Face face,Vector2 min,Vector2 max){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(FloorTimingInstrument));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;var graphic=go.GetComponent<FloorTimingInstrument>();graphic.Artwork=face;graphic.raycastTarget=false;return graphic;}
  protected override void OnPopulateMesh(VertexHelper mesh){mesh.Clear();var r=rectTransform.rect;Color brass=new(.96f,.77f,.40f),dark=new(.035f,.095f,.095f),cream=new(1,.94f,.74f);
   void Vert(Vector2 p,Color c){mesh.AddVert(p,c*color,Vector2.zero);}
   void Quad(Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color bottom,Color top){int k=mesh.currentVertCount;Vert(a,bottom);Vert(b,bottom);Vert(c,top);Vert(d,top);mesh.AddTriangle(k,k+1,k+2);mesh.AddTriangle(k,k+2,k+3);}
   void Capsule(Rect bounds,Color bottom,Color top){float radius=Mathf.Min(bounds.width,bounds.height)*.5f;int first=mesh.currentVertCount;Vert(bounds.center,Color.Lerp(bottom,top,.5f));for(int i=0;i<=48;i++){float a=i*Mathf.PI*2/48;float x=bounds.center.x+Mathf.Cos(a)*radius+Mathf.Sign(Mathf.Cos(a))*Mathf.Max(0,bounds.width*.5f-radius),y=bounds.center.y+Mathf.Sin(a)*radius+Mathf.Sign(Mathf.Sin(a))*Mathf.Max(0,bounds.height*.5f-radius);var p=new Vector2(x,y);Vert(p,Color.Lerp(bottom,top,(p.y-bounds.yMin)/bounds.height));if(i>0)mesh.AddTriangle(first,first+i,first+i+1);}}
   Rect Inset(float amount)=>new(r.xMin+amount,r.yMin+amount,Mathf.Max(1,r.width-amount*2),Mathf.Max(1,r.height-amount*2));
   if(Artwork==Face.Frame){Capsule(r,new Color(.32f,.21f,.10f),cream);Capsule(Inset(1.2f),new Color(.08f,.16f,.17f),new Color(.04f,.09f,.10f));Capsule(Inset(3),dark,new Color(.10f,.19f,.19f));return;}
   if(Artwork==Face.Track){Capsule(r,new Color(.77f,.63f,.36f),cream);Capsule(Inset(.8f),new Color(.23f,.24f,.23f),new Color(.37f,.39f,.36f));return;}
   if(Artwork==Face.Needle){Capsule(r,new Color(.82f,.60f,.18f),cream);Capsule(Inset(.65f),Color.white,new Color(1,.99f,.94f));return;}
   if(Artwork==Face.Landing||Artwork==Face.Perfect){Color a=Artwork==Face.Landing?new Color(.17f,.57f,.30f):new Color(1,.78f,.08f),b=Artwork==Face.Landing?new Color(.51f,.78f,.22f):new Color(1,.94f,.36f);for(int i=0;i<12;i++){float start=i/12f,end=(i+1)/12f;float peak=1-Mathf.Abs((start+end)-1);Color c=Color.Lerp(a,b,peak);Quad(new(r.xMin+start*r.width,r.yMin),new(r.xMin+end*r.width,r.yMin),new(r.xMin+end*r.width,r.yMax),new(r.xMin+start*r.width,r.yMax),c*.78f,c);}return;}
   float size=Mathf.Min(r.width,r.height);Vector2 P(float x,float y)=>r.center+new Vector2(x-.5f,y-.5f)*size;
   void Line(float ax,float ay,float bx,float by,float width,Color c){var a=P(ax,ay);var b=P(bx,by);var n=new Vector2(-(b-a).y,(b-a).x).normalized*(width*size*.5f);Quad(a-n,b-n,b+n,a+n,c,c);}
   void Box(float x,float y,float w,float h,Color c)=>Quad(P(x,y),P(x+w,y),P(x+w,y+h),P(x,y+h),c,c);
   if(Artwork==Face.Transfer){var ink=Color.white;for(int i=0;i<2;i++){float x=.20f+i*.30f;Line(x,.20f,x+.24f,.5f,.105f,ink);Line(x+.24f,.5f,x,.80f,.105f,ink);}return;}
   if(Artwork==Face.Lift){Line(.50f,.90f,.50f,.97f,.06f,cream);Line(.19f,.76f,.50f,.88f,.07f,cream);Line(.50f,.88f,.81f,.76f,.07f,cream);Line(.19f,.74f,.81f,.74f,.07f,cream);for(int side=0;side<2;side++){float x=.27f+side*.46f;Line(x,.20f,x,.73f,.065f,cream);}Line(.19f,.18f,.81f,.18f,.07f,cream);Line(.16f,.09f,.84f,.09f,.07f,cream);Line(.5f,.20f,.5f,.67f,.04f,cream);return;}
   // Arched stone station, distinct from the open elevator cage.
   Box(.13f,.09f,.74f,.07f,brass);Box(.21f,.18f,.09f,.55f,brass);Box(.70f,.18f,.09f,.55f,brass);Box(.43f,.18f,.14f,.46f,brass);Line(.16f,.76f,.5f,.90f,.10f,brass);Line(.5f,.90f,.84f,.76f,.10f,brass);Box(.16f,.71f,.68f,.07f,brass);
  }
 }
}
