#if UNITY_EDITOR
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.UI;
public static class FloorMenuConceptValidation
{
 static void Fixture(Action<FloorHud,RectTransform,FloorProfile> inspect)
 {
  var root=new GameObject("ActualMenuConceptFixture");Canvas canvas=null;
  try{
   var course=root.AddComponent<FloorCourse>();var profile=new FloorProfile{UnlockedRealm=2,Crystals=428,TutorialCompleted=true};profile.Stars=new[]{0,1,3};profile.Normalize();typeof(FloorCourse).GetProperty("Profile").SetValue(course,profile);
   course.Player=new GameObject("ActualMenuActor").transform;course.Player.SetParent(root.transform,false);var hud=root.AddComponent<FloorHud>();hud.Initialize(course);
   var safe=(RectTransform)typeof(FloorHud).GetField("_safe",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();inspect(hud,safe,profile);
   if(profile.Crystals!=428||profile.Step!=0||profile.Stars[0]!=0||profile.Stars[1]!=1||profile.Stars[2]!=3)throw new InvalidOperationException("Menu asset integration changed live economy/progression");
  }finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}
 }
 public static void ValidateCoin()=>Fixture((hud,safe,profile)=>{
  var original=Resources.Load<Sprite>("UI/Concept/GoldCoin-v1");var icon=safe.Find("Crystals/CrystalIcon/Icon").GetComponent<Graphic>();
  if(original==null||icon.mainTexture!=original.texture||icon.color!=Color.white)throw new InvalidOperationException("MENU_CONCEPT: actual currency icon must consume the original transparent dimensional coin without gold tint");
  var wallet=safe.Find("ModalBackdrop/ModalCard/LobbyWallet/Coin/Icon").GetComponent<Graphic>();if(wallet.mainTexture!=original.texture||wallet.raycastTarget)throw new InvalidOperationException("MENU_CONCEPT: lobby/HUD share the real coin without intercepting touches");
  Debug.Log("FLOOR_MENU_COIN_PASS actual HUD and lobby consume original alpha coin");
 });
 public static void ValidateCards()=>Fixture((hud,safe,profile)=>{
  var content=safe.Find("ModalBackdrop/ModalCard/Viewport/Content");var names=new[]{"Sunset","Neon","Skyline"};
  for(int i=0;i<3;i++){
   var tab=content.Find("Realm"+i);var art=tab.Find("WorldArtwork");var texture=Resources.Load<Texture2D>("UI/Concept/Cards/"+names[i]+"-v1");
   if(art==null||texture==null||art.GetComponent<RawImage>().texture!=texture||art.GetComponent<RawImage>().raycastTarget)throw new InvalidOperationException("MENU_CONCEPT: visible world card must consume its own original scenic artwork "+names[i]);
   if(tab.Find("ChapterLegend")!=null)throw new InvalidOperationException("MENU_CONCEPT: title/star box must not obscure scenic world card");
   int filled=0;for(int star=0;star<3;star++){var glyph=tab.Find("RatingStar"+star).GetComponentInChildren<FloorLobbyGlyph>();if(!glyph.Outlined)filled++;}
   if(filled!=profile.Stars[i])throw new InvalidOperationException("MENU_CONCEPT: stars must remain earned live data "+names[i]);
  }Debug.Log("FLOOR_MENU_CARDS_PASS three actual scenic textures and live0/1/3 ratings");
 });
 public static void ValidateType()=>Fixture((hud,safe,profile)=>{
  var content=safe.Find("ModalBackdrop/ModalCard/Viewport/Content");
  for(int i=0;i<3;i++){
   var card=content.Find("Realm"+i);var title=card.GetComponentInChildren<TextMeshProUGUI>();
   if(title==null||title.font!=Resources.Load<TMP_FontAsset>("Fonts/FloorConceptSerif-Bold SDF"))throw new InvalidOperationException("MENU_CONCEPT: chapter titles require actual mixed-case serif face, not small capitals");
   title.ForceMeshUpdate(); // Dynamic TMP glyphs populate when the actual label renders, also after build cache clearing.
   var lower=title.font.characterLookupTable['u'].glyph.metrics;var upper=title.font.characterLookupTable['S'].glyph.metrics;
   if(lower.height>=upper.height*.85f)throw new InvalidOperationException("MENU_CONCEPT: title face must have a real lowercase x-height");
  }
  var action=hud.Jump.GetComponentInChildren<TextMeshProUGUI>();if(action==null||action.font!=Resources.Load<TMP_FontAsset>("Fonts/Barlow-Bold SDF"))throw new InvalidOperationException("MENU_CONCEPT: gameplay primary action needs the heavy readable Concept sans face");
  var gold=Resources.Load<Sprite>("UI/Concept/GoldAction-v2");var actionImage=hud.Jump.GetComponent<Image>();
  if(gold==null||actionImage.sprite!=gold||actionImage.type!=Image.Type.Sliced)throw new InvalidOperationException("MENU_CONCEPT: actual primary action must consume the new golden nine-slice");
  var pixels=gold.texture;float low=pixels.GetPixel(pixels.width/2,Mathf.RoundToInt(pixels.height*.15f)).g,high=pixels.GetPixel(pixels.width/2,Mathf.RoundToInt(pixels.height*.85f)).g;
  if(high-low<.2f)throw new InvalidOperationException("MENU_CONCEPT: gold primary action needs real strong dimensional color variation");
  Debug.Log("FLOOR_MENU_TYPE_PASS actual mixed-case serif cards and heavy sans gameplay action");
 });
 public static void ValidateWordmark()=>Fixture((hud,safe,profile)=>{
  // Exercise actual Canvas graphic rebuild, not only TMP ForceMeshUpdate.
  for(int frame=0;frame<4;frame++){
   foreach(var detail in safe.GetComponentsInChildren<FloorWordmarkDetails>()){
    var text=detail.GetComponent<TextMeshProUGUI>();text.havePropertiesChanged=true;text.SetVerticesDirty();
   }
   Canvas.ForceUpdateCanvases();
   foreach(var detail in safe.GetComponentsInChildren<FloorWordmarkDetails>())typeof(FloorWordmarkDetails).GetMethod("LateUpdate",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(detail,null);
  }
  var lifts=safe.GetComponentsInChildren<FloorWordmarkLift>();if(lifts.Length!=1||lifts[0].rectTransform.rect.width<=0||lifts[0].raycastTarget)throw new InvalidOperationException("MENU_CONCEPT: one visible live wordmark elevator outside Canvas rebuild");
  Debug.Log("FLOOR_MENU_WORDMARK_PASS real Canvas rebuild and deferred live elevator placement");
 });
 public static void ValidateChevron()
 {
  foreach(var size in new[]{new Vector2(32,32),new Vector2(80,24),new Vector2(24,80)}){
   var node=new GameObject("ActualClimbChevron",typeof(RectTransform),typeof(CanvasRenderer),typeof(HudIcons));var mesh=new Mesh();
   try{var icon=node.GetComponent<HudIcons>();icon.Icon=HudIcons.Kind.ChevronUp;icon.rectTransform.sizeDelta=size;
    using(var vertices=new VertexHelper()){typeof(HudIcons).GetMethod("OnPopulateMesh",BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{typeof(VertexHelper)},null).Invoke(icon,new object[]{vertices});vertices.FillMesh(mesh);}
    var point=new Vector2(0,-Mathf.Min(size.x,size.y)*.13f);var points=mesh.vertices;var indices=mesh.triangles;
    float Side(Vector2 a,Vector2 b)=> (point.x-b.x)*(a.y-b.y)-(a.x-b.x)*(point.y-b.y);
    for(int i=0;i<indices.Length;i+=3){var a=(Vector2)points[indices[i]];var b=(Vector2)points[indices[i+1]];var d=(Vector2)points[indices[i+2]];float ab=Side(a,b),bd=Side(b,d),da=Side(d,a);
     if(!((ab<0||bd<0||da<0)&&(ab>0||bd>0||da>0)))throw new InvalidOperationException("MENU_CONCEPT: climb caret lower opening must remain empty, not a filled triangle fan");
    }
    float aspect=mesh.bounds.size.x/mesh.bounds.size.y;if(aspect<.9f||aspect>1.3f)throw new InvalidOperationException("MENU_CONCEPT: climb chevron must retain its shape in wide/narrow action panels, aspect="+aspect);
   }finally{UnityEngine.Object.DestroyImmediate(mesh);UnityEngine.Object.DestroyImmediate(node);}
  }Debug.Log("FLOOR_MENU_CHEVRON_PASS actual generated graphic shape independent of panel aspect");
 }
 public static void Validate(){ValidateCoin();ValidateCards();ValidateType();ValidateWordmark();ValidateChevron();}
}
#endif
