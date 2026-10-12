using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Shared stationary mechanism for the live lift and owned atrium preview.
 // Geometry only: the course retains motion, route-base anchoring and collision.
 public static class LiftGuideArt
 {
  public static void Build(Transform root,float bottom,float top,Material teal,Material brass,string railName)
  {
   float height=top-bottom,middle=(bottom+top)*.5f;
   for(int side=-1;side<=1;side+=2){
    MeshArt.Box(root,railName,new Vector3(side*.80f,middle,.80f),new Vector3(.18f,height,.19f),teal);
    MeshArt.Box(root,"VerticalGuideInlay",new Vector3(side*.80f,middle,.697f),new Vector3(.039f,height,.021f),brass);
    MeshArt.Box(root,"GuideChannelReturn",new Vector3(side*.875f,middle,.675f),new Vector3(.027f,height,.11f),teal);
   }
   MeshArt.Box(root,"LiftServiceBackbone",new Vector3(0,middle,.96f),new Vector3(.28f,height,.20f),teal);
   MeshArt.Box(root,"ServiceBackboneInlay",new Vector3(0,middle,.851f),new Vector3(.025f,height,.020f),brass);
   var collars=new RunnerMeshBuilder(root,"RailMount",brass);
   for(float y=Mathf.Ceil(bottom/4.6f)*4.6f;y<=top;y+=4.6f){
    collars.Stitch(new Vector3(-.88f,y,.87f),new Vector3(.88f,y,.87f),.025f);
    for(int side=-1;side<=1;side+=2)collars.Profile(new Vector3(side*.80f,y,.80f),new[]{-.07f,-.04f,.04f,.07f},new[]{.10f,.12f,.12f,.10f},new[]{.11f,.13f,.13f,.11f},8);
   }
   collars.Finish();
  }
 }
}
