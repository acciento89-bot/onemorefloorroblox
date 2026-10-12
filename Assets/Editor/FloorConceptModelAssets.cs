#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorConceptModelAssets
{
 // Editable authoring counterpart of the same owned rig generated in gameplay.
 // Copies become assets; transient originals retain their existing teardown.
 public static void Export()
 {
  const string folder="Assets/Models/Concept";
  if(AssetDatabase.LoadAssetAtPath<GameObject>(folder+"/ConceptBoy.prefab")!=null)return;
  Directory.CreateDirectory(folder);AssetDatabase.Refresh();var host=new GameObject("ConceptBoyAuthoring");
  try
  {
   var actor=RunnerArt.Build(host.transform);var materials=new Dictionary<Material,Material>();var textures=new Dictionary<Texture,Texture>();bool first=true;
   void Save(Object asset){if(first){AssetDatabase.CreateAsset(asset,folder+"/ConceptBoyGeometry.asset");first=false;}else AssetDatabase.AddObjectToAsset(asset,folder+"/ConceptBoyGeometry.asset");}
   foreach(var filter in actor.GetComponentsInChildren<MeshFilter>(true)){var copy=Object.Instantiate(filter.sharedMesh);copy.name=filter.sharedMesh.name;Save(copy);filter.sharedMesh=copy;}
   foreach(var renderer in actor.GetComponentsInChildren<MeshRenderer>(true))
   {
    var source=renderer.sharedMaterial;
    if(!materials.TryGetValue(source,out var copy))
    {
     copy=Object.Instantiate(source);copy.name=source.name;Save(copy);materials.Add(source,copy);
     if(source.mainTexture!=null&&!EditorUtility.IsPersistent(source.mainTexture))
     {
      if(!textures.TryGetValue(source.mainTexture,out var texture)){texture=Object.Instantiate(source.mainTexture);texture.name=source.mainTexture.name;Save(texture);textures.Add(source.mainTexture,texture);}
      copy.mainTexture=texture;
     }
    }
    renderer.sharedMaterial=copy;
   }
   actor.Hoodie=materials[actor.Hoodie];actor.name="ConceptBoy";
   PrefabUtility.SaveAsPrefabAsset(actor.gameObject,folder+"/ConceptBoy.prefab");AssetDatabase.SaveAssets();
   Debug.Log("FLOOR_CONCEPT_MODEL_EXPORTED actual owned gameplay rig / original meshes / green jacket / brown hair / travel pack");
  }
  finally{Object.DestroyImmediate(host);}
 }
}
#endif
