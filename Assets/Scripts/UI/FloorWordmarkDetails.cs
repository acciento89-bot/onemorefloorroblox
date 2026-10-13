using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 // Original live wordmark detailing: gently arched top line and an elevator
 // inside the first O. Runs only after TMP rebuilds; no per-frame mesh work.
 public sealed class FloorWordmarkDetails:MonoBehaviour
 {
  TextMeshProUGUI _text;FloorWordmarkLift _lift;bool _placeLift;public bool Curve;
  public static void Add(Text source,bool curve){var view=source.GetComponentInChildren<TextMeshProUGUI>();var detail=view.gameObject.AddComponent<FloorWordmarkDetails>();detail.Curve=curve;if(!curve){var node=new GameObject("WordmarkElevator",typeof(RectTransform),typeof(CanvasRenderer),typeof(FloorWordmarkLift));node.transform.SetParent(view.transform,false);detail._lift=node.GetComponent<FloorWordmarkLift>();detail._lift.raycastTarget=false;}view.havePropertiesChanged=true;}
  void Awake()=>_text=GetComponent<TextMeshProUGUI>();
  void OnEnable()=>TMPro_EventManager.TEXT_CHANGED_EVENT.Add(Changed);
  void OnDisable()=>TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(Changed);
  void Changed(Object changed)
  {
   if(changed!=_text||_text.textInfo.characterCount==0)return;var info=_text.textInfo;
   if(Curve){float left=float.PositiveInfinity,right=float.NegativeInfinity;
    for(int i=0;i<info.characterCount;i++)if(info.characterInfo[i].isVisible){left=Mathf.Min(left,info.characterInfo[i].bottomLeft.x);right=Mathf.Max(right,info.characterInfo[i].topRight.x);}
    float width=Mathf.Max(1,right-left),wordCenter=(left+right)*.5f;
    for(int i=0;i<info.characterCount;i++){var letter=info.characterInfo[i];if(!letter.isVisible)continue;var vertices=info.meshInfo[letter.materialReferenceIndex].vertices;float center=(letter.bottomLeft.x+letter.topRight.x)*.5f;float x=Mathf.Clamp((center-wordCenter)/(width*.5f),-1,1);var pivot=new Vector3(center,letter.baseLine,0);var rotation=Quaternion.Euler(0,0,-x*9);
     for(int corner=0;corner<4;corner++){int index=letter.vertexIndex+corner;vertices[index]=pivot+rotation*(vertices[index]-pivot)+Vector3.up*(1-x*x)*_text.fontSize*.28f;}
    }_text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);return;
   }
   // TMP raises this during Canvas graphic rebuild. Creating/moving another
   // Graphic here would recursively register it and flood the player Console.
   _placeLift=true;
  }
  void LateUpdate()
  {
   if(!_placeLift||_lift==null)return;_placeLift=false;var info=_text.textInfo;
   int word=_text.text.IndexOf("FLOOR",System.StringComparison.Ordinal);if(word<0||word+2>=info.characterCount)return;var target=info.characterInfo[word+2];float height=target.topRight.y-target.bottomLeft.y;
   var rect=_lift.rectTransform;rect.anchorMin=rect.anchorMax=Vector2.one*.5f;rect.pivot=Vector2.one*.5f;rect.anchoredPosition=(target.topRight+target.bottomLeft)*.5f;rect.sizeDelta=new Vector2(height*.32f,height*.70f);_lift.color=_text.color;_lift.SetVerticesDirty();
  }
 }
 public sealed class FloorWordmarkLift:MaskableGraphic
 {
  protected override void OnPopulateMesh(VertexHelper mesh)
  {
   mesh.Clear();var r=rectTransform.rect;Vector2 P(float x,float y)=>new(r.xMin+x*r.width,r.yMin+y*r.height);
   void Poly(params Vector2[] points){int first=mesh.currentVertCount;foreach(var point in points)mesh.AddVert(point,color,Vector2.zero);for(int i=1;i<points.Length-1;i++)mesh.AddTriangle(first,first+i,first+i+1);}
   void Box(float x,float y,float width,float height)=>Poly(P(x,y),P(x+width,y),P(x+width,y+height),P(x,y+height));
   Box(.10f,.06f,.80f,.09f);Box(.10f,.18f,.14f,.56f);Box(.76f,.18f,.14f,.56f);Box(.45f,.18f,.10f,.51f);Box(.09f,.70f,.82f,.08f);
   Poly(P(.10f,.78f),P(.21f,.92f),P(.5f,.98f),P(.79f,.92f),P(.90f,.78f));
  }
 }
}
