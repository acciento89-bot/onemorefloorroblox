using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.OneMoreFloor.Visuals;
namespace Kamilunavo.OneMoreFloor.UI
{
 // A real 3D view of the shipping kit. No screenshot background and no gameplay state clone.
 [RequireComponent(typeof(CanvasRenderer))] public sealed class LiftScenePreview:RawImage
 {
  public const int PreviewLayer=31;
  const int SlotCount=64;const float SlotSpacing=128;
  static readonly bool[] Slots=new bool[SlotCount];
  int _slot=-1;
  static int LeaseSlot(){for(int i=0;i<Slots.Length;i++)if(!Slots[i]){Slots[i]=true;return i;}throw new System.InvalidOperationException("Too many simultaneous lift previews");}
  GameObject _scene;Camera _camera;RenderTexture _target;bool _rendered,_city;Vector2 _size;
  public GameObject SceneRoot=>_scene;
  static LiftScenePreview CreatePreview(Transform parent,string name){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(LiftScenePreview));go.transform.SetParent(parent,false);var preview=go.GetComponent<LiftScenePreview>();preview.raycastTarget=false;preview.InitializeScene();return preview;}
  public static RectTransform Create(Transform parent,int tower){var preview=CreatePreview(parent,"LiftElevation");preview.BuildCabin(tower);return preview.rectTransform;}
  public static RectTransform CreateCity(Transform parent,int chapter){var preview=CreatePreview(parent,"ChapterIllustration");var rect=preview.rectTransform;preview._city=true;var city=new GameObject("ChapterCityKit").transform;city.SetParent(preview._scene.transform,false);var stone=MeshArt.Mat("ChapterStone"+chapter,chapter==1?new Color(.27f,.35f,.45f):chapter==2?new Color(.79f,.77f,.69f):new Color(.69f,.55f,.42f));var roof=MeshArt.Mat("ChapterRoof"+chapter,chapter==1?new Color(.16f,.55f,.69f):new Color(.10f,.30f,.32f));var lamp=MeshArt.Mat("ChapterWindow"+chapter,chapter==1?new Color(.89f,.29f,.78f):new Color(1,.72f,.26f),-1,true);
   for(int i=0;i<9;i++){float x=(i%3-1)*2.2f,z=i/3*2.0f,height=1.7f+(i*7%5)*.6f;var tower=new GameObject("IllustratedCityTower").transform;tower.SetParent(city,false);tower.localPosition=new Vector3(x,0,z);MeshArt.Box(tower,"Masonry",new Vector3(0,height/2,0),new Vector3(1.55f,height,1.45f),stone);var dome=MeshArt.Lathe(tower,i%2==0?"Dome":"Spire",new[]{0f,.35f,.85f,1.12f},new[]{.84f,.71f,.35f,0f},16,roof,Vector3.one);dome.transform.localPosition=new Vector3(0,height,0);for(int row=0;row<3;row++)for(int col=0;col<2;col++)MeshArt.Box(tower,"LitWindow",new Vector3(-.36f+col*.72f,.5f+row*.7f,-.74f),new Vector3(.20f,.32f,.035f),lamp);MeshArt.Box(tower,"Cornice",new Vector3(0,height-.06f,0),new Vector3(1.7f,.12f,1.6f),stone);}
   MeshArt.Box(city,"Canal",new Vector3(0,-.07f,-1.5f),new Vector3(8,.08f,1.8f),MeshArt.Mat("ChapterRiver",new Color(.25f,.55f,.61f)));SetLayer(city);preview._camera.transform.localPosition=new Vector3(8,7,-12);preview._camera.transform.LookAt(preview._scene.transform.position+new Vector3(0,2,1));preview._camera.backgroundColor=chapter==1?new Color(.09f,.16f,.32f):chapter==2?new Color(.60f,.81f,.89f):new Color(.96f,.63f,.37f);return rect;
  }
  void BuildCabin(int tower){LiftArchitecture.Cabin(_scene.transform);LiftArchitecture.Rails(_scene.transform);LiftArchitecture.Bay(_scene.transform,0);var stone=MeshArt.Mat("LobbyMarble",new Color(.78f,.76f,.67f));MeshArt.Box(_scene.transform,"LobbyLanding",new Vector3(0,.35f,0),new Vector3(4.4f,.45f,3.5f),stone);var runner=RunnerArt.Build(_scene.transform);runner.transform.localPosition=new Vector3(0,.66f,-.1f);runner.transform.localRotation=Quaternion.Euler(0,155,0);runner.Style(tower==1?1:2);SetLayer(_scene.transform);}
  void InitializeScene(){_scene=new GameObject("Lobby3DKit");_slot=LeaseSlot();_scene.transform.position=new Vector3(1000+(_slot%8)*SlotSpacing,0,1000+(_slot/8)*SlotSpacing);
   var cameraObject=new GameObject("LobbyPreviewCamera",typeof(Camera));cameraObject.transform.SetParent(_scene.transform,false);_camera=cameraObject.GetComponent<Camera>();_camera.enabled=false;_camera.cullingMask=1<<PreviewLayer;_camera.clearFlags=CameraClearFlags.SolidColor;_camera.backgroundColor=new Color(.89f,.85f,.77f);_camera.orthographic=true;_camera.nearClipPlane=.1f;_camera.farClipPlane=40;_camera.allowHDR=false;cameraObject.transform.localPosition=new Vector3(6.5f,4.6f,-12);cameraObject.transform.LookAt(_scene.transform.position+new Vector3(0,1.7f,.2f));
   _target=new RenderTexture(384,768,16,RenderTextureFormat.ARGB32){name="RealLiftLobbyPreview",antiAliasing=2};if(Application.isPlaying)_target.Create();_camera.targetTexture=_target;texture=_target;
  }
  static void SetLayer(Transform root){root.gameObject.layer=PreviewLayer;foreach(Transform child in root)SetLayer(child);}
  void LateUpdate(){if(!Application.isPlaying||_camera==null)return;var size=rectTransform.rect.size;if(size.x<1||size.y<1||(_rendered&&size==_size))return;_size=size;_camera.aspect=size.x/size.y;_camera.orthographicSize=_city?Mathf.Max(3.7f,4.5f/_camera.aspect):Mathf.Max(3.2f,2.65f/_camera.aspect);_camera.Render();_rendered=true;}
  protected override void OnDisable(){base.OnDisable();if(_scene!=null)_scene.SetActive(false);}
  protected override void OnEnable(){base.OnEnable();if(_scene!=null)_scene.SetActive(true);_rendered=false;}
  protected override void OnDestroy(){base.OnDestroy();texture=null;if(_camera!=null)_camera.targetTexture=null;if(_target!=null){_target.Release();Dispose(_target);}if(_scene!=null){_scene.SetActive(false);Dispose(_scene);}if(_slot>=0){Slots[_slot]=false;_slot=-1;}}
  static void Dispose(Object asset){if(Application.isPlaying)Destroy(asset);else DestroyImmediate(asset);}
 }
}
