using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // One shared authored volume, owned by the bootstrap. UI remains an overlay.
 public static class FloorConceptLighting
 {
  public static void AddVolume(Transform owner)
  {
   var root=new GameObject("ConceptWarmAtmosphere",typeof(Volume));root.transform.SetParent(owner,false);
   var volume=root.GetComponent<Volume>();volume.isGlobal=true;volume.priority=10;volume.weight=1;
   volume.sharedProfile=Resources.Load<VolumeProfile>("Rendering/ConceptAtmosphere");
  }
  public static void ConfigureCamera(Camera camera)
  {
   var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;
   data.volumeLayerMask=1;data.volumeTrigger=camera.transform;data.antialiasing=AntialiasingMode.FastApproximateAntialiasing;
   camera.allowHDR=true;
  }
  public static Light CabinLamp(Transform cabin)
  {
   var lamp=new GameObject("CabinWarmPointLight",typeof(Light)).GetComponent<Light>();lamp.transform.SetParent(cabin,false);
   lamp.transform.localPosition=new Vector3(0,3.12f,-.12f);lamp.type=LightType.Point;
   lamp.color=new Color(1,.74f,.34f);lamp.intensity=1.4f;lamp.range=4.0f;
   lamp.shadows=LightShadows.None;lamp.renderMode=LightRenderMode.ForcePixel;lamp.cullingMask=~(1<<31);
   return lamp;
  }
  public static void RenderPreview(Camera camera,RenderTexture target)
  {
   RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
  }
 }
}
