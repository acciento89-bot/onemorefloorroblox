using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Kamilunavo.OneMoreFloor.UI
{
 // These are original menu illustrations. The live room and checkpoint
 // previews stay real 3D; playable chapter geometry remains independently built.
 public sealed class FloorWorldCardArt:RawImage
 {
  public static readonly string[] Names={"Sunset","Neon","Skyline"};
  public static RectTransform Create(Transform parent,int chapter,bool unlocked)
  {
   var node=new GameObject("WorldArtwork",typeof(RectTransform),typeof(CanvasRenderer),typeof(FloorWorldCardArt));node.transform.SetParent(parent,false);var image=node.GetComponent<FloorWorldCardArt>();image.texture=Resources.Load<Texture2D>("UI/Concept/Cards/"+Names[chapter]+"-v1");image.color=unlocked?Color.white:new Color(.58f,.65f,.65f,1);image.raycastTarget=false;
   var rect=image.rectTransform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=new Vector2(4,4);rect.offsetMax=new Vector2(-4,-4);image.Crop();return rect;
  }
  // Validate actual visible text against the cropped painting, not against the
  // hidden white stencil mask. Button tint affects only its border graphic.
  public float MinimumTextContrast(Text source)
  {
   var display=source.GetComponentInChildren<TextMeshProUGUI>();display.ForceMeshUpdate();var painting=(Texture2D)texture;float lowest=float.PositiveInfinity;
   for(int i=0;i<display.textInfo.characterCount;i++){var letter=display.textInfo.characterInfo[i];if(!letter.isVisible)continue;var point=rectTransform.InverseTransformPoint(display.transform.TransformPoint((letter.bottomLeft+letter.topRight)*.5f));var rect=rectTransform.rect;
    float x=(point.x-rect.xMin)/rect.width,y=(point.y-rect.yMin)/rect.height;var backdrop=painting.GetPixelBilinear(uvRect.x+x*uvRect.width,uvRect.y+y*uvRect.height)*color;
    var shade=transform.parent.GetComponentInChildren<FloorCardCaptionShade>();if(shade!=null)backdrop=Color.Lerp(backdrop,shade.Backdrop,FloorCardCaptionShade.Alpha(new Vector2(x,y)));
    lowest=Mathf.Min(lowest,UiFactory.Contrast(source.color,backdrop));
   }return lowest;
  }
  protected override void OnRectTransformDimensionsChange(){base.OnRectTransformDimensionsChange();Crop();}
  void Crop(){if(texture==null||rectTransform.rect.width<=0||rectTransform.rect.height<=0)return;float source=texture.width/(float)texture.height,display=rectTransform.rect.width/rectTransform.rect.height;
   if(source>display){float width=display/source;uvRect=new Rect((1-width)*.5f,0,width,1);}else{float height=source/display;uvRect=new Rect(0,(1-height)*.5f,1,height);}}
 }
}
