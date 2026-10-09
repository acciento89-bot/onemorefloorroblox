using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamilunavo.OneMoreFloor.Input
{
    public sealed class PressButton:MonoBehaviour,IPointerDownHandler
    {
        public void ResetInput()=>_pressed=false;private void OnDisable()=>ResetInput();private bool _pressed;public event System.Action Pressed;public void OnPointerDown(PointerEventData e){_pressed=true;Pressed?.Invoke();}public bool Consume(){if(!_pressed)return false;_pressed=false;return true;}
        public static PressButton Create(Transform p,string label,Vector2 min,Vector2 max,bool wide=false){var go=new GameObject(label+"Button",typeof(RectTransform),wide?typeof(Kamilunavo.OneMoreFloor.UI.RoundedPanel):typeof(Kamilunavo.OneMoreFloor.UI.CircleImage),typeof(PressButton));go.transform.SetParent(p,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;go.GetComponent<Image>().color=new Color(.03f,.06f,.10f,.74f);UI.UiFactory.Label(go.transform,"Label",label,46,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);return go.GetComponent<PressButton>();}
    }
}
