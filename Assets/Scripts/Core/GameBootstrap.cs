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


Screen.orientation=ScreenOrientation.AutoRotation;Screen.autorotateToPortrait=true;Screen.autorotateToPortraitUpsideDown=false;Screen.autorotateToLandscapeLeft=true;Screen.autorotateToLandscapeRight=true;Application.targetFrameRate=60;QualitySettings.vSyncCount=0;QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowDistance=24;
   if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));Lighting();
   var player=new GameObject("Runner",typeof(CharacterController),typeof(PlayerMotor));var cc=player.GetComponent<CharacterController>();cc.height=1.96f;cc.radius=.25f;cc.center=new Vector3(0,.98f,0);cc.stepOffset=.15f;cc.skinWidth=.025f;
   var motor=player.GetComponent<PlayerMotor>();var runner=RunnerArt.Build(player.transform);runner.Motor=motor;runner.transform.localScale=Vector3.one*1.25f;
   var cameraObject=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitCamera),typeof(FloorBloom));cameraObject.tag="MainCamera";var camera=cameraObject.GetComponent<Camera>();camera.fieldOfView=58;camera.nearClipPlane=.12f;camera.farClipPlane=260;camera.clearFlags=CameraClearFlags.Skybox;camera.allowHDR=true;var orbit=cameraObject.GetComponent<OrbitCamera>();orbit.Target=player.transform;orbit.Height=1.9f;orbit.Distance=7.2f;
   var course=new GameObject("FloorCourse",typeof(FloorCourse)).GetComponent<FloorCourse>();course.Player=player.transform;motor.Course=course;motor.CameraTransform=camera.transform;course.Build();cc.enabled=false;orbit.Course=course;cameraObject.transform.position=player.transform.position+new Vector3(0,3,-7);
   var hud=new GameObject("FloorHud",typeof(FloorHud)).GetComponent<FloorHud>();hud.Initialize(course);motor.Joystick=hud.Joystick;motor.Jump=hud.Jump;
   var feedback=new GameObject("FloorFeedback",typeof(FloorFeedback)).GetComponent<FloorFeedback>();feedback.Initialize(course,motor,hud);
   course.Hud=hud;var preview=new GameObject("TransferPreview").AddComponent<ElevatorFlightPreview>();preview.Course=course;course.Store=gameObject.AddComponent<Kamilunavo.OneMoreFloor.Monetization.StorePurchases>();course.Videos=gameObject.AddComponent<Kamilunavo.OneMoreFloor.Monetization.RewardedVideos>();course.Store.Initialize(course);course.Videos.Initialize(course);hud.AttachCommerce();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Kamilunavo.OneMoreFloor.QA.FloorRuntimeQa.MaybeStart(course,hud,motor);Kamilunavo.OneMoreFloor.QA.ElevatorRuntimeQa.MaybeStart(course,hud);
#endif

  }
  private static void Lighting(){var sun=new GameObject("Sun",typeof(Light)).GetComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.15f;sun.color=new Color(1,.50f,.30f);sun.shadows=LightShadows.Soft;sun.shadowStrength=.55f;sun.transform.rotation=Quaternion.Euler(42,-28,0);}
 }
}
