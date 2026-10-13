using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 // Existing controllers keep their Text bindings; this owned TMP view renders
 // the live label with a real saved serif or heavy sans SDF. No duplicated authoritative text.
 [ExecuteAlways] public sealed class FloorConceptTypography:MonoBehaviour
 {
  Text _source;TextMeshProUGUI _view;string _fontResource="Barlow-Bold";string _text;Color _ink;int _size;
  public static void Apply(Text source,string face="Barlow-Bold")
  {
   var previous=source.GetComponent<FloorConceptTypography>();if(previous!=null){previous._fontResource=face;previous._view.font=Resources.Load<TMP_FontAsset>("Fonts/"+face+" SDF");return;}
   var adapter=source.gameObject.AddComponent<FloorConceptTypography>();adapter._fontResource=face;adapter.Initialize(source);
  }
  void Initialize(Text source)
  {
   _source=source;_source.enabled=false;
   var root=new GameObject("ConceptDisplay",typeof(RectTransform),typeof(TextMeshProUGUI));root.transform.SetParent(source.transform,false);
   var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
   _view=root.GetComponent<TextMeshProUGUI>();_view.font=Resources.Load<TMP_FontAsset>("Fonts/"+_fontResource+" SDF");
   _view.raycastTarget=false;_view.enableAutoSizing=true;_view.fontSizeMin=11;_view.textWrappingMode=TextWrappingModes.NoWrap;
   // A very restrained dark contour preserves serif detail at compact sizes.
   _view.outlineWidth=.025f;_view.outlineColor=new Color(.045f,.09f,.10f,.32f);
   Synchronize();
  }
  void OnEnable()=>Canvas.willRenderCanvases+=Synchronize;
  void OnDisable()=>Canvas.willRenderCanvases-=Synchronize;
  void Synchronize()
  {
   if(_source==null||_view==null)return;
   if(_text!=_source.text){_text=_source.text;_view.text=_text;}
   if(_ink!=_source.color){_ink=_source.color;_view.color=_ink;_view.outlineColor=_fontResource=="FloorConceptSerif-Bold"&&_ink.r<.5f?new Color(1,.96f,.86f,.65f):new Color(.045f,.09f,.10f,.32f);}
   if(_size!=_source.fontSize){_size=_source.fontSize;_view.fontSize=_size;_view.fontSizeMax=_size;}
   _view.alignment=_source.alignment==TextAnchor.MiddleLeft?TextAlignmentOptions.MidlineLeft:_source.alignment==TextAnchor.MiddleRight?TextAlignmentOptions.MidlineRight:TextAlignmentOptions.Midline;
   _view.fontStyle=_fontResource.EndsWith("-Bold")?FontStyles.Normal:_source.fontStyle==FontStyle.Bold?FontStyles.Bold:FontStyles.Normal;
  }
 }
}
