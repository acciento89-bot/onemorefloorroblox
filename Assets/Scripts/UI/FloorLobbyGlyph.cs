using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 public sealed class FloorLobbyGlyph:MaskableGraphic
 {
  public enum Kind{Route,Style,Shop,Gift,Award,Settings,Tutorial}
  public Kind Symbol;public bool Outlined;
  public static FloorLobbyGlyph Add(Transform parent,Kind symbol,Color ink){var go=new GameObject("LobbyGlyph",typeof(RectTransform),typeof(CanvasRenderer),typeof(FloorLobbyGlyph));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;var glyph=go.GetComponent<FloorLobbyGlyph>();glyph.Symbol=symbol;glyph.color=ink;glyph.raycastTarget=false;return glyph;}
  protected override void OnPopulateMesh(VertexHelper mesh){mesh.Clear();var rect=rectTransform.rect;float scale=Mathf.Min(rect.width,rect.height);var origin=new Vector2(rect.center.x-scale/2,rect.center.y-scale/2);Vector2 P(Vector2 p)=>origin+p*scale;
   void Poly(params Vector2[] points){int start=mesh.currentVertCount;foreach(var p in points)mesh.AddVert(P(p),color,Vector2.zero);for(int i=1;i<points.Length-1;i++)mesh.AddTriangle(start,start+i,start+i+1);}
   void Line(Vector2 a,Vector2 b,float width){var n=new Vector2(-(b-a).y,(b-a).x).normalized*width/2;Poly(a-n,b-n,b+n,a+n);}
   void Box(float x,float y,float w,float h)=>Poly(new(x,y),new(x+w,y),new(x+w,y+h),new(x,y+h));
   void Ring(Vector2 center,float radius,float width,int sides=24){for(int i=0;i<sides;i++){float a=i*2*Mathf.PI/sides,b=(i+1)*2*Mathf.PI/sides;Vector2 A(float angle,float r)=>center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*r;Poly(A(a,radius-width),A(a,radius),A(b,radius),A(b,radius-width));}}
   switch(Symbol){
    case Kind.Style:Poly(new(.23f,.35f),new(.27f,.76f),new(.66f,.81f),new(.73f,.35f));Poly(new(.06f,.28f),new(.23f,.35f),new(.73f,.35f),new(.94f,.26f),new(.74f,.15f),new(.23f,.16f));break;
    case Kind.Shop:Box(.18f,.12f,.64f,.54f);Ring(new(.5f,.66f),.21f,.065f);break;
    case Kind.Gift:Box(.13f,.12f,.29f,.46f);Box(.56f,.12f,.29f,.46f);Box(.08f,.60f,.36f,.13f);Box(.54f,.60f,.36f,.13f);Ring(new(.33f,.81f),.13f,.045f);Ring(new(.65f,.81f),.13f,.045f);Box(.46f,.12f,.06f,.63f);break;
    case Kind.Route:Ring(new(.70f,.75f),.17f,.065f);Poly(new(.57f,.66f),new(.83f,.66f),new(.70f,.44f));Line(new(.17f,.18f),new(.35f,.18f),.07f);Line(new(.35f,.18f),new(.48f,.30f),.07f);Line(new(.48f,.30f),new(.33f,.42f),.07f);Line(new(.33f,.42f),new(.16f,.35f),.07f);Ring(new(.15f,.18f),.075f,.04f);break;
    case Kind.Award:{if(Outlined){for(int i=0;i<10;i++){Vector2 Star(int k,float scale){float a=(90-k*36)*Mathf.Deg2Rad,r=(k%2==0?.42f:.18f)*scale;return new Vector2(.5f,.48f)+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r;}Poly(Star(i,1),Star(i+1,1),Star(i+1,.70f),Star(i,.70f));}break;}int first=mesh.currentVertCount;bool gold=color.r>.65f&&color.g>.35f;mesh.AddVert(P(new(.5f,.48f)),gold?Color.Lerp(color,Color.white,.22f):color,Vector2.zero);for(int i=0;i<10;i++){float a=(90-i*36)*Mathf.Deg2Rad,r=i%2==0?.42f:.18f;var point=new Vector2(.5f,.48f)+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r;var tint=gold?Color.Lerp(color*.62f,Color.Lerp(color,Color.white,.3f),point.y):color;tint.a=color.a;mesh.AddVert(P(point),tint,Vector2.zero);}for(int i=0;i<10;i++)mesh.AddTriangle(first,first+1+i,first+1+(i+1)%10);break;}
    case Kind.Settings:Ring(new(.50f,.50f),.26f,.12f);for(int i=0;i<8;i++){float a=i*Mathf.PI/4;Line(new(.50f+Mathf.Cos(a)*.23f,.50f+Mathf.Sin(a)*.23f),new(.50f+Mathf.Cos(a)*.41f,.50f+Mathf.Sin(a)*.41f),.12f);}break;
    case Kind.Tutorial:Line(new(.5f,.13f),new(.5f,.77f),.05f);Poly(new(.10f,.20f),new(.10f,.80f),new(.43f,.73f),new(.43f,.13f));Poly(new(.57f,.13f),new(.57f,.73f),new(.90f,.80f),new(.90f,.20f));Line(new(.12f,.13f),new(.42f,.07f),.04f);Line(new(.58f,.07f),new(.88f,.13f),.04f);break;
   }
  }
 }
}
