using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 // A cropped resource image, with vertex tint keeping the text half dark. No per-open textures or sprites.
 [RequireComponent(typeof(CanvasRenderer))] public sealed class ScenicCityGraphic:RawImage
 {
  private int _tower;private float _strength=1;
  public static RectTransform Create(Transform parent,int tower,float strength=1){var go=new GameObject("ScenicCity",typeof(RectTransform),typeof(CanvasRenderer),typeof(ScenicCityGraphic));go.transform.SetParent(parent,false);var image=go.GetComponent<ScenicCityGraphic>();image._tower=Mathf.Clamp(tower,0,2);image._strength=strength;image.texture=Resources.Load<Texture2D>("Art/SkyPanorama");image.raycastTarget=false;var rect=(RectTransform)go.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=new Vector2(5,5);rect.offsetMax=new Vector2(-5,-5);return rect;}
  protected override void OnPopulateMesh(VertexHelper mesh){mesh.Clear();var rect=rectTransform.rect;float source=texture!=null?(float)texture.width/texture.height:3,aspect=rect.width/Mathf.Max(1,rect.height),crop=Mathf.Min(1,aspect/source),centre=_tower==0?.24f:_tower==1?.72f:.49f;float u=Mathf.Clamp(centre-crop*.5f,0,1-crop);var columns=new[]{0f,.30f,.49f,1f};Color left=_tower==1?new Color(.48f,.72f,1):_tower==2?new Color(.70f,.80f,.94f):new Color(.94f,.81f,.71f),right=new Color(.10f,.15f,.20f);
   Color Tint(float x)=>_strength<1?new Color(_strength,_strength,_strength,1):Color.Lerp(left,right,Mathf.Clamp01((x-.30f)/.19f));
   for(int i=0;i<columns.Length-1;i++){float x=columns[i],next=columns[i+1];int start=mesh.currentVertCount;mesh.AddVert(new Vector3(rect.xMin+x*rect.width,rect.yMin),Tint(x),new Vector2(u+x*crop,0));mesh.AddVert(new Vector3(rect.xMin+x*rect.width,rect.yMax),Tint(x),new Vector2(u+x*crop,1));mesh.AddVert(new Vector3(rect.xMin+next*rect.width,rect.yMax),Tint(next),new Vector2(u+next*crop,1));mesh.AddVert(new Vector3(rect.xMin+next*rect.width,rect.yMin),Tint(next),new Vector2(u+next*crop,0));mesh.AddTriangle(start,start+1,start+2);mesh.AddTriangle(start,start+2,start+3);}
  }
 }
}
