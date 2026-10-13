#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
// Original code-native UI shapes rendered in the native Unity Editor. Menu
// paintings/coin retain their separately documented built-in Imagegen pixels.
public static class FloorMenuAssetsAuthoring
{
 const string Folder="Assets/Resources/UI/Concept/";
 static float Rounded(Vector2 point,Vector2 half,float radius){var q=new Vector2(Mathf.Abs(point.x),Mathf.Abs(point.y))-half+Vector2.one*radius;return new Vector2(Mathf.Max(q.x,0),Mathf.Max(q.y,0)).magnitude+Mathf.Min(Mathf.Max(q.x,q.y),0)-radius;}
 static void Shape(string name,int width,int height,bool frame,bool clip)
 {
  if(File.Exists(Folder+name+".png"))return;
  var image=new Texture2D(width,height,TextureFormat.RGBA32,false);var pixels=new Color[width*height];
  for(int y=0;y<height;y++)for(int x=0;x<width;x++){
   var p=new Vector2(x+.5f-width*.5f,y+.5f-height*.5f);var half=new Vector2(width*.5f-3,height*.5f-4);float distance=Rounded(p,half,26);Color colour=Color.clear;
   if(distance<=1){float vertical=y/(float)height;
    colour=clip?Color.white:Color.Lerp(new Color(.66f,.36f,.035f),new Color(1,.83f,.30f),Mathf.Pow(vertical,.65f));
    if(!clip){colour=Color.Lerp(colour,new Color(1,.93f,.63f),Mathf.Exp(-Mathf.Pow((vertical-.88f)*18,2))*.65f);
     if(distance> -3)colour=new Color(.27f,.15f,.055f);else if(distance> -5)colour=new Color(1,.87f,.40f);else if(distance> -7)colour=new Color(1,.97f,.78f);
     else if(distance> -9)colour=new Color(.77f,.47f,.10f);
     if(frame&&distance< -9)colour=Color.clear;
    }colour.a*=Mathf.Clamp01(1-distance);
   }pixels[y*width+x]=colour;
  }image.SetPixels(pixels);image.Apply();File.WriteAllBytes(Folder+name+".png",image.EncodeToPNG());Object.DestroyImmediate(image);
 }
 static void Sprite(string name,int limit,Vector4 border)
 {
  var importer=(TextureImporter)AssetImporter.GetAtPath(Folder+name+".png");importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spriteBorder=border;importer.spritePixelsPerUnit=100;importer.maxTextureSize=limit;importer.isReadable=true;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
 }
 static void Font(string name)
 {
  string path="Assets/Resources/Fonts/"+name+" SDF.asset";if(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path)!=null)return;
  var font=TMP_FontAsset.CreateFontAsset(Resources.Load<Font>("Fonts/"+name),72,8,UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,1024,1024,AtlasPopulationMode.Dynamic,true);font.name=name+" SDF";
  font.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ÄÖÜäöüß€$.,:;!?/()+−-×·›‹↑↓%&\"'");AssetDatabase.CreateAsset(font,path);AssetDatabase.AddObjectToAsset(font.material,font);foreach(var texture in font.atlasTextures)AssetDatabase.AddObjectToAsset(texture,font);EditorUtility.SetDirty(font);
 }
 public static void Configure()
 {
  Shape("GoldAction-v2",512,128,false,false);Shape("WorldFrame-v1",512,256,true,false);Shape("CardClip-v1",512,256,false,true);AssetDatabase.Refresh();
  Sprite("GoldAction-v2",512,new Vector4(28,28,28,28));Sprite("WorldFrame-v1",512,new Vector4(28,28,28,28));Sprite("CardClip-v1",512,new Vector4(28,28,28,28));Sprite("GoldCoin-v1",256,Vector4.zero);
  foreach(var name in new[]{"Sunset","Neon","Skyline"}){var importer=(TextureImporter)AssetImporter.GetAtPath(Folder+"Cards/"+name+"-v1.png");importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.maxTextureSize=1024;importer.mipmapEnabled=false;importer.isReadable=true;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Compressed;importer.SaveAndReimport();}
  Font("FloorConceptSerif-Regular");Font("FloorConceptSerif-Bold");AssetDatabase.SaveAssets();Debug.Log("FLOOR_MENU_ASSETS_AUTHORED original textures / true serif faces / gold frame and action / alpha coin");
 }
}
#endif
