#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorAuthoredCabinValidation
{
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new InvalidOperationException("AUTHORED_CABIN: "+message);}
 static string Key(Vector3 p)=>Mathf.RoundToInt(p.x*100000)+","+Mathf.RoundToInt(p.y*100000)+","+Mathf.RoundToInt(p.z*100000);
 static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
 static float Area(List<Vector2> points){float area=0;for(int i=0;i<points.Count;i++)area+=Cross(points[i],points[(i+1)%points.Count]);return Mathf.Abs(area)*.5f;}
 static float Overlap(Vector2[] first,Vector2[] second)
 {
  var clipped=new List<Vector2>(first);
  for(int edge=0;edge<3&&clipped.Count>0;edge++){
   var a=second[edge];var b=second[(edge+1)%3];var input=clipped;clipped=new List<Vector2>();
   var previous=input[input.Count-1];float before=Cross(b-a,previous-a);
   foreach(var current in input){float after=Cross(b-a,current-a);if((before>=0)!=(after>=0))clipped.Add(previous+(current-previous)*(before/(before-after)));if(after>=0)clipped.Add(current);previous=current;before=after;}
  }
  return clipped.Count>=3?Area(clipped):0;
 }
 static void Cap(Mesh mesh,float degrees,bool start)
 {
  float angle=degrees*Mathf.Deg2Rad;var radial=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle)*.8f);var plane=Vector3.Cross(radial,Vector3.up).normalized;var outward=start?-plane:plane;
  Vector2 Project(Vector3 point)=>new Vector2(Vector3.Dot(point,radial)/radial.sqrMagnitude,point.y);
  bool AtEnd(Vector3 point)=>Mathf.Abs(Vector3.Dot(point,plane))<.00001f&&Vector3.Dot(point,radial)>0;
  var points=mesh.vertices;var indices=mesh.triangles;var faces=new List<Vector2[]>();float area=0,boundary=0;
  for(int i=0;i<indices.Length;i+=3){var a=points[indices[i]];var b=points[indices[i+1]];var c=points[indices[i+2]];
   if(AtEnd(a)&&AtEnd(b)&&AtEnd(c)){
    Check(Vector3.Dot(Vector3.Cross(b-a,c-a),outward)>1e-8f,"every partial-sweep cap triangle faces its outward plane normal");
    var triangle=new[]{Project(a),Project(b),Project(c)};float signed=Cross(triangle[1]-triangle[0],triangle[2]-triangle[0]);area+=Mathf.Abs(signed)*.5f;
    if(signed<0){var swap=triangle[1];triangle[1]=triangle[2];triangle[2]=swap;}faces.Add(triangle);
   }else{
    var triangle=new[]{a,b,c};for(int edge=0;edge<3;edge++)if(AtEnd(triangle[edge])&&AtEnd(triangle[(edge+1)%3]))boundary+=Cross(Project(triangle[edge]),Project(triangle[(edge+1)%3]))*.5f;
   }
  }
  Check(faces.Count>0,"both partial-sweep ends contain physical cap faces");
  Check(Mathf.Abs(area-Mathf.Abs(boundary))<.000001f,"cap triangle area equals the actual swept-profile boundary area");
  for(int a=0;a<faces.Count;a++)for(int b=a+1;b<faces.Count;b++)Check(Overlap(faces[a],faces[b])<1e-8f,"partial-sweep cap triangles have no interior overlap");
 }
 static void Closed(Mesh mesh){var edges=new Dictionary<string,int>();var v=mesh.vertices;var t=mesh.triangles;double volume=0;for(int i=0;i<t.Length;i+=3){Check(Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).sqrMagnitude>1e-12f,"nondegenerate solid triangles: "+mesh.name);volume+=Vector3.Dot(v[t[i]],Vector3.Cross(v[t[i+1]],v[t[i+2]]))/6d;for(int e=0;e<3;e++){string a=Key(v[t[i+e]]),b=Key(v[t[i+(e+1)%3]]);string key=string.CompareOrdinal(a,b)<0?a+"/"+b:b+"/"+a;edges.TryGetValue(key,out int n);edges[key]=n+1;}}foreach(var n in edges.Values)Check(n==2,"solid skin has no open/nonmanifold edge: "+mesh.name);Check(volume>0,"solid profile has outward winding: "+mesh.name);}
 public static void Validate(){checks=0;var host=new GameObject("AuthoredPassengerCabinFixture");var assets=new HashSet<UnityEngine.Object>();var errors=new List<string>();Application.LogCallback capture=(m,s,t)=>{if(t==LogType.Error||t==LogType.Assert||t==LogType.Exception)errors.Add(m);};Application.logMessageReceived+=capture;try{PlatformArt.Build(host.transform,0,0);var cabin=host.transform.Find("CopperLiftCabin");var skirt=cabin.Find("CabinPassengerSkirt");Check(skirt!=null,"substantial passenger shell is visible above the authoritative landing deck");var skin=skirt.GetComponent<MeshFilter>().sharedMesh;Check(skin.bounds.min.y>=.65f&&skin.bounds.max.y<=1.12f&&skin.bounds.size.y>=.40f,"passenger shell covers only the low carriage, leaving torso/head clear");Closed(skin);Cap(skin,-35,true);Cap(skin,215,false);foreach(var v in skin.vertices)Check(v.z>=-.01f||Mathf.Abs(v.x)>.75f,"wide negative-Z transfer opening has no brass face across its centre");foreach(var name in new[]{"CabinBaseDrum","CabinCrown","CabinDome"}){var mesh=cabin.Find(name).GetComponent<MeshFilter>().sharedMesh;Closed(mesh);Check(mesh.bounds.size.x<=2.35f,"existing narrow camera envelope retained: "+name);}var crown=cabin.Find("CabinCrown");Check(crown.GetComponent<MeshFilter>().sharedMesh.bounds.size.y<=.30f,"crown retains shallow height for fixed-camera head visibility");Check(cabin.Find("CabinCurvedPassengerRail")!=null&&cabin.Find("CabinMachinedRivets")!=null,"continuous rolled passenger rail and physically modeled fasteners exist");var rail=cabin.Find("CabinCurvedPassengerRail").GetComponent<MeshFilter>().sharedMesh;Closed(rail);Cap(rail,-35,true);Cap(rail,215,false);Closed(cabin.Find("CabinMachinedRivets").GetComponent<MeshFilter>().sharedMesh);var collider=host.transform.Find("LandingSurface").GetComponent<BoxCollider>();Check(collider!=null&&collider.size==new Vector3(4.6f,.70f,3.6f)&&collider.center==new Vector3(0,.25f,0)&&host.GetComponentsInChildren<Collider>().Length==1,"exact original landing collision authority retained");int vertices=0,renderers=0;var materials=new HashSet<Material>();var textures=new HashSet<Texture>();foreach(var f in cabin.GetComponentsInChildren<MeshFilter>()){vertices+=f.sharedMesh.vertexCount;assets.Add(f.sharedMesh);foreach(var normal in f.sharedMesh.normals)Check(float.IsFinite(normal.x)&&float.IsFinite(normal.y)&&float.IsFinite(normal.z)&&normal.sqrMagnitude>.5f,"finite valid polished-surface normals");}foreach(var r in host.GetComponentsInChildren<MeshRenderer>()){if(r.transform.IsChildOf(cabin)&&r.enabled)renderers++;if(r.sharedMaterial.name.StartsWith("FloorArchitecture")){materials.Add(r.sharedMaterial);assets.Add(r.sharedMaterial);if(r.sharedMaterial.mainTexture!=null){textures.Add(r.sharedMaterial.mainTexture);if(!UnityEditor.EditorUtility.IsPersistent(r.sharedMaterial.mainTexture))assets.Add(r.sharedMaterial.mainTexture);}}}Check(vertices<8500&&renderers<=80&&materials.Count<=12&&textures.Count<=2,"existing cabin render/geometry/material/finish budgets retained");Debug.Log("FLOOR_AUTHORED_CABIN_BUDGET vertices="+vertices+" rawRenderers="+renderers);UnityEngine.Object.DestroyImmediate(host);foreach(var asset in assets)Check(asset==null,"authored geometry and finish released with root");Check(errors.Count==0,"zero logged errors on generation and teardown: "+string.Join(" | ",errors));Debug.Log("FLOOR_AUTHORED_CABIN_PASS checks="+checks+" actualConceptReview=PENDING");}finally{Application.logMessageReceived-=capture;if(host!=null)UnityEngine.Object.DestroyImmediate(host);}}
}
#endif
