using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class FloorArchitectureDetails
 {
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
   for(int i=0;i<7;i++){
    float a=i*2.39996f+seed*.21f;var p=origin+new Vector3(Mathf.Cos(a)*.20f,.36f+(i%3)*.11f,Mathf.Sin(a)*.20f);
    var leaf=MeshArt.Oval(parent,"PlantLeaf",p,new Vector3(.21f,.58f,.16f),i%3==0?light:shade,8);leaf.transform.localRotation=Quaternion.Euler(8,i*137,23+i%3*7);
   }
  }
 }
}
