using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public class Tool_LoadSceneDropdown : MonoBehaviour {

    [MenuItem("Tools/Scene Loading/Load Title Screen &#t")]
    private static void LoadTitleScreen() {
        EditorSceneManager.OpenScene("Assets/Scenes/DankAssTitleScreen.unity");
    }

    [MenuItem("Tools/Scene Loading/Load Backyard &#b")]
    private static void LoadBackyard() {
        EditorSceneManager.OpenScene("Assets/Scenes/Backyard.unity");
    }

    [MenuItem("Tools/Seasonal Scene Loading/Load Halloween &#h")]
    private static void LoadHalloween() {
        EditorSceneManager.OpenScene("Assets/Scenes/Backyard.unity");
        EditorSceneManager.OpenScene("Assets/Scenes/Seasonal Scenes/Halloween.unity", OpenSceneMode.Additive);
    }
}