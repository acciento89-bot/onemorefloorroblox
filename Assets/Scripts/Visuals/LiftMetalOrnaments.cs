using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Fine moldings and scrollwork are accumulated in two meshes per kit.
 // They remain decoration; the wide transfer opening and original collider are untouched.
 public static class LiftMetalOrnaments
 {
  public static void Cabin(Transform root,Material brass,Material copper)
  {
   var trim=new RunnerMeshBuilder(root,"CabinGildedEngraving",copper);
   for(int panel=0;panel<9;panel++){
    float center=(-23+panel*25)*Mathf.Deg2Rad,half=8*Mathf.Deg2Rad;
    Vector3 P(float a,float y)=>new Vector3(Mathf.Cos(a)*1.137f,y,Mathf.Sin(a)*1.137f*.80f);
    var top=new Vector3[7];var bottom=new Vector3[7];for(int i=0;i<7;i++){float a=center-half+i*half/3;top[i]=P(a,1.022f);bottom[i]=P(a,.802f);}
    float[] width={.010f,.010f,.010f,.010f,.010f,.010f,.010f};trim.Sweep(top,width,width,5);trim.Sweep(bottom,width,width,5);trim.Stitch(top[0],bottom[0],.010f);trim.Stitch(top[6],bottom[6],.010f);
   }trim.Finish();
   var fittings=new RunnerMeshBuilder(root,"CabinCrownAndPostJewelry",brass);
   for(int side=-1;side<=1;side+=2)foreach(float z in new[]{-.19f,.58f}){
    float x=side*(z<0?1.04f:.85f);
    foreach(float y in new[]{1.29f,3.30f})fittings.Profile(new Vector3(x,y,z),new[]{-.056f,-.032f,.032f,.056f},new[]{.051f,.069f,.069f,.051f},new[]{.051f,.069f,.069f,.051f},10);
   }
   for(int ray=0;ray<12;ray++){float a=ray*Mathf.PI/6;var center=new Vector3(Mathf.Cos(a)*1.117f,3.565f,Mathf.Sin(a)*1.117f*.8f);fittings.Pebble(center,new Vector3(.034f,.073f,.034f),7);}
   // Continuous double rolled bands, rather than a plain yellow cap. Accumulate
   // them into the existing owned fittings mesh, with no extra material or draw.
   foreach(float y in new[]{3.51f,3.66f}){
    var path=new Vector3[33];var widths=new float[33];for(int i=0;i<=32;i++){float a=i*Mathf.PI/16;path[i]=new Vector3(Mathf.Cos(a)*1.116f,y,Mathf.Sin(a)*1.116f*.8f);widths[i]=.018f;}
    fittings.Sweep(path,widths,widths,6);
   }
   fittings.Finish();
  }
  public static void Balcony(Transform root,Material iron,Material brass)
  {
   var scrolls=new RunnerMeshBuilder(root,"BalconyForgedScrollwork",iron);
   for(int cell=0;cell<8;cell++)for(int side=-1;side<=1;side+=2){
    var path=new Vector3[14];var width=new float[14];for(int i=0;i<14;i++){float f=i/13f,a=f*Mathf.PI*2.1f,r=.101f*(1-f*.77f);path[i]=new Vector3(-.98f+cell*.28f+side*Mathf.Cos(a)*r,.83f+side*Mathf.Sin(a)*r,-1.19f);width[i]=.011f;}scrolls.Sweep(path,width,width,5);
   }scrolls.Finish();
   var molding=new RunnerMeshBuilder(root,"BalconyGildedRecessMolding",brass);
   foreach(float z in new[]{.59f,1.13f}){
    var arch=new Vector3[25];var width=new float[25];for(int i=0;i<25;i++){float a=i*Mathf.PI/24;arch[i]=new Vector3(Mathf.Cos(a)*.975f,2.82f+Mathf.Sin(a)*.975f,z);width[i]=.025f;}molding.Sweep(arch,width,width,7);
   }
   for(int side=-1;side<=1;side+=2){molding.Stitch(new Vector3(side*.965f,.56f,.57f),new Vector3(side*.965f,2.82f,.57f),.021f);molding.Stitch(new Vector3(side*1.27f,1.15f,-1.18f),new Vector3(side*1.27f,1.15f,.42f),.025f);}
   molding.Finish();
  }
 }
}
