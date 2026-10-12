#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorAssetConsumptionValidation
{
 public static void Validate()
 {
  // Artist-created assets must enter the player. Exported unused prefabs alone
  // do not satisfy the user's request to work with Unity's actual asset pipeline.
  var set=Resources.Load<ScriptableObject>("Models/ConceptArtSet");
  if(set==null)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: shipping resource catalogue does not reference the editable original model prefabs");
  foreach(var name in new[]{"Boy","Cabin","Balcony","Atrium"}){
   var field=set.GetType().GetField(name);var prefab=field?.GetValue(set) as GameObject;
   if(prefab==null||prefab.GetComponentsInChildren<MeshFilter>(true).Length==0)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: missing persisted model geometry "+name);
  }
  var catalogue=(FloorAuthoredArtSet)set;var root=new GameObject("PersistedModelConsumptionFixture");
  try{
   var a=catalogue.CreateBoy(root.transform);var b=catalogue.CreateBoy(root.transform);var source=catalogue.Boy.GetComponent<RunnerAnimator>();
   var sourceFace=source.Head.Find("FaceSkin").GetComponent<MeshFilter>().sharedMesh;var face=a.Head.Find("FaceSkin").GetComponent<MeshFilter>().sharedMesh;
   if(!EditorUtility.IsPersistent(sourceFace)||sourceFace==face||sourceFace.vertexCount!=face.vertexCount)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: actual asset geometry must be instantiated with independent owned meshes");
   if(a.Hoodie==b.Hoodie||a.Hoodie==source.Hoodie)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: selected outfit must never mutate original artist materials");
   var sourceColour=source.Hoodie.color;var peerColour=b.Hoodie.color;a.Style(7);
   if(source.Hoodie.color!=sourceColour||b.Hoodie.color!=peerColour)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: paid style leaked into original/peer asset");
   UnityEngine.Object.DestroyImmediate(a.gameObject);if(face!=null||sourceFace==null)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: owned clone released but editable source retained");
   if(root.GetComponentsInChildren<Collider>(true).Length!=0)throw new InvalidOperationException("AUTHORED_ASSET_FLOW: visual assets introduced physics authority");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
  Debug.Log("FLOOR_AUTHORED_ASSET_FLOW_PASS original persistent models / independent instance materials / immutable sources / teardown");
 }
}
#endif
