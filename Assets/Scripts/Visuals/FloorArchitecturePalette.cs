using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 public enum FloorArchitectureSurface { Stone,Trim,Brass,Copper,Teal,Leaf,LeafLight,Lamp,Deck,CityStone,CityShade,CityTrim,Glass }
 // Thirteen finite surface kinds and two original 128² finishes per course/preview root.
 // No static generated assets survive a scene or a menu preview.
 [ExecuteAlways] public sealed class FloorArchitecturePalette:MonoBehaviour
 {
  readonly Dictionary<FloorArchitectureSurface,Material> _materials=new();Texture2D _stone,_metal;
  public static Material Get(Transform parent,FloorArchitectureSurface surface)
  {
   var course=parent.GetComponentInParent<FloorCourse>();var inherited=parent.GetComponentInParent<FloorArchitecturePalette>();
   var owner=course!=null?course.gameObject:inherited!=null?inherited.gameObject:parent.gameObject;
   var palette=owner.GetComponent<FloorArchitecturePalette>()??owner.AddComponent<FloorArchitecturePalette>();return palette.Material(surface);
  }
  static float Hash(int x,int y){unchecked{uint h=(uint)(x*374761393+y*668265263);h=(h^(h>>13))*1274126177;return ((h^(h>>16))&65535)/65535f;}}
  static float Noise(float x,float y,int period)
  {
   int ix=Mathf.FloorToInt(x),iy=Mathf.FloorToInt(y);float fx=x-ix,fy=y-iy;fx=fx*fx*(3-2*fx);fy=fy*fy*(3-2*fy);
   float H(int a,int b)=>Hash((a%period+period)%period,(b%period+period)%period);
   return Mathf.Lerp(Mathf.Lerp(H(ix,iy),H(ix+1,iy),fx),Mathf.Lerp(H(ix,iy+1),H(ix+1,iy+1),fx),fy);
  }
  Texture2D Finish(bool metal)
  {
   const int size=128;var pixels=new Color[size*size];
   for(int y=0;y<size;y++)for(int x=0;x<size;x++){
    float u=x/(float)size,v=y/(float)size,broad=Noise(u*5,v*5,5),grain=Noise(u*37,v*37,37);
    float value=metal?.88f+(broad-.5f)*.08f+(grain-.5f)*.025f:.87f+(broad-.5f)*.12f+(grain-.5f)*.035f;
    if(!metal){float vein=Mathf.Abs(Mathf.Sin(u*12.56637f+Mathf.Sin(v*6.28318f)*.5f));value-=(1-Mathf.Clamp01(vein/.025f))*Mathf.Clamp01((broad-.55f)*4)*.06f;}
    pixels[y*size+x]=new Color(value,value,value*(metal?1:.975f));
   }
   var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name=metal?"FloorArchitectureBrushedMetal":"FloorArchitectureMineral",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear,anisoLevel=2};texture.SetPixels(pixels);texture.Apply(true,true);ArtLifetime.Own(gameObject,texture);return texture;
  }
  Material Material(FloorArchitectureSurface kind)
  {
   if(_materials.TryGetValue(kind,out var cached))return cached;
   var material=new Material(Resources.Load<Material>(kind==FloorArchitectureSurface.Lamp?"Materials/Glow":"Materials/Surface")){name="FloorArchitecture"+kind};
   material.color=kind switch {
    FloorArchitectureSurface.Stone=>new Color(.84f,.79f,.66f),FloorArchitectureSurface.Trim=>new Color(.91f,.85f,.72f),
    FloorArchitectureSurface.Brass=>new Color(.84f,.61f,.30f),FloorArchitectureSurface.Copper=>new Color(.67f,.41f,.21f),
    FloorArchitectureSurface.Teal=>new Color(.035f,.19f,.20f),FloorArchitectureSurface.Leaf=>new Color(.13f,.30f,.14f),FloorArchitectureSurface.LeafLight=>new Color(.30f,.44f,.17f),
    FloorArchitectureSurface.Lamp=>new Color(1,.73f,.31f),FloorArchitectureSurface.Deck=>new Color(.36f,.23f,.13f),
    FloorArchitectureSurface.CityStone=>new Color(.70f,.58f,.43f),FloorArchitectureSurface.CityShade=>new Color(.55f,.44f,.34f),FloorArchitectureSurface.Glass=>new Color(.35f,.64f,.64f,.09f),_=>new Color(.82f,.71f,.53f)};
   bool metal=kind==FloorArchitectureSurface.Brass||kind==FloorArchitectureSurface.Copper;
   if(metal)material.shader=Resources.Load<Shader>("FloorArchitecturalMetal");
   if(kind==FloorArchitectureSurface.Glass)material.shader=Resources.Load<Shader>("FloorCabinGlass");
   if(metal){if(_metal==null)_metal=Finish(true);material.mainTexture=_metal;}
   else if(kind==FloorArchitectureSurface.Stone||kind==FloorArchitectureSurface.Trim||kind==FloorArchitectureSurface.Deck||kind==FloorArchitectureSurface.CityStone||kind==FloorArchitectureSurface.CityShade||kind==FloorArchitectureSurface.CityTrim){material.shader=Resources.Load<Shader>("FloorMasonry");if(_stone==null)_stone=Finish(false);material.mainTexture=_stone;}
   material.SetFloat("_Glossiness",metal?.72f:.12f);if(material.HasProperty("_Metallic"))material.SetFloat("_Metallic",metal?.72f:0);
   if(kind==FloorArchitectureSurface.Lamp){material.EnableKeyword("_EMISSION");material.SetColor("_EmissionColor",material.color*.65f);}
   ArtLifetime.Own(gameObject,material);_materials.Add(kind,material);return material;
  }
 }
}
