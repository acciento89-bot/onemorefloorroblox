#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;

public static class FloorAuthoredRunnerValidation
{
 public static void Validate()
 {
  var errors=new List<string>();void Log(string text,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors.Add(text);}
  Application.logMessageReceived+=Log;var host=new GameObject("AuthoredTravelRunnerFixture");
  var meshes=new HashSet<Mesh>();int checks=0;
  void Check(bool value,string label){checks++;if(!value)throw new InvalidOperationException("AUTHORED_RUNNER: "+label);}
  try{
   var runner=RunnerArt.Build(host.transform);
   Check(runner.Hips.localPosition==new Vector3(0,.711f,0)&&runner.Head.localPosition==new Vector3(0,.815f,.003f),"shorter child rig retains its intended hips/head origins and ground contact");
   foreach(int side in new[]{-1,1}){
    var knee=side<0?runner.LeftKnee:runner.RightKnee;
    Check(knee.localPosition==new Vector3(0,-.35f,.012f),"knee retains guided animation origin");
    var boot=knee.Find("WhiteSneakerLast").GetComponent<MeshFilter>().sharedMesh.bounds;
    Check(boot.max.y>-.29f&&boot.min.y>=-.491f,"structured ankle collar rises above old low shoe with unchanged sole height");
    Check(boot.max.z<=.249f&&boot.min.z>=-.15f,"toe and heel preserve the existing foot envelope");
   }
   var tunic=runner.Hips.Find("TailoredLiftHoodie").GetComponent<MeshFilter>().sharedMesh;
   float hemMin=float.PositiveInfinity,hemMax=float.NegativeInfinity;
   foreach(var v in tunic.vertices)if(v.y>.035f&&v.y<.053f&&Mathf.Abs(v.x)>.10f){hemMin=Mathf.Min(hemMin,v.y);hemMax=Mathf.Max(hemMax,v.y);}
   Check(hemMax-hemMin>.007f,"cloth hem follows unequal authored side folds instead of a level capsule ring");
   var pack=runner.Hips.Find("CargoWaist").GetComponent<MeshFilter>().sharedMesh;int raisedSeams=0;
   foreach(var v in pack.vertices)if(v.y>.22f&&v.y<.40f&&Mathf.Abs(v.x)>.12f&&Mathf.Abs(v.x)<.15f&&v.z<-.335f)raisedSeams++;
   Check(raisedSeams>=8,"raised back-panel piping has its own surface depth");
   Check(pack.bounds.min.z>-.374f,"back panel stays inside existing premium crest plane");
   var hair=runner.Head.Find("SweptChestnutHair").GetComponent<MeshFilter>().sharedMesh;int rearLocks=0;
   foreach(var v in hair.vertices)if(v.z<-.165f&&v.y>.06f&&v.y<.12f)rearLocks++;
   Check(rearLocks>20,"wide overlapping swept locks reach the rear hair silhouette");
   Check(hair.bounds.max.y<.295f&&hair.bounds.min.z>-.224f&&hair.bounds.size.x<.47f,"authored hair stays within the previous head envelope");
   int vertices=0,triangles=0,renderers=0;
   foreach(var filter in host.GetComponentsInChildren<MeshFilter>(true)){
    var mesh=filter.sharedMesh;meshes.Add(mesh);vertices+=mesh.vertexCount;triangles+=mesh.triangles.Length/3;
    var points=mesh.vertices;var normals=mesh.normals;var indices=mesh.triangles;
    foreach(var p in points)Check(float.IsFinite(p.x)&&float.IsFinite(p.y)&&float.IsFinite(p.z),"finite authored coordinate");
    foreach(var n in normals)Check(float.IsFinite(n.x)&&float.IsFinite(n.y)&&float.IsFinite(n.z)&&n.sqrMagnitude>.5f,"finite lighting normals");
    for(int i=0;i<indices.Length;i+=3)Check(Vector3.Cross(points[indices[i+1]]-points[indices[i]],points[indices[i+2]]-points[indices[i]]).sqrMagnitude>1e-12f,"no degenerate authored triangles");
   }
   foreach(var renderer in host.GetComponentsInChildren<Renderer>(true))if(renderer.gameObject.activeInHierarchy)renderers++;
   Check(vertices<10000&&triangles<10000&&renderers==31,"all detailed surfaces stay in the original 31 default batches and geometry cap");
   Check(host.GetComponentsInChildren<Collider>(true).Length==0,"original art introduces no gameplay collider");
   runner.Style(7);Check(runner.Hips.Find("PremiumCrest7").gameObject.activeSelf,"existing premium selection binds to authored pack");
   UnityEngine.Object.DestroyImmediate(host);foreach(var mesh in meshes)Check(mesh==null,"all authored meshes are released with the avatar");
   Debug.Log("FLOOR_AUTHORED_RUNNER_PASS checks="+checks+" vertices="+vertices+" triangles="+triangles+" defaultRenderers="+renderers);
  }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);Application.logMessageReceived-=Log;}
  if(errors.Count>0)throw new InvalidOperationException("AUTHORED_RUNNER logged errors: "+string.Join("; ",errors));
 }
}
#endif
