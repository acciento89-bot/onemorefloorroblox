using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Original lift-specific solids. Cross sections author the actual rolled edge,
 // recessed field and internal return; a closed sweep supplies physical end faces.
 public static class CabinAuthoredGeometry
 {
  public static GameObject Profile(Transform parent,string name,Vector2[] section,float from,float to,int segments,Material material,Vector3 at)
  {
   var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();int count=section.Length;
   for(int ring=0;ring<=segments;ring++){float a=Mathf.Lerp(from,to,(float)ring/segments)*Mathf.Deg2Rad;for(int p=0;p<count;p++){vertices.Add(new Vector3(Mathf.Cos(a)*section[p].x,section[p].y,Mathf.Sin(a)*section[p].x*.8f));uv.Add(new Vector2((float)ring/segments,(float)p/count));}}
   for(int ring=0;ring<segments;ring++)for(int p=0;p<count;p++){int next=(p+1)%count,a=ring*count+p,b=(ring+1)*count+p,c=ring*count+next,d=(ring+1)*count+next;triangles.AddRange(new[]{a,c,b,c,d,b});}
   if(to-from<359.9f){var cap=Triangulate(section);foreach(int end in new[]{0,segments})for(int i=0;i<cap.Count;i+=3){int offset=end*count;triangles.AddRange(end==0?new[]{offset+cap[i],offset+cap[i+2],offset+cap[i+1]}:new[]{offset+cap[i],offset+cap[i+1],offset+cap[i+2]});}}
   var go=MeshArt.Mesh(parent,name,vertices.ToArray(),triangles.ToArray(),uv.ToArray(),material);go.transform.localPosition=at;SmoothSeams(go.GetComponent<MeshFilter>().sharedMesh);return go;
  }
  static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
  // A rolled cross section is concave. Clip real polygon ears instead of
  // overlapping a centroid fan across its recessed field and return lip.
  static List<int> Triangulate(Vector2[] section)
  {
   var remaining=new List<int>();var triangles=new List<int>();float area=0;
   for(int i=0;i<section.Length;i++){remaining.Add(i);area+=Cross(section[i],section[(i+1)%section.Length]);}
   float winding=area>=0?1:-1;
   while(remaining.Count>3){bool clipped=false;
    for(int i=0;i<remaining.Count;i++){
     int a=remaining[(i+remaining.Count-1)%remaining.Count],b=remaining[i],c=remaining[(i+1)%remaining.Count];
     if(Cross(section[b]-section[a],section[c]-section[b])*winding<=1e-8f)continue;
     bool occupied=false;foreach(int p in remaining){if(p==a||p==b||p==c)continue;
      if(Cross(section[b]-section[a],section[p]-section[a])*winding>=-1e-8f&&Cross(section[c]-section[b],section[p]-section[b])*winding>=-1e-8f&&Cross(section[a]-section[c],section[p]-section[c])*winding>=-1e-8f){occupied=true;break;}}
     if(occupied)continue;triangles.AddRange(new[]{a,b,c});remaining.RemoveAt(i);clipped=true;break;
    }
    if(!clipped)throw new System.ArgumentException("Cabin cross section must be a simple nondegenerate polygon",nameof(section));
   }
   triangles.AddRange(remaining);return triangles;
  }
  static void SmoothSeams(Mesh mesh){var points=mesh.vertices;var normals=mesh.normals;var sums=new Dictionary<(int,int,int),Vector3>();(int,int,int) Key(Vector3 p)=>(Mathf.RoundToInt(p.x*100000),Mathf.RoundToInt(p.y*100000),Mathf.RoundToInt(p.z*100000));for(int i=0;i<points.Length;i++){var key=Key(points[i]);sums.TryGetValue(key,out var n);sums[key]=n+normals[i];}for(int i=0;i<points.Length;i++){var n=sums[Key(points[i])];if(n.sqrMagnitude>.01f)normals[i]=n.normalized;}mesh.normals=normals;}
  public static void Rivets(Transform parent,Material material){var vertices=new List<Vector3>();var triangles=new List<int>();
   void Button(float a,float radius,float height){float angle=a*Mathf.Deg2Rad;var normal=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle)/.8f).normalized;var across=Vector3.Cross(normal,Vector3.up);var center=new Vector3(Mathf.Cos(angle)*radius,height,Mathf.Sin(angle)*radius*.8f);int k=vertices.Count;const int sides=10;for(int ring=0;ring<2;ring++)for(int i=0;i<sides;i++){float t=i*Mathf.PI*2/sides;vertices.Add(center+normal*(ring==0?0:.012f)+(Vector3.up*Mathf.Cos(t)+across*Mathf.Sin(t))*(ring==0?.022f:.012f));}vertices.Add(center);vertices.Add(center+normal*.014f);for(int i=0;i<sides;i++){int n=(i+1)%sides;triangles.AddRange(new[]{k+i,k+n,k+sides+i,k+n,k+sides+n,k+sides+i,k+2*sides,k+n,k+i,k+2*sides+1,k+sides+i,k+sides+n});}}
   for(int i=0;i<18;i++)Button(i*20,1.108f,3.55f);for(int i=0;i<12;i++)Button(-31+i*242f/11,1.113f,.94f);
   MeshArt.Mesh(parent,"CabinMachinedRivets",vertices.ToArray(),triangles.ToArray(),null,material);
  }
 }
}
