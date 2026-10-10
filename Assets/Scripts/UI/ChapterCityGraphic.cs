using UnityEngine;
namespace Kamilunavo.OneMoreFloor.UI
{
 // Cached real 3D chapter art; the left ink panel provides stable contrast over all chapters.
 public static class ChapterCityGraphic
 {
  public static void Create(Transform parent,int chapter){var scene=LiftScenePreview.CreateCity(parent,chapter);scene.anchorMin=new Vector2(.39f,0);scene.anchorMax=Vector2.one;scene.offsetMin=new Vector2(0,4);scene.offsetMax=new Vector2(-4,-4);var band=UiFactory.Panel(parent,"ChapterInk",new Color(.035f,.11f,.14f),Vector2.zero,new Vector2(.39f,1));band.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;var edge=UiFactory.Panel(parent,"ChapterCopper",new Color(.89f,.62f,.25f),new Vector2(.39f,0),new Vector2(.40f,1));edge.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;}
 }
}
