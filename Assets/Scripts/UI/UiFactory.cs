using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamilunavo.OneMoreFloor.UI
{
    public static class UiFactory
    {
        private static Font _font;
        public static Font Font => _font != null ? _font : (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        public static Canvas Canvas()
        {
            var go = new GameObject("MobileCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static RectTransform Panel(Transform parent, string name, Color color, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>();
            r.anchorMin=min; r.anchorMax=max; r.offsetMin=Vector2.zero; r.offsetMax=Vector2.zero;
            go.GetComponent<Image>().color=color;
            return r;
        }

        public static Text Label(Transform parent, string name, string text, int size, Vector2 min, Vector2 max, TextAnchor align, Color color, FontStyle style=FontStyle.Normal)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));
            go.transform.SetParent(parent,false);
            var r=go.GetComponent<RectTransform>(); r.anchorMin=min; r.anchorMax=max; r.offsetMin=Vector2.zero; r.offsetMax=Vector2.zero;
            var t=go.GetComponent<Text>(); t.font=Font; t.text=text; t.fontSize=size; t.alignment=align; t.color=color; t.fontStyle=style; t.resizeTextForBestFit=true; t.resizeTextMinSize=12; t.resizeTextMaxSize=size;
            return t;
        }

        public static Button Button(Transform parent,string name,string label,Color bg,Color fg,Vector2 min,Vector2 max,UnityAction action)
        {
            var r=Panel(parent,name,bg,min,max);
            var b=r.gameObject.AddComponent<Button>(); if(action!=null)b.onClick.AddListener(action);
            Label(r,"Label",label,32,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,fg,FontStyle.Bold);
            return b;
        }

        public static Image Progress(Transform parent, Vector2 min, Vector2 max, Color track, Color fill)
        {
            var tr=Panel(parent,"ProgressTrack",track,min,max);
            var fr=Panel(tr,"Fill",fill,Vector2.zero,Vector2.one);
            var image=fr.GetComponent<Image>(); image.type=Image.Type.Filled; image.fillMethod=Image.FillMethod.Horizontal; image.fillAmount=0f;
            return image;
        }
    }
}
