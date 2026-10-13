#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kamilunavo.OneMoreFloor.Visuals;

// A native Mac Unity authoring surface: persistent editable meshes/prefabs,
// a saved scene, actual GPU close-ups and explicit commands. No external MCP,
// network server or changes to Main/build scene selection are introduced.
[InitializeOnLoad] public sealed class FloorArtStudio:EditorWindow
{
 const string Folder="Assets/Models/SculptedConcept";
 const string Command="/tmp/one-more-floor-art-studio.command";
 static readonly string Output=Environment.GetEnvironmentVariable("FLOOR_ART_OUTPUT")??"/Volumes/SSK SSD/Kamilunavo/Builds/FloorSculpt-20261012/EditorStudio";
 static string _exportRevision="";
 static readonly string[] Names={"AdventureBoy","OrnateCarriage","CarvedBalcony","GrandLobby"};
 GameObject _stage;Camera _camera;Light _key,_fill;RenderTexture _target;int _selected;
 static FloorArtStudio(){EditorApplication.update-=Poll;EditorApplication.update+=Poll;}
 static void Poll()
 {
  if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(Command))return;
  string command=File.ReadAllText(Command).Trim();File.Delete(Command);
  try{switch(command){case "capture":Open();break;case "marble-red":FloorMarbleFinishValidation.Validate();break;case "slim-carriage":AuthorSlimCarriage();Open();break;case "proportion-red":FloorCabinProportionValidation.Validate();break;case "cards-proportion-red":FloorLobbyVisualRegression.Validate();break;case "menu-coin-red":FloorMenuConceptValidation.ValidateCoin();break;case "menu-cards-red":FloorMenuConceptValidation.ValidateCards();break;case "menu-type-red":FloorMenuConceptValidation.ValidateType();break;case "menu-green":FloorMenuConceptValidation.Validate();break;case "menu-assets":FloorMenuAssetsAuthoring.Configure();break;case "bake-reflection":FloorReflectionAuthoring.Begin(Output);break;case "composition-green":FloorCompositionReflectionValidation.Validate();break;case "composition-red":FloorCompositionReflectionValidation.ValidateFloor();break;case "reflection-red":FloorCompositionReflectionValidation.ValidateReflection();break;case "rebuild-lobby":ExportLobby();Open();break;case "reopen-test":ValidateReopening();break;case "rebuild":_exportRevision="-v2";try{ExportModels();CreateCatalogue();Open();}finally{_exportRevision="";}break;case "asset-red":FloorAssetConsumptionValidation.Validate();break;case "validate":FloorValidation.ValidateAll();File.WriteAllText(Output+"/validation-pass.txt","Editor validation completed");break;case "build":BuildAutomation.BuildMacPreview();File.WriteAllText(Output+"/build-pass.txt","Mac development build completed");break;case "play":GetWindow<FloorArtStudio>().Close();EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");EditorApplication.isPlaying=true;break;case "stop":EditorApplication.isPlaying=false;break;case "quit":EditorApplication.Exit(0);break;default:throw new InvalidOperationException("Unknown studio command");}}
  catch(Exception error){Debug.LogException(error);Directory.CreateDirectory(Output);File.WriteAllText(Output+"/command-failure.txt",command+"\n"+error);}
 }
 [MenuItem("One More Floor/Art Studio/Open original concept models")]
 public static void Open()
 {
  Directory.CreateDirectory(Output);FloorConceptAssets.Configure();FloorSculptedArtValidation.Validate();
  var window=GetWindow<FloorArtStudio>();window.titleContent=new GUIContent("One More Floor · Art Studio");window.minSize=new Vector2(440,580);window.position=new Rect(110,90,650,850);window.Show();window.CreateStage();
  for(int i=0;i<Names.Length;i++){window.Select(i);window.Capture(Names[i]);}window.Select(0);
  File.WriteAllText(Output+"/editor-open.txt","Native Unity Editor "+Application.unityVersion+"\n"+EditorSceneManager.GetActiveScene().path+"\nOriginal editable model assets; actual GPU captures.");
  Debug.Log("FLOOR_ART_STUDIO_OPEN native Editor / saved scene / original editable models / actual captures");
 }
 void CreateStage()
 {
  Release();Directory.CreateDirectory(Folder);AssetDatabase.Refresh();
  if(!File.Exists(Folder+"/AdventureBoy.prefab"))ExportModels();
  CreateCatalogue();FloorAssetConsumptionValidation.Validate();
  // Unity's first GUI launch can have a new untitled startup scene. Preserve it
  // before opening the artist scene additively; never discard its contents.
  foreach(var loaded in EnumerableScenes())if(string.IsNullOrEmpty(loaded.path))EditorSceneManager.SaveScene(loaded,Folder+"/PreservedStartup.unity");
  var scene=SceneManager.GetSceneByPath(Folder+"/ConceptArtStudio.unity");
  if(!scene.IsValid())scene=File.Exists(Folder+"/ConceptArtStudio.unity")?EditorSceneManager.OpenScene(Folder+"/ConceptArtStudio.unity",OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);
  foreach(var root in scene.GetRootGameObjects())if(root.name=="OriginalModelArtStage")_stage=root;
  if(_stage==null){_stage=new GameObject("OriginalModelArtStage");SceneManager.MoveGameObjectToScene(_stage,scene);}
  for(int i=0;i<Names.Length;i++)if(_stage.transform.Find(Names[i])==null){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/"+Names[i]+".prefab");
   var model=(GameObject)PrefabUtility.InstantiatePrefab(prefab,_stage.transform);model.name=Names[i];
   model.transform.localPosition=Vector3.zero;model.SetActive(i==0);SetLayer(model.transform);
  }
  var cameraObject=new GameObject("StudioCamera",typeof(Camera));cameraObject.transform.SetParent(_stage.transform,false);
  _camera=cameraObject.GetComponent<Camera>();_camera.enabled=false;_camera.cullingMask=1<<31;_camera.orthographic=true;_camera.nearClipPlane=.1f;_camera.farClipPlane=80;_camera.clearFlags=CameraClearFlags.Skybox;FloorConceptLighting.ConfigureCamera(_camera);
  var sky=new Material(Resources.Load<Shader>("FloorSky")){name="StudioSunsetSky",mainTexture=Resources.Load<Texture2D>("Art/CanalSunset-20261012")};ArtLifetime.Own(_stage,sky);cameraObject.AddComponent<Skybox>().material=sky;
  _key=Light("StudioWarmKey",new Color(1,.88f,.69f),1.05f,Quaternion.Euler(32,-35,0));
  _fill=Light("StudioSkyFill",new Color(.66f,.79f,1),.35f,Quaternion.Euler(22,145,0));
  if(_stage.transform.Find("ConceptWarmAtmosphere")==null)FloorConceptLighting.AddVolume(_stage.transform);
  // Only persisted model instances and the reference profile are saved. Camera
  // and capture resources are transient, so scene reopening has no missing refs.
  cameraObject.hideFlags=HideFlags.DontSave;_key.gameObject.hideFlags=HideFlags.DontSave;_fill.gameObject.hideFlags=HideFlags.DontSave;
  // An additive author's scene must not serialize the startup scene's sun.
  var priorSun=RenderSettings.sun;RenderSettings.sun=null;EditorSceneManager.SaveScene(scene,Folder+"/ConceptArtStudio.unity");RenderSettings.sun=priorSun;
  _target=new RenderTexture(800,1000,24,RenderTextureFormat.ARGBHalf){name="ArtStudioActualGpu",antiAliasing=2};_target.Create();_camera.targetTexture=_target;
  Selection.activeGameObject=_stage.transform.Find(Names[0]).gameObject;
  if(SceneView.lastActiveSceneView!=null)SceneView.lastActiveSceneView.Frame(new Bounds(new Vector3(0,1,0),Vector3.one*3),false);
 }
 [MenuItem("One More Floor/Art Studio/Validate saved edits survive reopening")]
 public static void ValidateReopening()
 {
  const string path=Folder+"/ConceptArtStudio.unity";
  Open();
  string name="ArtStudioReopenProbe-"+Guid.NewGuid().ToString("N");
  var expected=new Vector3(1.234f,2.345f,3.456f);
  var scene=SceneManager.GetSceneByPath(path);
  var probe=new GameObject(name);SceneManager.MoveGameObjectToScene(probe,scene);
  probe.transform.position=expected;probe.SetActive(false);
  try{
   GetWindow<FloorArtStudio>().Close();
   if(!EditorSceneManager.SaveScene(scene,path))throw new InvalidOperationException("Cannot save original authoring edits");
   if(!EditorSceneManager.CloseScene(scene,true))throw new InvalidOperationException("Cannot close saved authoring scene");
   Open();scene=SceneManager.GetSceneByPath(path);
   GameObject survivor=null;foreach(var root in scene.GetRootGameObjects())if(root.name==name)survivor=root;
   if(survivor==null||(survivor.transform.position-expected).sqrMagnitude>1e-8f||survivor.activeSelf)throw new InvalidOperationException("Saved authoring edits were lost on reopen");
   File.WriteAllText(Output+"/STUDIO-REOPEN-PASS.txt","Native Editor: saved artist scene root, transform and active state survive close/reopen; original model instances retained.");
   Debug.Log("FLOOR_STUDIO_REOPEN_PASS saved edits survive native scene close/reopen");
  }finally{
   scene=SceneManager.GetSceneByPath(path);
   if(scene.IsValid()&&scene.isLoaded){foreach(var root in scene.GetRootGameObjects())if(root.name==name)DestroyImmediate(root);EditorSceneManager.SaveScene(scene,path);}
  }
 }
 static IEnumerable<Scene> EnumerableScenes(){for(int i=0;i<SceneManager.sceneCount;i++)yield return SceneManager.GetSceneAt(i);}
 Light Light(string name,Color colour,float intensity,Quaternion rotation)
 {
  var light=new GameObject(name,typeof(Light)).GetComponent<Light>();light.transform.SetParent(_stage.transform,false);light.type=LightType.Directional;light.cullingMask=1<<31;light.color=colour;light.intensity=intensity;light.transform.localRotation=rotation;light.shadows=LightShadows.Soft;light.shadowStrength=.45f;light.enabled=false;return light;
 }
 static void SetLayer(Transform root){root.gameObject.layer=31;var l=root.GetComponent<Light>();if(l!=null){l.cullingMask=1<<31;l.enabled=false;}foreach(Transform child in root)SetLayer(child);}
 void Select(int index)
 {
  _selected=index;for(int i=0;i<Names.Length;i++)_stage.transform.Find(Names[i]).gameObject.SetActive(i==index);
  Vector3 focus=index==0?new Vector3(0,1.0f,0):index==1?new Vector3(0,1.65f,0):index==2?new Vector3(1.4f,2.0f,.5f):new Vector3(.1f,2.6f,1.5f);
  _camera.orthographicSize=index==0?1.15f:index==1?2.4f:index==2?3.0f:5.4f;
  _camera.transform.position=focus+new Vector3(index==0?7.0f:index==1?4.5f:3.2f,1.3f,index==0?14:-14);_camera.transform.LookAt(focus);Render();
  Selection.activeGameObject=_stage.transform.Find(Names[index]).gameObject;Repaint();
 }
 void Render()
 {
  var priorSun=RenderSettings.sun;var priorFog=RenderSettings.fog;var lamps=_stage.GetComponentsInChildren<Light>();
  var priorAsync=ShaderUtil.allowAsyncCompilation;
  // A cold GUI shader variant can initially omit geometry. Compile capture
  // passes synchronously so the first asset image is complete, not a placeholder.
  ShaderUtil.allowAsyncCompilation=false;
  var materials=new HashSet<Material>();foreach(var renderer in _stage.GetComponentsInChildren<Renderer>())foreach(var material in renderer.sharedMaterials)if(material!=null)materials.Add(material);
  foreach(var material in materials)ShaderUtil.CompilePass(material,0,true);
  RenderSettings.sun=_key;RenderSettings.fog=false;foreach(var lamp in lamps)lamp.enabled=true;
  try{FloorConceptLighting.RenderPreview(_camera,_target);}
  finally{foreach(var lamp in lamps)lamp.enabled=false;RenderSettings.sun=priorSun;RenderSettings.fog=priorFog;ShaderUtil.allowAsyncCompilation=priorAsync;}
 }
 void Capture(string name)
 {
  var prior=RenderTexture.active;var pixels=new Texture2D(_target.width,_target.height,TextureFormat.RGB24,false);
  try{RenderTexture.active=_target;pixels.ReadPixels(new Rect(0,0,_target.width,_target.height),0,0);pixels.Apply();File.WriteAllBytes(Output+"/"+name+".png",pixels.EncodeToPNG());}
  finally{RenderTexture.active=prior;DestroyImmediate(pixels);}
 }
 void OnGUI()
 {
  EditorGUILayout.LabelField("Originale Modelle · echte Unity-Beleuchtung",EditorStyles.boldLabel);
  EditorGUILayout.HelpBox("Bearbeitbare Prefabs im Projekt. Die Aufnahmen zeigen tatsächlich gerenderte Modelle; die grafische Konzeptfreigabe bleibt separat.",MessageType.Info);
  int selected=GUILayout.Toolbar(_selected,new[]{"Figur","Fahrstuhl","Balkon","Lobby"});if(_stage!=null&&selected!=_selected)Select(selected);
  if(_target!=null){var rect=GUILayoutUtility.GetRect(400,500,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));GUI.DrawTexture(rect,_target,ScaleMode.ScaleToFit);}
  if(GUILayout.Button("Im Scene View bearbeiten")&&_stage!=null){Selection.activeGameObject=_stage.transform.Find(Names[_selected]).gameObject;var view=SceneView.lastActiveSceneView??GetWindow<SceneView>();view.Show();view.FrameSelected();}
  if(GUILayout.Button("Aktuelle echte Aufnahme speichern")&&_stage!=null){Render();Capture(Names[_selected]);}
 }
 // Targeted native prefab edits preserve original meshes/materials, GUIDs and artist children.
 static void AuthorSlimCarriage()
 {
  foreach(var name in new[]{"OrnateCarriage","GrandLobby"}){
   string path=Folder+"/"+name+".prefab";var root=PrefabUtility.LoadPrefabContents(path);
   try{
    var cabin=name=="OrnateCarriage"?root.transform:root.transform.Find("CopperLiftCabin");cabin.localScale=LiftArchitecture.CabinRadialScale;
    if(name=="GrandLobby"){
     root.transform.Find("CopperLiftGuides").localScale=LiftArchitecture.CabinRadialScale;
     root.GetComponentInChildren<RunnerAnimator>(true).transform.localScale=Vector3.one*Kamilunavo.OneMoreFloor.UI.LiftScenePreview.LobbyPassengerScale;
    }
    PrefabUtility.SaveAsPrefabAsset(root,path);
   }finally{PrefabUtility.UnloadPrefabContents(root);}
  }
  AssetDatabase.SaveAssets();Debug.Log("FLOOR_SLIM_NATIVE_PREFAB_AUTHORED actual passenger cabin and saved lobby / same height / preserved source meshes");
 }
 static void ExportModels()
 {
  var host=new GameObject("OriginalArtAssetAuthoring");
  try{
   var runner=RunnerArt.Build(host.transform);runner.name=Names[0];Persist(runner.gameObject,Names[0]);
   LiftArchitecture.Cabin(host.transform);Persist(host.transform.Find("CopperLiftCabin").gameObject,Names[1]);
   LiftArchitecture.Bay(host.transform,0);Persist(host.transform.Find("ArchitecturalFloorBay").gameObject,Names[2]);
   var lobby=new GameObject(Names[3]);lobby.transform.SetParent(host.transform,false);LiftAtriumArt.Build(lobby.transform);LiftArchitecture.Cabin(lobby.transform);LiftArchitecture.Rails(lobby.transform);
   lobby.transform.Find("CopperLiftCabin").localPosition=new Vector3(-1.4f,0,0);lobby.transform.Find("CopperLiftGuides").localPosition=Vector3.left*1.4f;
   var passenger=RunnerArt.Build(lobby.transform);passenger.transform.localPosition=new Vector3(-1.4f,.66f,-.1f);passenger.transform.localRotation=Quaternion.Euler(0,105,0);passenger.transform.localScale=Vector3.one*Kamilunavo.OneMoreFloor.UI.LiftScenePreview.LobbyPassengerScale;Persist(lobby,Names[3]);
  }finally{DestroyImmediate(host);}
  AssetDatabase.SaveAssets();
 }
 static void ExportLobby()
 {
  var host=new GameObject("OriginalLobbyFloorAuthoring");
  _exportRevision="-v4";
  try{
   var lobby=new GameObject(Names[3]);lobby.transform.SetParent(host.transform,false);
   LiftAtriumArt.Build(lobby.transform);LiftArchitecture.Cabin(lobby.transform);LiftArchitecture.Rails(lobby.transform);
   lobby.transform.Find("CopperLiftCabin").localPosition=new Vector3(-1.4f,0,0);lobby.transform.Find("CopperLiftGuides").localPosition=Vector3.left*1.4f;
   var boy=RunnerArt.Build(lobby.transform);boy.transform.localPosition=new Vector3(-1.4f,.66f,-.1f);boy.transform.localRotation=Quaternion.Euler(0,105,0);boy.transform.localScale=Vector3.one*Kamilunavo.OneMoreFloor.UI.LiftScenePreview.LobbyPassengerScale;
   Persist(lobby,Names[3]);AssetDatabase.SaveAssets();CreateCatalogue();
  }finally{_exportRevision="";DestroyImmediate(host);}
 }
 static void CreateCatalogue()
 {
  const string path="Assets/Resources/Models/ConceptArtSet.asset";Directory.CreateDirectory("Assets/Resources/Models");AssetDatabase.Refresh();
  var set=AssetDatabase.LoadAssetAtPath<FloorAuthoredArtSet>(path);if(set==null){set=CreateInstance<FloorAuthoredArtSet>();AssetDatabase.CreateAsset(set,path);}
  set.Boy=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/AdventureBoy.prefab");set.Cabin=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/OrnateCarriage.prefab");set.Balcony=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/CarvedBalcony.prefab");
  // The lobby authoring scene includes all models; runtime already composes its
  // carriage and passenger, so the catalogue supplies only the original room.
  var lobby=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/GrandLobby.prefab");set.Atrium=lobby.transform.Find("GrandAtriumArchitecture").gameObject;
  EditorUtility.SetDirty(set);AssetDatabase.SaveAssets();
 }
 static void Persist(GameObject model,string name)
 {
  string geometry=Folder+"/"+name+"Geometry"+_exportRevision+".asset";bool first=true;var meshes=new Dictionary<Mesh,Mesh>();var materials=new Dictionary<Material,Material>();var textures=new Dictionary<Texture,Texture>();
  void Save(UnityEngine.Object asset){if(first){AssetDatabase.CreateAsset(asset,geometry);first=false;}else AssetDatabase.AddObjectToAsset(asset,geometry);}
  foreach(var filter in model.GetComponentsInChildren<MeshFilter>(true)){var original=filter.sharedMesh;if(!meshes.TryGetValue(original,out var mesh)){mesh=Instantiate(original);mesh.name=original.name;Save(mesh);meshes.Add(original,mesh);}filter.sharedMesh=mesh;}
  foreach(var renderer in model.GetComponentsInChildren<MeshRenderer>(true)){
   var original=renderer.sharedMaterial;if(original==null||EditorUtility.IsPersistent(original))continue;
   if(!materials.TryGetValue(original,out var material)){material=Instantiate(original);material.name=original.name;Save(material);materials.Add(original,material);if(original.mainTexture!=null&&!EditorUtility.IsPersistent(original.mainTexture)){if(!textures.TryGetValue(original.mainTexture,out var texture)){texture=Instantiate(original.mainTexture);texture.name=original.mainTexture.name;Save(texture);textures.Add(original.mainTexture,texture);}material.mainTexture=texture;}}
   renderer.sharedMaterial=material;
  }
  foreach(var runner in model.GetComponentsInChildren<RunnerAnimator>(true))if(materials.TryGetValue(runner.Hoodie,out var hoodie))runner.Hoodie=hoodie;
  PrefabUtility.SaveAsPrefabAsset(model,Folder+"/"+name+".prefab");
 }
 void Release(){if(_camera!=null){_camera.targetTexture=null;DestroyImmediate(_camera.gameObject);}if(_key!=null)DestroyImmediate(_key.gameObject);if(_fill!=null)DestroyImmediate(_fill.gameObject);if(_target!=null){_target.Release();DestroyImmediate(_target);}_target=null;_stage=null;}
 void OnDisable()=>Release();
}
#endif
