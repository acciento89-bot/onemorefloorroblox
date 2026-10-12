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
  // Folded single-sided leaf surfaces keep coherent normals. The shared shader
  // handles back faces; duplicate reversed triangles cancelled the old normals.
  static void Leaf(List<Vector3> v,List<int> t,List<Vector2> uv,List<Color> colors,Vector3 p,Vector3 tip,Vector3 across,float width,bool ivy,float tone)
  {
   int k=v.Count;var along=tip-p;Vector3 normal=Vector3.Cross(across,along).normalized;
   Vector2[] shape=ivy?new[]{new Vector2(0,0),new Vector2(-.58f,.18f),new Vector2(-1,.48f),new Vector2(-.49f,.52f),new Vector2(0,1),new Vector2(.49f,.52f),new Vector2(1,.48f),new Vector2(.58f,.18f)}:new[]{new Vector2(0,0),new Vector2(-.80f,.26f),new Vector2(-1,.55f),new Vector2(0,1),new Vector2(1,.55f),new Vector2(.80f,.26f)};
   v.Add(p+along*.48f+normal*width*.22f);uv.Add(new Vector2(.5f,.48f));colors.Add(new Color(tone,tone,tone,1));
   foreach(var point in shape){v.Add(p+along*point.y+across*(point.x*width));uv.Add(new Vector2(.5f+point.x*.5f,point.y));colors.Add(new Color(tone,tone,tone,1));}
   for(int edge=0;edge<shape.Length;edge++)t.AddRange(new[]{k,k+1+edge,k+1+(edge+1)%shape.Length});
  }
  static void Stem(List<Vector3> v,List<int> t,List<Vector2> uv,List<Color> colors,Vector3 a,Vector3 b,float width)
  {
   int k=v.Count;Vector3 side=Vector3.Cross(b-a,Vector3.forward).normalized*width;
   v.AddRange(new[]{a-side,a+side,b+side,b-side});uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up});for(int i=0;i<4;i++)colors.Add(new Color(.68f,.72f,.57f,1));t.AddRange(new[]{k,k+1,k+2,k,k+2,k+3});
  }
  public static void Ivy(Transform parent,Vector3 origin,float length,int seed,Material shade,Material light)
  {
   var root=new GameObject("HangingIvy");root.transform.SetParent(parent,false);root.transform.localPosition=origin;
   for(int strand=0;strand<3;strand++){
    var v=new List<Vector3>();var t=new List<int>();var uv=new List<Vector2>();var colors=new List<Color>();float drop=length*(.72f+strand*.14f);Vector3 previous=new Vector3(strand*.14f,0,0);
    for(int leaf=0;leaf<19;leaf++){
     float f=leaf/18f;var p=new Vector3(strand*.14f+.08f*Mathf.Sin(f*7+seed),-f*drop,-.08f-.09f*Mathf.Sin(f*4+strand));if(leaf>0)Stem(v,t,uv,colors,previous,p,.008f);previous=p;
     float side=leaf%2==0?-1:1;var tip=p+new Vector3(side*(.17f+.04f*Mathf.Sin(seed+leaf)), -.10f-.035f*(leaf%3),-.045f);var across=new Vector3(.42f,.89f,.10f)*side;
     Leaf(v,t,uv,colors,p,tip,across,.115f-(f*.025f),true,.82f+(leaf%4)*.075f);
    }
    MeshArt.Mesh(root.transform,"IvyLeaves",v.ToArray(),t.ToArray(),uv.ToArray(),strand==1?light:shade,colors:colors.ToArray());
   }
  }
  public static void Plant(Transform parent,Vector3 origin,Material shade,Material light,int seed)
  {
   for(int tone=0;tone<2;tone++){
    var v=new List<Vector3>();var t=new List<int>();var uv=new List<Vector2>();var colors=new List<Color>();
    for(int branch=tone;branch<7;branch+=2){float a=branch*2.39996f+seed*.21f;var outwards=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));float height=.68f+(branch%3)*.17f;var basePoint=origin+Vector3.up*.27f;Vector3 previous=basePoint;
     for(int joint=1;joint<=5;joint++){float f=joint/5f;var center=basePoint+Vector3.up*(height*f)+outwards*(.42f*f*f);Stem(v,t,uv,colors,previous,center,.012f);previous=center;
      for(int side=-1;side<=1;side+=2){var axis=new Vector3(-outwards.z,0,outwards.x)*side;var tip=center+axis*(.23f+.15f*Mathf.Sin(f*Mathf.PI))+outwards*.11f+Vector3.up*(.14f-f*.11f);var across=(Vector3.up+outwards*.32f).normalized;Leaf(v,t,uv,colors,center,tip,across,.11f+(joint%2)*.024f,false,.78f+(branch%4)*.08f);}
     }
    }
    MeshArt.Mesh(parent,"PlantLeaf",v.ToArray(),t.ToArray(),uv.ToArray(),tone==0?shade:light,colors:colors.ToArray());
   }
  }
 }
}
