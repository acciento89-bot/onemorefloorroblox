using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Shipping catalogue references original editable prefabs. The same objects
 // seen in Unity's Art Studio become the actual live game/menu geometry.
 public sealed class FloorAuthoredArtSet:ScriptableObject
 {
  public GameObject Boy,Cabin,Balcony,Atrium;
  static FloorAuthoredArtSet _loaded;
  public static FloorAuthoredArtSet Runtime=>Application.isPlaying?(_loaded!=null?_loaded:_loaded=Resources.Load<FloorAuthoredArtSet>("Models/ConceptArtSet")):null;
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset()=>_loaded=null;
  public RunnerAnimator CreateBoy(Transform parent)
  {
   var model=Clone(Boy,parent,"HoodieRunner",false);var rig=model.GetComponent<RunnerAnimator>();rig.CaptureRestPose();return rig;
  }
  public GameObject CreateKit(GameObject prefab,Transform parent,string name)=>Clone(prefab,parent,name,true);
  static GameObject Clone(GameObject prefab,Transform parent,string name,bool architecture)
  {
   if(prefab==null)throw new InvalidOperationException("Missing authored concept prefab "+name);
   var model=Instantiate(prefab,parent,false);model.name=name;
   // Each root retains the existing owned-mesh/batching contract. Editable
   // source meshes/textures remain immutable and survive menu/scene teardown.
   var meshes=new Dictionary<Mesh,Mesh>();var materials=new Dictionary<Material,Material>();
   foreach(var filter in model.GetComponentsInChildren<MeshFilter>(true)){
    var source=filter.sharedMesh;if(!meshes.TryGetValue(source,out var copy)){copy=Instantiate(source);copy.name=source.name;meshes.Add(source,copy);ArtLifetime.Own(model,copy);}filter.sharedMesh=copy;
   }
   foreach(var renderer in model.GetComponentsInChildren<MeshRenderer>(true)){
    var slots=renderer.sharedMaterials;for(int i=0;i<slots.Length;i++){
     var source=slots[i];if(source==null)continue;
     if(architecture&&source.name.StartsWith("FloorArchitecture",StringComparison.Ordinal)&&Enum.TryParse(source.name.Substring("FloorArchitecture".Length),out FloorArchitectureSurface surface))slots[i]=FloorArchitecturePalette.Get(parent,surface);
     else if(source.shader.name!="GUI/Text Shader"){
      if(!materials.TryGetValue(source,out var copy)){copy=Instantiate(source);copy.name=source.name;materials.Add(source,copy);ArtLifetime.Own(model,copy);}slots[i]=copy;
     }
    }renderer.sharedMaterials=slots;
   }
   foreach(var rig in model.GetComponentsInChildren<RunnerAnimator>(true))if(materials.TryGetValue(rig.Hoodie,out var hoodie))rig.Hoodie=hoodie;
   return model;
  }
 }
}
