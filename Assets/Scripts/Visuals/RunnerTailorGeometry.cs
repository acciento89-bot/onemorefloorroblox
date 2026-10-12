using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Original garment surfaces. Each ring follows authored cloth panels and folds;
 // all pieces remain in the existing per-joint/material renderer batches.
 internal sealed class RunnerTailorGeometry
 {
  readonly List<Vector3> _v=new(); readonly List<Vector2> _uv=new(); readonly List<int> _t=new();
  void Surface(int rings,int sides,Func<int,float,Vector3> point)
  {
   int first=_v.Count;
   for(int r=0;r<rings;r++)for(int s=0;s<=sides;s++){_v.Add(point(r,s*Mathf.PI*2/sides));_uv.Add(new Vector2((float)s/sides,(float)r/(rings-1)));}
   for(int r=0;r<rings-1;r++)for(int s=0;s<sides;s++){int a=first+r*(sides+1)+s,b=a+sides+1;_t.AddRange(new[]{a,b,a+1,a+1,b,b+1});}
   foreach(int r in new[]{0,rings-1}){
    int start=first+r*(sides+1),middle=_v.Count;Vector3 center=Vector3.zero;
    for(int s=0;s<sides;s++)center+=_v[start+s];center/=sides;_v.Add(center);_uv.Add(new Vector2(.5f,r==0?0:1));
    for(int s=0;s<sides;s++)_t.AddRange(r==0?new[]{start+s,start+s+1,middle}:new[]{start+s,middle,start+s+1});
   }
  }
  void Profile(Vector3 origin,float[] y,float[] width,float[] depth,float square=.65f,int sides=16)
  {Surface(y.Length,sides,(r,a)=>origin+new Vector3(Signed(Mathf.Cos(a),square)*width[r],y[r],Signed(Mathf.Sin(a),square)*depth[r]));}
  static float Signed(float value,float exponent)=>Mathf.Sign(value)*Mathf.Pow(Mathf.Abs(value),exponent);
  void Ribbon(Vector3[] path,float[] widths,float[] depths,int sides=8)
  {
   Vector3[] right=new Vector3[path.Length],forward=new Vector3[path.Length];
   for(int r=0;r<path.Length;r++){
    var direction=(path[Mathf.Min(r+1,path.Length-1)]-path[Mathf.Max(r-1,0)]).normalized;
    var axis=Mathf.Abs(Vector3.Dot(direction,Vector3.forward))>.9f?Vector3.right:Vector3.forward;
    var x=r==0?Vector3.Cross(direction,axis):right[r-1]-direction*Vector3.Dot(right[r-1],direction);
    if(x.sqrMagnitude<.001f)x=Vector3.Cross(direction,axis);right[r]=x.normalized;forward[r]=Vector3.Cross(right[r],direction).normalized;
   }
   Surface(path.Length,sides,(r,a)=>path[r]+right[r]*Mathf.Cos(a)*widths[r]+forward[r]*Mathf.Sin(a)*depths[r]);
  }
  GameObject Finish(Transform parent,string name,Material material)
  {
   var go=MeshArt.Mesh(parent,name,_v.ToArray(),_t.ToArray(),_uv.ToArray(),material);
   // Average the duplicate UV seam only; separate overlapping cloth panels keep
   // their own normal edges. The renderer owns this mesh through MeshArt.
   var mesh=go.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices;var normals=mesh.normals;
   var sums=new Dictionary<Vector3Int,Vector3>();
   Vector3Int Key(Vector3 v)=>new(Mathf.RoundToInt(v.x*100000),Mathf.RoundToInt(v.y*100000),Mathf.RoundToInt(v.z*100000));
   for(int i=0;i<points.Length;i++){var k=Key(points[i]);sums.TryGetValue(k,out var n);sums[k]=n+normals[i];}
   for(int i=0;i<points.Length;i++){var n=sums[Key(points[i])];if(n.sqrMagnitude>.001f)normals[i]=n.normalized;}mesh.normals=normals;
   return go;
  }
  public static GameObject Tunic(Transform parent,Material material)
  {
   var b=new RunnerTailorGeometry();
   float[] y={.045f,.078f,.12f,.21f,.32f,.43f,.505f,.55f,.57f},w={.205f,.217f,.216f,.205f,.222f,.233f,.242f,.191f,.155f},d={.133f,.143f,.145f,.146f,.151f,.150f,.133f,.107f,.095f};
   b.Surface(y.Length,24,(r,a)=>{
    float x=Signed(Mathf.Cos(a),.67f)*w[r],z=Signed(Mathf.Sin(a),.70f)*d[r];
    float side=Mathf.Pow(Mathf.Abs(Mathf.Cos(a)),8),front=Mathf.Max(0,Mathf.Sin(a));
    // Unequal side folds converge at the waist; chest remains a broad cloth panel.
    float fold=(r==2?.008f:r==3?-.009f:r==4?.006f:0)*side;
    x+=Mathf.Sign(x)*fold;z+=front*(r==3?.006f:r==4?.009f:0);
    return new Vector3(x,y[r]+(r<3?Mathf.Sin(a*3+.4f)*.004f:side*(r==5?-.014f:0)),z);
   });
   b.Profile(new Vector3(0,0,.154f),new[]{.159f,.19f,.265f,.296f},new[]{.10f,.147f,.139f,.108f},new[]{.006f,.018f,.022f,.008f},.48f);
   b.Profile(Vector3.zero,new[]{.043f,.063f,.080f},new[]{.206f,.219f,.217f},new[]{.134f,.146f,.144f},.67f,24);
   return b.Finish(parent,"TailoredLiftHoodie",material);
  }
  public static GameObject TravelPack(Transform parent,Material material)
  {
   var b=new RunnerTailorGeometry();
   b.Profile(Vector3.zero,new[]{-.055f,.015f,.10f},new[]{.19f,.218f,.205f},new[]{.113f,.133f,.126f},.72f);
   // Broad planar back, pinched lower gusset and softly bevelled shoulder corners.
   b.Profile(new Vector3(0,0,-.239f),new[]{.14f,.165f,.20f,.35f,.46f,.51f,.525f},new[]{.10f,.139f,.159f,.163f,.161f,.14f,.115f},new[]{.045f,.077f,.093f,.096f,.09f,.078f,.058f},.42f,20);
   b.Profile(new Vector3(0,0,-.250f),new[]{.43f,.442f,.47f,.515f,.548f,.556f},new[]{.13f,.159f,.171f,.17f,.151f,.12f},new[]{.087f,.111f,.113f,.106f,.088f,.065f},.40f,20);
   // Two broad raised seams frame the back panel without an extra renderer.
   for(int side=-1;side<=1;side+=2){
    b.Ribbon(new[]{new Vector3(side*.116f,.196f,-.326f),new Vector3(side*.135f,.27f,-.339f),new Vector3(side*.139f,.40f,-.333f)},new[]{.006f,.007f,.006f},new[]{.004f,.005f,.004f},6);
    b.Profile(new Vector3(side*.160f,0,-.227f),new[]{.174f,.204f,.30f,.344f},new[]{.025f,.040f,.043f,.030f},new[]{.044f,.068f,.067f,.042f},.48f,12);
    b.Ribbon(new[]{new Vector3(side*.110f,.27f,.157f),new Vector3(side*.122f,.405f,.177f),new Vector3(side*.137f,.52f,.176f),new Vector3(side*.158f,.576f,-.060f),new Vector3(side*.129f,.49f,-.178f)},new[]{.018f,.023f,.025f,.024f,.021f},new[]{.008f,.008f,.009f,.010f,.009f},10);
   }
   b.Ribbon(new[]{new Vector3(-.071f,.519f,-.236f),new Vector3(-.064f,.579f,-.225f),new Vector3(.032f,.592f,-.225f),new Vector3(.063f,.536f,-.238f)},new[]{.009f,.013f,.012f,.009f},new[]{.008f,.009f,.009f,.008f},8);
   return b.Finish(parent,"CargoWaist",material);
  }
  public static GameObject Hair(Transform parent,Material material)
  {
   var b=new RunnerTailorGeometry();
   float[] y={-.045f,.025f,.12f,.20f,.25f,.272f},w={.17f,.205f,.211f,.19f,.13f,.040f},d={.13f,.16f,.166f,.146f,.098f,.033f};
   b.Surface(y.Length,24,(r,a)=>{
    float front=Mathf.Max(0,Mathf.Sin(a)),part=Mathf.Sin(a*3+.5f)*.007f;
    return new Vector3(Mathf.Cos(a)*(w[r]+part),y[r]+(r==0?front*front*.16f:r==3?Mathf.Cos(a)*.010f:0),Mathf.Sin(a)*(d[r]+part)-.033f);
   });
   // Wide overlapping locks travel from the part over the rear silhouette.
   // They have rounded broad roots and flattened tapered ends, not cone spikes.
   for(int i=0;i<5;i++){
    float x=-.139f+i*.061f,top=.20f+(1-Mathf.Abs(x)/.17f)*.039f;
    b.Ribbon(new[]{new Vector3(x-.018f,top-.031f,.081f),new Vector3(x+.019f,top+.012f,.019f),new Vector3(x+.045f,top-.004f,-.090f),new Vector3(x+.033f,.074f+(i%2)*.016f,-.178f)},new[]{.052f,.051f,.045f,.008f},new[]{.023f,.020f,.016f,.005f},10);
   }
   for(int side=-1;side<=1;side+=2)b.Ribbon(new[]{new Vector3(side*.165f,.145f,.039f),new Vector3(side*.196f,.081f,-.047f),new Vector3(side*.172f,-.025f,-.103f)},new[]{.042f,.036f,.012f},new[]{.019f,.017f,.007f},10);
   // Two raised forelocks break the helmet silhouette while retaining the head envelope.
   for(int i=0;i<2;i++){float x=-.06f+i*.09f;b.Ribbon(new[]{new Vector3(x,.214f,.037f),new Vector3(x+.025f,.265f,.020f),new Vector3(x+.061f,.243f,-.034f)},new[]{.041f,.020f,.004f},new[]{.023f,.017f,.003f},10);}
   for(int i=0;i<3;i++){
    float x=-.11f+i*.082f;
    b.Ribbon(new[]{new Vector3(x-.030f,.167f,.087f),new Vector3(x+.008f,.136f,.140f),new Vector3(x+.046f,.077f+(i%2)*.024f,.153f)},new[]{.049f,.041f,.007f},new[]{.021f,.016f,.005f},10);
   }
   return b.Finish(parent,"SweptChestnutHair",material);
  }
 }
}
