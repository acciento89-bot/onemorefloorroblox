#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorCabinProportionValidation
{
 static Bounds BoundsIn(Transform root){bool first=true;Bounds bounds=default;foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true))foreach(var vertex in filter.sharedMesh.vertices){var point=root.parent.InverseTransformPoint(filter.transform.TransformPoint(vertex));if(first){bounds=new Bounds(point,Vector3.zero);first=false;}else bounds.Encapsulate(point);}return bounds;}
 public static void Validate()
 {
  var root=new GameObject("ActualSlenderCarriageFixture");
  try{
   var set=Resources.Load<FloorAuthoredArtSet>("Models/ConceptArtSet");var shipping=set.CreateKit(set.Cabin,root.transform,"ShippingCabin");var actual=BoundsIn(shipping.transform);
   if(actual.size.x/actual.size.y>.38f||actual.size.y<4.1f||actual.size.x<1.3f)throw new InvalidOperationException("CONCEPT_PROPORTIONS: actual shipping carriage must be tall and slender, width="+actual.size.x+" height="+actual.size.y);
   PlatformArt.Build(root.transform,0,0);var fallback=BoundsIn(root.transform.Find("CopperLiftCabin"));if(Mathf.Abs(fallback.size.x-actual.size.x)>.015f||Mathf.Abs(fallback.size.y-actual.size.y)>.015f)throw new InvalidOperationException("CONCEPT_PROPORTIONS: procedural and imported live cabin proportions differ");
   var deck=root.transform.Find("ChamferedDeck");var floor=BoundsIn(deck);if(floor.size.x>1.7f||floor.size.x<1.4f)throw new InvalidOperationException("CONCEPT_PROPORTIONS: real visible passenger deck must fit the slim body, width="+floor.size.x);
   var collider=root.transform.Find("LandingSurface").GetComponent<BoxCollider>();if(collider.size!=new Vector3(4.6f,.7f,3.6f)||collider.center!=new Vector3(0,.25f,0))throw new InvalidOperationException("CONCEPT_PROPORTIONS: timing collision authority changed");
   var threshold=BoundsIn(root.transform.Find("DoorwayThreshold"));if(threshold.max.z<-.48f||threshold.min.z> -1.50f)throw new InvalidOperationException("CONCEPT_PROPORTIONS: visible doorway tread must connect the slim deck through the original walk-out phase");
   Debug.Log("FLOOR_SLENDER_PROPORTION_PASS actual imported/procedural body="+actual.size+" slim deck="+floor.size.x+" supported exit tread / unchanged collision");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
 }
}
#endif
