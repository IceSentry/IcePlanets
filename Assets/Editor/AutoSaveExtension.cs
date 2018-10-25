using UnityEditor;
using UnityEditor.SceneManagement;


[InitializeOnLoad]
public static class AutoSaveExtension
{
    static AutoSaveExtension()
    {
        EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
    }

    private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange playModeStateChange)
    {
        // If we're about to run the scene...
        if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
        {
            // Save the scene and the assets.
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }
    }
}
