#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorConceptPolishValidation
{
 static void Check(bool value,string label){if(!value)throw new InvalidOperationException("CONCEPT_POLISH: "+label);}
 public static void Validate(){var host=new GameObject("ConceptProportionFixture");try{
  var runner=RunnerArt.Build(host.transform);var face=runner.Head.Find("FaceSkin").GetComponent<Renderer>().bounds;
  Check(face.size.x>.45f&&face.size.x<.52f,"Concept adventurer has a larger childlike head silhouette");
  var sole=runner.LeftKnee.Find("SneakerSoleEdge").GetComponent<Renderer>().bounds;Check(Mathf.Abs(sole.min.y)<.025f,"shorter travel body keeps its feet on the actual platform");
  var body=runner.GetComponentsInChildren<Renderer>();Bounds visible=body[0].bounds;foreach(var part in body)visible.Encapsulate(part.bounds);Check(visible.max.y<1.90f,"child proportions stay below carriage ceiling and gameplay framing envelope");
  LiftArchitecture.Bay(host.transform,0);var bay=host.transform.Find("ArchitecturalFloorBay");var door=bay.Find("RecessedTealDoor").GetComponent<Renderer>().bounds;var column=bay.Find("FacadeColumn").GetComponent<Renderer>().bounds;Check(door.min.z>column.max.z+.80f,"door sits inside a real deep entrance rather than on a flat facade");
  var renderers=bay.GetComponentsInChildren<Renderer>();Bounds facade=renderers[0].bounds;foreach(var part in renderers)facade.Encapsulate(part.bounds);Check(facade.size.x>5.5f,"tower has the substantial residential facade mass from Concept");Check(host.GetComponentsInChildren<Collider>().Length==0,"visual rebuild does not introduce physics authority");
  Debug.Log("FLOOR_CONCEPT_POLISH_PASS actualConceptReview=REQUIRED");
 }finally{UnityEngine.Object.DestroyImmediate(host);}}
}
#endif
