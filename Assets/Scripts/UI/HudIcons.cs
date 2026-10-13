using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 [RequireComponent(typeof(CanvasRenderer))] public sealed class HudIcons:MaskableGraphic
 {
  public enum Kind{Menu,Jump,Crystal,Arrow,ChevronUp}public Kind Icon;Sprite _coin;Sprite Coin=>_coin!=null?_coin:_coin=Resources.Load<Sprite>("UI/Concept/GoldCoin-v1");public override Texture mainTexture=>Icon==Kind.Crystal&&Coin!=null?Coin.texture:base.mainTexture;
  public static void Add(Transform parent,Kind kind){var go=new GameObject("Icon",typeof(RectTransform),typeof(CanvasRenderer),typeof(HudIcons));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=(kind==Kind.Crystal||kind==Kind.ChevronUp)?Vector2.zero:Vector2.one*.20f;r.anchorMax=(kind==Kind.Crystal||kind==Kind.ChevronUp)?Vector2.one:Vector2.one*.80f;r.offsetMin=r.offsetMax=Vector2.zero;var g=go.GetComponent<HudIcons>();g.Icon=kind;g.color=Color.white;g.raycastTarget=false;}
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();var r=rectTransform.rect;if(Icon==Kind.ChevronUp){float side=Mathf.Min(r.width,r.height);r=new Rect(r.center-Vector2.one*side*.5f,Vector2.one*side);}var ink=color;Vector2 P(float x,float y)=>new(r.xMin+x*r.width,r.yMin+y*r.height);
   void Poly(params Vector2[] points){int start=v.currentVertCount;foreach(var p in points)v.AddVert(p,ink,Vector2.zero);for(int i=1;i<points.Length-1;i++)v.AddTriangle(start,start+i,start+i+1);}
   if(Icon==Kind.Crystal&&Coin!=null){float size=Mathf.Min(r.width,r.height);var bounds=new Rect(r.center-Vector2.one*size*.5f,Vector2.one*size);var uv=UnityEngine.Sprites.DataUtility.GetOuterUV(Coin);v.AddVert(new Vector2(bounds.xMin,bounds.yMin),color,new Vector2(uv.x,uv.y));v.AddVert(new Vector2(bounds.xMin,bounds.yMax),color,new Vector2(uv.x,uv.w));v.AddVert(new Vector2(bounds.xMax,bounds.yMax),color,new Vector2(uv.z,uv.w));v.AddVert(new Vector2(bounds.xMax,bounds.yMin),color,new Vector2(uv.z,uv.y));v.AddTriangle(0,1,2);v.AddTriangle(2,3,0);return;}
   if(Icon==Kind.ChevronUp){Poly(P(.13f,.30f),P(.5f,.80f),P(.5f,.52f),P(.27f,.15f));Poly(P(.5f,.80f),P(.87f,.30f),P(.73f,.15f),P(.5f,.52f));return;}
   if(Icon==Kind.Menu){for(int i=0;i<3;i++){float y=.20f+i*.26f;Poly(P(.12f,y),P(.88f,y),P(.88f,y+.10f),P(.12f,y+.10f));}}
   else if(Icon==Kind.Crystal){var points=new Vector2[32];for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32;points[i]=P(.5f+Mathf.Cos(a)*.45f,.5f+Mathf.Sin(a)*.45f);}Poly(points);var old=ink;ink=new Color(.55f,.30f,.065f);Poly(P(.44f,.20f),P(.56f,.20f),P(.56f,.80f),P(.44f,.80f));ink=old;}
   else {Poly(P(.36f,.08f),P(.64f,.08f),P(.64f,.56f),P(.36f,.56f));Poly(P(.10f,.51f),P(.90f,.51f),P(.5f,.95f));}
  }
 }
}
