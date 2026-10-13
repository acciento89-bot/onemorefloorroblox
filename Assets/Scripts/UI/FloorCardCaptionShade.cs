using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 // A corner atmosphere, not an opaque title panel. The whole original scene
 // stays visible while white mixed-case captions retain readable contrast.
 public sealed class FloorCardCaptionShade:MaskableGraphic
 {
  public static float Alpha(Vector2 uv)=>.76f*Mathf.SmoothStep(0,1,Mathf.InverseLerp(.38f,.83f,uv.y))*(1-.18f*Mathf.SmoothStep(0,1,Mathf.InverseLerp(.35f,.99f,uv.x)));
  public bool light;
  public Color Backdrop=>light?Color.white:Color.black;
  public static void Add(Transform parent,bool light=false){var node=new GameObject("CaptionAtmosphere",typeof(RectTransform),typeof(CanvasRenderer),typeof(FloorCardCaptionShade));node.transform.SetParent(parent,false);var shade=node.GetComponent<FloorCardCaptionShade>();shade.light=light;shade.raycastTarget=false;var r=shade.rectTransform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(4,4);r.offsetMax=new Vector2(-4,-4);}
  protected override void OnPopulateMesh(VertexHelper mesh)
  {
   mesh.Clear();var r=rectTransform.rect;const int columns=16,rows=12;
   for(int y=0;y<=rows;y++)for(int x=0;x<=columns;x++){var uv=new Vector2(x/(float)columns,y/(float)rows);mesh.AddVert(new Vector2(r.xMin+uv.x*r.width,r.yMin+uv.y*r.height),new Color(light?1:0,light?1:0,light?1:0,Alpha(uv)),Vector2.zero);}
   for(int y=0;y<rows;y++)for(int x=0;x<columns;x++){int first=y*(columns+1)+x;mesh.AddTriangle(first,first+columns+1,first+1);mesh.AddTriangle(first+1,first+columns+1,first+columns+2);}
  }
 }
}
