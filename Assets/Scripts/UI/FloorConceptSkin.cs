using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.OneMoreFloor.UI
{
 public static class FloorConceptSkin
 {
  public static Color FillColor(Image image)
  {
   if(image.sprite==null)return image.color;
   var sprite=image.sprite;var center=sprite.rect.center;
   return sprite.texture.GetPixel(Mathf.RoundToInt(center.x),Mathf.RoundToInt(center.y))*image.color;
  }
  public static void Apply(Image image,string name)
  {
   image.sprite=Resources.Load<Sprite>("UI/Concept/"+(name=="GoldAction"?"GoldAction-v2":name));image.type=Image.Type.Sliced;image.material=null;
   image.color=Color.white;image.pixelsPerUnitMultiplier=name=="GoldAction"||name=="TealAction"?2:3;
  }
  public static void Button(Button button,string name)
  {
   Apply(button.GetComponent<Image>(),name);var colours=button.colors;colours.normalColor=Color.white;colours.highlightedColor=Color.white;
   colours.pressedColor=new Color(.92f,.89f,.82f);colours.disabledColor=new Color(.80f,.80f,.80f);button.colors=colours;
   foreach(var text in button.GetComponentsInChildren<Text>())text.color=name=="TealAction"||name=="GlassBanner"?new Color(1,.96f,.85f):new Color(.055f,.085f,.09f);
  }
 }
}
