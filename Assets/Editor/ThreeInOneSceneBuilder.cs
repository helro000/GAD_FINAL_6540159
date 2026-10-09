#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>One-click setup: generates four separate playable scenes and build settings.</summary>
public static class ThreeInOneSceneBuilder
{
    private const string SceneFolder = "Assets/Scenes";

    [MenuItem("Tools/Three-in-One Game/Generate All Four Scenes")]
    public static void GenerateScenes()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (!Directory.Exists(SceneFolder)) Directory.CreateDirectory(SceneFolder);

        BuildScene("MainMenu", ThreeInOneScene.MainMenu);
        BuildScene("Driving", ThreeInOneScene.Driving);
        BuildScene("Flying", ThreeInOneScene.Flying);
        BuildScene("Sumo", ThreeInOneScene.Sumo);

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            BuildSetting("MainMenu"),
            BuildSetting("Driving"),
            BuildSetting("Flying"),
            BuildSetting("Sumo")
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(SceneFolder + "/MainMenu.unity");
        Debug.Log("Three-in-One Game is ready. Press Play on the MainMenu scene!");
    }

    private static EditorBuildSettingsScene BuildSetting(string name)
    {
        return new EditorBuildSettingsScene(SceneFolder + "/" + name + ".unity", true);
    }

    private static void BuildScene(string sceneName, ThreeInOneScene kind)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject bootstrap = new GameObject("Three-in-One Game Entry");
        GameEntry entry = bootstrap.AddComponent<GameEntry>();
        entry.scene = kind;
        EditorSceneManager.SaveScene(scene, SceneFolder + "/" + sceneName + ".unity");
    }
}
#endif
