using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamilunavo.OneMoreFloor.Input
{
    public sealed class PressButton:MonoBehaviour,IPointerDownHandler
    {
        private bool _pressed;
        public void OnPointerDown(PointerEventData e)=>_pressed=true;
        public bool Consume(){if(!_pressed)return false;_pressed=false;return true;}
        public static PressButton Create(Transform parent,string label,Vector2 min,Vector2 max,Color color){var go=new GameObject(label+"Button",typeof(RectTransform),typeof(Image),typeof(PressButton));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;go.GetComponent<Image>().color=color;UI.UiFactory.Label(go.transform,"Label",label,42,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);return go.GetComponent<PressButton>();}
    }
}
