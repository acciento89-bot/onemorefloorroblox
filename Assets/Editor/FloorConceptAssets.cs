#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class FloorConceptAssets
{
 const string Folder="Assets/Resources/Rendering";
 public static void BuildAndValidate(){Configure();FloorConceptLightingValidation.ValidateLight();BuildAutomation.BuildMacPreview();}
 public static void Configure()
 {
  Directory.CreateDirectory(Folder);AssetDatabase.Refresh();
  var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Folder+"/ConceptRenderer.asset");
  if(renderer==null)
  {
   // Use URP's own asset factory so post shaders and renderer resources are wired.
   var create=typeof(UniversalRenderPipelineAsset).GetMethod("CreateRendererAsset",BindingFlags.Static|BindingFlags.NonPublic);
   renderer=(UniversalRendererData)create.Invoke(null,new object[]{Folder+"/ConceptRenderer.asset",RendererType.UniversalRenderer,false,"Renderer"});
  }
  bool focusFound=false;foreach(var feature in renderer.rendererFeatures)if(feature is Kamilunavo.OneMoreFloor.Visuals.FloorOrthographicFocus)focusFound=true;
  if(!focusFound){var feature=ScriptableObject.CreateInstance<Kamilunavo.OneMoreFloor.Visuals.FloorOrthographicFocus>();feature.name="Concept Orthographic Focus";AssetDatabase.AddObjectToAsset(feature,renderer);renderer.rendererFeatures.Add(feature);renderer.SetDirty();}
  var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Folder+"/ConceptPipeline.asset");
  if(pipeline==null){pipeline=UniversalRenderPipelineAsset.Create(renderer);AssetDatabase.CreateAsset(pipeline,Folder+"/ConceptPipeline.asset");}
  pipeline.supportsHDR=true;pipeline.msaaSampleCount=2;pipeline.supportsCameraDepthTexture=true;pipeline.supportsCameraOpaqueTexture=false;
  pipeline.maxAdditionalLightsCount=4;pipeline.mainLightShadowmapResolution=1024;pipeline.shadowDistance=32;pipeline.shadowCascadeCount=2;pipeline.colorGradingMode=ColorGradingMode.HighDynamicRange;
  var settings=new SerializedObject(pipeline);settings.FindProperty("m_AdditionalLightsRenderingMode").intValue=(int)LightRenderingMode.PerPixel;
  settings.FindProperty("m_AdditionalLightShadowsSupported").boolValue=false;settings.FindProperty("m_MainLightShadowsSupported").boolValue=true;settings.FindProperty("m_SoftShadowsSupported").boolValue=true;settings.ApplyModifiedPropertiesWithoutUndo();
  GraphicsSettings.defaultRenderPipeline=pipeline;
  int quality=QualitySettings.GetQualityLevel();for(int i=0;i<QualitySettings.names.Length;i++){QualitySettings.SetQualityLevel(i,false);QualitySettings.renderPipeline=pipeline;}QualitySettings.SetQualityLevel(quality,false);
  typeof(UniversalRenderPipelineAsset).Assembly.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineGlobalSettings").GetMethod("Ensure",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{true});
  EditorUtility.SetDirty(pipeline);EditorUtility.SetDirty(renderer);
  var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Folder+"/ConceptAtmosphere.asset");
  if(profile==null){profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,Folder+"/ConceptAtmosphere.asset");}
  T Effect<T>() where T:VolumeComponent{if(profile.TryGet<T>(out var value))return value;value=profile.Add<T>(true);AssetDatabase.AddObjectToAsset(value,profile);return value;}
  var bloom=Effect<Bloom>();bloom.intensity.Override(.32f);bloom.threshold.Override(1.15f);bloom.scatter.Override(.58f);bloom.tint.Override(new Color(1,.92f,.80f));
  var balance=Effect<WhiteBalance>();balance.temperature.Override(12);balance.tint.Override(2);
  var colour=Effect<ColorAdjustments>();colour.contrast.Override(7);colour.saturation.Override(5);colour.postExposure.Override(.05f);
  var tone=Effect<Tonemapping>();tone.mode.Override(TonemappingMode.Neutral);
  var focus=Effect<DepthOfField>();focus.mode.Override(DepthOfFieldMode.Gaussian);focus.gaussianStart.Override(65);focus.gaussianEnd.Override(170);focus.gaussianMaxRadius.Override(.75f);focus.highQualitySampling.Override(false);
  EditorUtility.SetDirty(profile);foreach(var component in profile.components)EditorUtility.SetDirty(component);
  foreach(var name in new[]{"GoldAction","GlassBanner","IvoryNavigation","ChapterFrame","TealAction","GlassMeter"})
  {
   string path="Assets/Resources/UI/Concept/"+name+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);
   importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spriteBorder=new Vector4(28,28,28,28);importer.spritePixelsPerUnit=100;
   importer.isReadable=true;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
  }
  var atlas=(TextureImporter)AssetImporter.GetAtPath("Assets/Resources/Art/ConceptStoneIvyAtlas-20261012.png");atlas.wrapMode=TextureWrapMode.Clamp;atlas.mipmapEnabled=true;atlas.maxTextureSize=1024;atlas.textureCompression=TextureImporterCompression.Compressed;atlas.SaveAndReimport();
  FontAsset("Cinzel-Regular");FontAsset("Barlow-Bold");FloorArtImports.Ensure();FloorConceptModelAssets.Export();AssetDatabase.SaveAssets();
  Debug.Log("FLOOR_CONCEPT_ASSETS_CONFIGURED URP17.6/HDR/perPixelPoint/Bloom/WarmGrading/BackgroundDoF/9slice/TMP");
 }
 static void FontAsset(string name)
 {
  string path="Assets/Resources/Fonts/"+name+" SDF.asset";
  if(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path)!=null)return;
  var asset=TMP_FontAsset.CreateFontAsset(Resources.Load<Font>("Fonts/"+name),72,8,UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,1024,1024,AtlasPopulationMode.Dynamic,true);
  asset.name=name+" SDF";asset.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ÄÖÜäöüß€$.,:;!?/()+−-×·›‹↑↓%&\"'");
  AssetDatabase.CreateAsset(asset,path);AssetDatabase.AddObjectToAsset(asset.material,asset);foreach(var texture in asset.atlasTextures)AssetDatabase.AddObjectToAsset(texture,asset);
  EditorUtility.SetDirty(asset);
 }
}
#endif
