using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Actual open leaves, not a flat illustration of a door. The shared entrance
 // recess and visible centre stay behind the existing landing authority.
 public static class TowerEntranceArt
 {
  public static void Build(Transform root,Material timber,Material brass)
  {
   for(int side=-1;side<=1;side+=2){
    var turn=Quaternion.Euler(0,-side*45,0);var center=new Vector3(side*.69f,1.68f,1.37f)+turn*new Vector3(-side*.32f,0,0);
    GameObject Part(string name,Vector3 local,Vector3 size,Material finish){var go=MeshArt.Box(root,name,center+turn*local,size,finish);go.transform.localRotation=turn;return go;}
    var pane=Part(side<0?"RecessedTealDoor":"DoorOpenLeafRight",Vector3.zero,new Vector3(.64f,2.45f,.055f),timber);FloorArchitectureDetails.SurfaceMode(pane,.5f);
    for(int edge=-1;edge<=1;edge+=2)Part("DoorLeafStile",new Vector3(edge*.302f,0,-.038f),new Vector3(.035f,2.45f,.050f),brass);
    foreach(float y in new[]{-1.20f,-.16f,1.20f})Part("DoorLeafRail",new Vector3(0,y,-.038f),new Vector3(.62f,.042f,.050f),brass);
    Part("DoorTimberLowerPanel",new Vector3(0,-.65f,-.046f),new Vector3(.50f,.91f,.034f),timber);
    Part("DoorGlazingMullion",new Vector3(0,.51f,-.041f),new Vector3(.025f,1.27f,.027f),brass);
    var handle=MeshArt.Oval(root,"DoorBrassHandle",center+turn*new Vector3(-side*.23f,-.06f,-.078f),new Vector3(.043f,.17f,.043f),brass,8);handle.transform.localRotation=turn;
   }
   for(int side=-1;side<=1;side+=2)MeshArt.Box(root,"DoorFrame",new Vector3(side*.69f,1.68f,1.30f),new Vector3(.08f,2.57f,.12f),brass);
   MeshArt.Box(root,"DoorLintel",new Vector3(0,2.96f,1.30f),new Vector3(1.46f,.08f,.12f),brass);
  }
 }
}
