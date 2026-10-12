#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
using Kamilunavo.OneMoreFloor.UI;
using Kamilunavo.OneMoreFloor.Core;
using Kamilunavo.OneMoreFloor.Gameplay;
using UnityEngine.UI;
using System.Reflection;

public static class FloorConceptLightingValidation
{
 public static void DiagnoseShaders()
 {
  foreach(var shader in Resources.LoadAll<Shader>(""))
  {
   var material=new Material(shader);
   try{for(int i=0;i<material.passCount;i++)UnityEditor.ShaderUtil.CompilePass(material,i,true);
    foreach(var message in UnityEditor.ShaderUtil.GetShaderMessages(shader))Debug.Log("CONCEPT_SHADER "+shader.name+" line="+message.line+" "+message.severity+" "+message.message);
   }finally{UnityEngine.Object.DestroyImmediate(material);}
  }
 }
 public static void ValidateUi()
 {
  var root=new GameObject("ConceptUiFixture");Canvas canvas=null;
  try
  {
   var course=root.AddComponent<FloorCourse>();var profile=new FloorProfile{Language="de",UnlockedRealm=2,TutorialCompleted=true};
   typeof(FloorCourse).GetProperty("Profile").SetValue(course,profile);course.Player=new GameObject("Player").transform;course.Player.SetParent(root.transform);
   var hud=root.AddComponent<FloorHud>();hud.Initialize(course);canvas=UnityEngine.Object.FindFirstObjectByType<Canvas>();
   var safe=(RectTransform)canvas.transform.Find("SafeArea");var modal=typeof(FloorHud).GetField("_modal",BindingFlags.Instance|BindingFlags.NonPublic);UnityEngine.Object.DestroyImmediate(((RectTransform)modal.GetValue(hud)).gameObject);modal.SetValue(hud,null);
   safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=Vector2.zero;safe.pivot=Vector2.zero;safe.anchoredPosition=Vector2.zero;safe.sizeDelta=new Vector2(390,844);
   typeof(FloorHud).GetMethod("Layout",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,null);
   var button=hud.Jump.GetComponent<Image>();
   if(button.sprite==null||button.type!=Image.Type.Sliced)throw new Exception("CONCEPT_UI: main action must render the authored gold nine-slice sprite");
   var meter=(RectTransform)safe.Find("TimingMeter");var action=(RectTransform)hud.Jump.transform;
   // Independent measurements from the unchanged 543 x 923 Concept panel.
   if(Mathf.Abs(meter.rect.width/390-401f/543)>.01f||Mathf.Abs(action.rect.width/390-432f/543)>.01f)
    throw new Exception("CONCEPT_UI: instrument must be narrower than the gold action at the measured Concept proportions");
   if(Mathf.Abs(action.anchoredPosition.y/844-73f/923)>.01f||Mathf.Abs((meter.anchoredPosition.y-action.anchoredPosition.y-action.rect.height)/844-39f/923)>.01f)
    throw new Exception("CONCEPT_UI: bottom margin and instrument-to-action gap must follow the measured Concept");
   if(hud.Jump.GetComponentInChildren<TMPro.TextMeshProUGUI>()==null)throw new Exception("CONCEPT_UI: live main action must render through the saved serif TMP asset");
   Debug.Log("FLOOR_CONCEPT_UI_PASS");
  }
  finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}
 }
 public static void ValidateLight()
 {
  var root=new GameObject("ConceptLightFixture");
  try
  {
   LiftArchitecture.Cabin(root.transform);
   var lights=root.GetComponentsInChildren<Light>();
   if(lights.Length!=1||lights[0].type!=LightType.Point)throw new Exception("CONCEPT_LIGHT: carriage must own one real warm Point Light");
   var light=lights[0];
   if(light.transform.localPosition.y<2.7f||light.transform.localPosition.y>3.42f||light.color.r<=light.color.b||light.range<3||light.shadows!=LightShadows.None)
    throw new Exception("CONCEPT_LIGHT: bounded warm overhead fill must illuminate the passenger without a point-shadow cubemap");
   if((light.cullingMask&(1<<31))!=0)throw new Exception("CONCEPT_LIGHT: gameplay lamp must exclude isolated previews");
   Debug.Log("FLOOR_CONCEPT_POINT_LIGHT_PASS");
  }
  finally{UnityEngine.Object.DestroyImmediate(root);}
 }
}
#endif
