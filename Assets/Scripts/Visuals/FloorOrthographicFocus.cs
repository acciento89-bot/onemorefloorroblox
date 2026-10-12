using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 // Stock URP Gaussian CoC uses perspective depth. The approved fixed camera
 // requires a true orthographic depth conversion to keep the cabin sharp.
 public sealed class FloorOrthographicFocus:ScriptableRendererFeature
 {
  Material _material;FocusPass _pass;
  public override void Create()
  {
   CoreUtils.Destroy(_material);_material=CoreUtils.CreateEngineMaterial(Shader.Find("Floor/OrthographicFocus"));
   _pass=new FocusPass(_material){renderPassEvent=RenderPassEvent.BeforeRenderingPostProcessing};
  }
  public override void AddRenderPasses(ScriptableRenderer renderer,ref RenderingData data)
  {
   if(_material!=null&&data.cameraData.camera.orthographic&&data.cameraData.postProcessEnabled)renderer.EnqueuePass(_pass);
  }
  protected override void Dispose(bool disposing)=>CoreUtils.Destroy(_material);
  sealed class FocusPass:ScriptableRenderPass
  {
   readonly Material _finish;
   public FocusPass(Material finish){_finish=finish;requiresIntermediateTexture=true;ConfigureInput(ScriptableRenderPassInput.Depth);}
   public override void RecordRenderGraph(RenderGraph graph,ContextContainer frame)
   {
    var dof=VolumeManager.instance.stack.GetComponent<DepthOfField>();
    if(dof==null||!dof.active||dof.mode.value==DepthOfFieldMode.Off)return;
    var resources=frame.Get<UniversalResourceData>();if(resources.isActiveTargetBackBuffer||!resources.cameraDepthTexture.IsValid())return;
    var camera=frame.Get<UniversalCameraData>().camera;
    _finish.SetVector("_Focus",new Vector4(dof.gaussianStart.value,dof.gaussianEnd.value,dof.gaussianMaxRadius.value,camera.nearClipPlane));
    _finish.SetFloat("_FocusFar",camera.farClipPlane);
    var desc=graph.GetTextureDesc(resources.activeColorTexture);desc.name="ConceptOrthographicFocus";desc.clearBuffer=false;desc.msaaSamples=MSAASamples.None;
    var target=graph.CreateTexture(desc);
    var parameters=new RenderGraphUtils.BlitMaterialParameters(resources.activeColorTexture,target,_finish,0);
    using(var builder=graph.AddBlitPass(parameters,passName:"Concept orthographic distant-city focus",returnBuilder:true))
    {builder.UseTexture(resources.cameraDepthTexture);builder.UseAllGlobalTextures(true);}
    resources.cameraColor=target;
   }
  }
 }
}
