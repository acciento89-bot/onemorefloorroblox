#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Kamilunavo.OneMoreFloor;

namespace Kamilunavo.OneMoreFloor.Editor
{
    [InitializeOnLoad]
    public static class ProjectBootstrap
    {
        private const string ScenePath="Assets/Scenes/Main.unity";
        static ProjectBootstrap()=>EditorApplication.delayCall+=Ensure;
        private static void Ensure(){if(EditorApplication.isPlayingOrWillChangePlaymode)return;PlayerSettings.companyName="Kamilunavo";PlayerSettings.productName="One More Floor";PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,"com.kamilunavo.onemorefloor");PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.kamilunavo.onemorefloor");if(!File.Exists(ScenePath)){Directory.CreateDirectory("Assets/Scenes");var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("GameBootstrap").AddComponent<GameBootstrap>();EditorSceneManager.SaveScene(scene,ScenePath);}EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};}
    }
}
