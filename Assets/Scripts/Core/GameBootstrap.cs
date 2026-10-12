using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.OneMoreFloor.CameraSystem;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.UI;
using Kamilunavo.OneMoreFloor.Visuals;
namespace Kamilunavo.OneMoreFloor
{
 public sealed class GameBootstrap:MonoBehaviour
 {
  private void Start(){
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Kamilunavo.OneMoreFloor.QA.FloorRuntimeQa.Configure();Kamilunavo.OneMoreFloor.QA.ElevatorRuntimeQa.Configure();
#endif


Screen.orientation=ScreenOrientation.AutoRotation;Screen.autorotateToPortrait=true;Screen.autorotateToPortraitUpsideDown=false;Screen.autorotateToLandscapeLeft=true;Screen.autorotateToLandscapeRight=true;Application.targetFrameRate=60;QualitySettings.vSyncCount=0;QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowDistance=32;
   if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));Lighting();FloorConceptLighting.AddVolume(transform);
   var player=new GameObject("Runner",typeof(CharacterController),typeof(PlayerMotor));var cc=player.GetComponent<CharacterController>();cc.height=1.96f;cc.radius=.25f;cc.center=new Vector3(0,.98f,0);cc.stepOffset=.15f;cc.skinWidth=.025f;
   var motor=player.GetComponent<PlayerMotor>();var runner=RunnerArt.Build(player.transform);runner.Motor=motor;runner.transform.localRotation=Quaternion.Euler(0,105,0);runner.transform.localScale=Vector3.one*RunnerArt.DisplayScale;
   var cameraObject=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitCamera));cameraObject.tag="MainCamera";var camera=cameraObject.GetComponent<Camera>();camera.cullingMask=~(1<<31);camera.fieldOfView=58;camera.nearClipPlane=.12f;camera.farClipPlane=260;camera.clearFlags=CameraClearFlags.Skybox;camera.allowHDR=true;FloorConceptLighting.ConfigureCamera(camera);var orbit=cameraObject.GetComponent<OrbitCamera>();orbit.Target=player.transform;orbit.Height=1.9f;orbit.Distance=7.2f;
   var course=new GameObject("FloorCourse",typeof(FloorCourse)).GetComponent<FloorCourse>();course.Player=player.transform;motor.Course=course;motor.CameraTransform=camera.transform;course.Build();cc.enabled=false;orbit.Course=course;cameraObject.transform.position=player.transform.position+new Vector3(0,3,-7);
   var roomLights=new GameObject("NearBalconyLighting",typeof(FloorBalconyLighting)).GetComponent<FloorBalconyLighting>();roomLights.transform.SetParent(course.transform,false);roomLights.Initialize(course);
   var hud=new GameObject("FloorHud",typeof(FloorHud)).GetComponent<FloorHud>();hud.Initialize(course);motor.Joystick=hud.Joystick;motor.Jump=hud.Jump;
   var feedback=new GameObject("FloorFeedback",typeof(FloorFeedback)).GetComponent<FloorFeedback>();feedback.Initialize(course,motor,hud);
   course.Hud=hud;var preview=new GameObject("TransferPreview").AddComponent<ElevatorFlightPreview>();preview.Course=course;course.Store=gameObject.AddComponent<Kamilunavo.OneMoreFloor.Monetization.StorePurchases>();course.Videos=gameObject.AddComponent<Kamilunavo.OneMoreFloor.Monetization.RewardedVideos>();course.Store.Initialize(course);course.Videos.Initialize(course);course.Interstitials=gameObject.AddComponent<Kamilunavo.OneMoreFloor.Monetization.RoundInterstitials>();course.Interstitials.Initialize(course);hud.AttachCommerce();course.Videos.Prepare();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Kamilunavo.OneMoreFloor.QA.FloorRuntimeQa.MaybeStart(course,hud,motor);Kamilunavo.OneMoreFloor.QA.ElevatorRuntimeQa.MaybeStart(course,hud);
#endif

  }
  private static void Lighting(){var sun=new GameObject("Sun",typeof(Light)).GetComponent<Light>();sun.type=LightType.Directional;sun.cullingMask=~(1<<31);sun.intensity=1.05f;sun.color=new Color(1,.92f,.79f);sun.shadows=LightShadows.Soft;sun.shadowStrength=.70f;sun.transform.rotation=Quaternion.Euler(34,-42,0);var rim=new GameObject("CyanRim",typeof(Light)).GetComponent<Light>();rim.type=LightType.Directional;rim.cullingMask=~(1<<31);rim.intensity=.35f;rim.color=new Color(.88f,.86f,.77f);rim.shadows=LightShadows.None;rim.transform.rotation=Quaternion.Euler(28,-42,0);}
 }
}
