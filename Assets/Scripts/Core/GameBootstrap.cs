using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.CameraSystem;
using Kamilunavo.OneMoreFloor.Gameplay;
using Kamilunavo.OneMoreFloor.Input;
using Kamilunavo.OneMoreFloor.UI;

namespace Kamilunavo.OneMoreFloor
{
    public sealed class GameBootstrap:MonoBehaviour
    {
        private static readonly Color Navy=new(.025f,.05f,.09f,.96f); private static readonly Color Gold=new(1f,.59f,.08f); private static readonly Color White=new(.97f,.985f,1f);
        private void Start(){Screen.orientation=ScreenOrientation.Portrait;Application.targetFrameRate=60;QualitySettings.vSyncCount=0;RenderSettings.fog=true;RenderSettings.fogColor=new Color(.20f,.10f,.24f);RenderSettings.fogDensity=.005f;EnsureEventSystem();Light();
            var canvas=UiFactory.Canvas();var safe=UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);safe.gameObject.AddComponent<SafeAreaFitter>();
            UiFactory.Button(safe,"Pause","Ⅱ",Navy,White,new Vector2(.03f,.91f),new Vector2(.13f,.98f),()=>Time.timeScale=Time.timeScale>.5f?0:1);
            var stagePanel=UiFactory.Panel(safe,"Stage",Navy,new Vector2(.15f,.91f),new Vector2(.53f,.98f));var stage=UiFactory.Label(stagePanel,"Text","STAGE 1/30",34,new Vector2(.06f,.34f),new Vector2(.94f,.94f),TextAnchor.MiddleLeft,White,FontStyle.Bold);var progress=UiFactory.Progress(stagePanel,new Vector2(.06f,.10f),new Vector2(.94f,.25f),new Color(.14f,.18f,.27f),Gold);
            var scorePanel=UiFactory.Panel(safe,"Score",Navy,new Vector2(.55f,.91f),new Vector2(.76f,.98f));var score=UiFactory.Label(scorePanel,"Text","SCORE\n0",30,new Vector2(.08f,.08f),new Vector2(.92f,.92f),TextAnchor.MiddleCenter,White,FontStyle.Bold);
            var coinsPanel=UiFactory.Panel(safe,"Coins",Navy,new Vector2(.78f,.91f),new Vector2(.97f,.98f));var coins=UiFactory.Label(coinsPanel,"Text","COINS\n0",30,new Vector2(.08f,.08f),new Vector2(.92f,.92f),TextAnchor.MiddleCenter,White,FontStyle.Bold);
            var joystick=VirtualJoystick.Create(safe,new Vector2(.03f,.035f),new Vector2(.28f,.18f));var jump=PressButton.Create(safe,"↑",new Vector2(.78f,.035f),new Vector2(.97f,.17f),new Color(.03f,.18f,.26f,.92f));
            var player=Player();var camera=Camera(player.transform);
            var courseObject=new GameObject("FloorCourse");var course=courseObject.AddComponent<FloorCourse>();course.Player=player.transform;course.StageText=stage;course.ScoreText=score;course.CoinsText=coins;course.Progress=progress;
            var panel=UiFactory.Panel(safe,"CheckpointPanel",Navy,new Vector2(.54f,.37f),new Vector2(.96f,.69f));course.CheckpointPanel=panel;course.CheckpointTitle=UiFactory.Label(panel,"Title","CHECKPOINT REACHED!",42,new Vector2(.08f,.66f),new Vector2(.92f,.94f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);UiFactory.Label(panel,"Hint","Continue from this floor or try again?",26,new Vector2(.08f,.49f),new Vector2(.92f,.68f),TextAnchor.MiddleCenter,White);UiFactory.Button(panel,"Continue","CONTINUE",Gold,Color.black,new Vector2(.08f,.25f),new Vector2(.92f,.46f),course.Continue);UiFactory.Button(panel,"Retry","RETRY",new Color(.08f,.11f,.17f),White,new Vector2(.08f,.05f),new Vector2(.92f,.22f),course.RetryCheckpoint);panel.gameObject.SetActive(false);
            var motor=player.GetComponent<PlayerMotor>();motor.Joystick=joystick;motor.Jump=jump;motor.CameraTransform=camera.transform;motor.Course=course;course.Build();
        }
        private static GameObject Player(){var go=GameObject.CreatePrimitive(PrimitiveType.Capsule);go.name="Runner";Destroy(go.GetComponent<Collider>());var cc=go.AddComponent<CharacterController>();cc.height=2;cc.radius=.42f;cc.center=new Vector3(0,1,0);go.AddComponent<PlayerMotor>();go.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=new Color(.04f,.05f,.08f)};return go;}
        private static Camera Camera(Transform t){var go=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitCamera));go.tag="MainCamera";var c=go.GetComponent<Camera>();c.fieldOfView=60;var o=go.GetComponent<OrbitCamera>();o.Target=t;go.transform.position=t.position+new Vector3(0,3,-7);return c;}
        private static void Light(){var go=new GameObject("Sun",typeof(Light));var l=go.GetComponent<Light>();l.type=LightType.Directional;l.intensity=1.15f;l.color=new Color(1f,.48f,.32f);go.transform.rotation=Quaternion.Euler(30,-35,0);}
        private static void EnsureEventSystem(){if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));}
    }
}
