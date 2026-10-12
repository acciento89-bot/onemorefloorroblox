using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class FloorArchitectureDetails
 {
  // One closed authored solid with real clipped corners and upper/lower bevels.
  public static GameObject BeveledBlock(Transform parent,string name,Vector3 position,Vector3 size,float bevel,Material material)
  {
   bevel=Mathf.Min(bevel,Mathf.Min(size.x,Mathf.Min(size.y,size.z))*.24f);
   float hx=size.x*.5f,hz=size.z*.5f,hy=size.y*.5f;
   var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
   Vector3 Point(int corner,int ring){float inset=ring==0||ring==3?bevel:0;float x=hx-inset,z=hz-inset,c=bevel*.62f;var point=corner switch{0=>new Vector2(-x+c,-z),1=>new Vector2(x-c,-z),2=>new Vector2(x,-z+c),3=>new Vector2(x,z-c),4=>new Vector2(x-c,z),5=>new Vector2(-x+c,z),6=>new Vector2(-x,z-c),_=>new Vector2(-x,-z+c)};return new Vector3(point.x,ring==0?-hy:ring==1?-hy+bevel:ring==2?hy-bevel:hy,point.y);}
   void Tri(Vector3 a,Vector3 b,Vector3 c){int k=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);uv.Add(new Vector2(a.x,a.z));uv.Add(new Vector2(b.x,b.z));uv.Add(new Vector2(c.x,c.z));triangles.Add(k);triangles.Add(k+1);triangles.Add(k+2);}
   void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d){int k=vertices.Count;vertices.AddRange(new[]{a,b,c,d});foreach(var point in new[]{a,b,c,d})uv.Add(new Vector2(point.x,point.z));triangles.AddRange(new[]{k,k+2,k+1,k,k+3,k+2});}
   for(int edge=0;edge<8;edge++){int next=(edge+1)%8;Tri(new Vector3(0,-hy,0),Point(edge,0),Point(next,0));Tri(new Vector3(0,hy,0),Point(next,3),Point(edge,3));for(int ring=0;ring<3;ring++){var a=Point(edge,ring);var b=Point(next,ring);var c=Point(next,ring+1);var d=Point(edge,ring+1);Quad(a,b,c,d);}}
   var go=MeshArt.Mesh(parent,name,vertices.ToArray(),triangles.ToArray(),uv.ToArray(),material);go.transform.localPosition=position;return go;
  }
  // Only the rear and side panels are glazed; the entire forward transfer opening stays clear.
  public static void CabinGlass(Transform parent,Material material)
  {
   var v=new List<Vector3>();var t=new List<int>();
   void Panel(Vector3 a,Vector3 b,Vector3 c,Vector3 d){int k=v.Count;v.AddRange(new[]{a,b,c,d});t.AddRange(new[]{k,k+1,k+2,k,k+2,k+3});}
   for(int side=-1;side<=1;side+=2){float x=side*1.025f;Panel(new Vector3(x,1.28f,-.13f),new Vector3(x,1.28f,.54f),new Vector3(x,3.405f,.54f),new Vector3(x,3.405f,-.13f));}
   Panel(new Vector3(-.82f,1.28f,.58f),new Vector3(.82f,1.28f,.58f),new Vector3(.82f,3.405f,.58f),new Vector3(-.82f,3.405f,.58f));
   MeshArt.Mesh(parent,"CabinSideAndRearGlass",v.ToArray(),t.ToArray(),null,material);
  }
  public static GameObject ArchStone(Transform parent,string name,float start,float end,float inner,float outer,float y,float z,float halfDepth,Material material)
  {
   Vector3 P(float a,float r,float d)=>new Vector3(Mathf.Cos(a)*r,y+Mathf.Sin(a)*r,z+d);
   var p=new[]{P(start,inner,-halfDepth),P(end,inner,-halfDepth),P(end,outer,-halfDepth),P(start,outer,-halfDepth),P(start,inner,halfDepth),P(end,inner,halfDepth),P(end,outer,halfDepth),P(start,outer,halfDepth)};
   int[] faces={0,1,2,0,2,3,4,6,5,4,7,6,0,5,1,0,4,5,3,6,7,3,2,6,0,7,4,0,3,7,1,6,2,1,5,6};
   var vertices=new Vector3[faces.Length];var uv=new Vector2[faces.Length];var t=new int[faces.Length];
   for(int i=0;i<faces.Length;i++){vertices[i]=p[faces[i]];uv[i]=new Vector2(vertices[i].x*.55f,vertices[i].y*.55f);t[i]=i;}
   return MeshArt.Mesh(parent,name,vertices,t,uv,material);
  }
  public static void Ivy(Transform parent,Vector3 origin,float length,int seed,Material shade,Material light)
  {
   var root=new GameObject("HangingIvy");root.transform.SetParent(parent,false);root.transform.localPosition=origin;
   for(int strand=0;strand<3;strand++){
    var v=new List<Vector3>();var t=new List<int>();float drop=length*(.72f+strand*.14f);
    for(int leaf=0;leaf<12;leaf++){
     float f=leaf/11f;var p=new Vector3(strand*.085f+.057f*Mathf.Sin(f*7+seed),-f*drop,-.045f-.08f*Mathf.Sin(f*4+strand));
     float a=leaf*2.4f+seed*.21f;var along=new Vector3(Mathf.Cos(a)*.045f,-.13f,Mathf.Sin(a)*.045f);var side=new Vector3(.085f*Mathf.Cos(a),.008f,.085f*Mathf.Sin(a));int k=v.Count;
     v.AddRange(new[]{p,p+along*.3f-side,p+along,p+along*.3f+side,p+along*.43f+Vector3.back*.018f});
     t.AddRange(new[]{k,k+1,k+4,k+1,k+2,k+4,k+2,k+3,k+4,k+3,k,k+4,k+4,k+1,k,k+4,k+2,k+1,k+4,k+3,k+2,k+4,k,k+3});
    }
    MeshArt.Mesh(root.transform,"IvyLeaves",v.ToArray(),t.ToArray(),null,strand==1?light:shade);
   }
  }
  public static void Plant(Transform parent,Vector3 origin,Material shade,Material light,int seed)
  {
   var v=new List<Vector3>();var t=new List<int>();
   for(int leaf=0;leaf<11;leaf++){float a=leaf*2.39996f+seed*.21f;var outwards=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var across=new Vector3(-outwards.z,0,outwards.x);float length=.48f+(leaf%4)*.14f;int start=v.Count;
    for(int joint=0;joint<7;joint++){float f=joint/6f;var center=origin+Vector3.up*(.06f+length*f*(1-.34f*f))+outwards*(.07f+.52f*f*f);float width=Mathf.Sin(f*Mathf.PI)*(.052f+(leaf%3)*.015f);v.Add(center-across*width);v.Add(center+Vector3.up*.018f);v.Add(center+across*width);if(joint<6){int k=start+joint*3;t.AddRange(new[]{k,k+3,k+1,k+1,k+3,k+4,k+1,k+4,k+2,k+2,k+4,k+5,k+1,k+3,k,k+4,k+3,k+1,k+2,k+4,k+1,k+5,k+4,k+2});}}
   }MeshArt.Mesh(parent,"PlantLeaf",v.ToArray(),t.ToArray(),null,shade);

  }
 }
}
