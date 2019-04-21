using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public class Tool_LoadScenesWindow : EditorWindow
{
    Dictionary<EditorBuildSettingsScene, bool> listOfScenes = new Dictionary<EditorBuildSettingsScene, bool>();
    Dictionary<EditorBuildSettingsScene, bool> listOfScenesTemp = new Dictionary<EditorBuildSettingsScene, bool>();

    // Add window to custom menu
    [MenuItem("Tools/Scene Loading/Load Scenes Window &#e")]
    static void Init()
    {

        // Get existing open window or if none, make a new one:
        Tool_LoadScenesWindow window = (Tool_LoadScenesWindow)GetWindow(typeof(Tool_LoadScenesWindow));

        window.Show();
    }

    private void Awake()
    {
        //Initialize Dictionary
        listOfScenes.Clear(); // = new Dictionary<EditorBuildSettingsScene, bool>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            listOfScenes.Add(scene, false);
        }
    }

    //Drawing Of The Custom Window
    void OnGUI()
    {
        GUILayout.Label("All Scenes", EditorStyles.boldLabel);
        listOfScenesTemp = new Dictionary<EditorBuildSettingsScene, bool>();

        //Show Toggle List
        foreach (KeyValuePair<EditorBuildSettingsScene, bool> keyValuePair in listOfScenes)
        {
            string[] splitPath = keyValuePair.Key.path.Split('/');
            string[] secondSplit = splitPath[splitPath.Length - 1].Split('.');
            string name = secondSplit[0];

            listOfScenesTemp.Add(keyValuePair.Key, EditorGUILayout.Toggle(name, keyValuePair.Value));
        }

        listOfScenes = listOfScenesTemp;

        //Load Selected Scenes Additively
        GUILayout.Label("Load Selected Scenes", EditorStyles.boldLabel);
        if (GUILayout.Button("Load"))
        {
            bool firstScene = true;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            foreach (KeyValuePair<EditorBuildSettingsScene, bool> keyValuePair in listOfScenes)
            {
                if (keyValuePair.Value == true)
                {
                    //If it is the first scene open it single, otherwise all other additive
                    EditorSceneManager.OpenScene(keyValuePair.Key.path, firstScene ? OpenSceneMode.Single : OpenSceneMode.Additive);
                    firstScene = false;

                }
            }

            //Reset Dictionary
            foreach (var key in listOfScenes.Keys.ToList())
            {
                listOfScenes[key] = false;
            }
        }
    }
}

public class SO_DictionaryOfScenes : ScriptableObject
{
    Dictionary<EditorBuildSettingsScene, bool> listOfScenes = new Dictionary<EditorBuildSettingsScene, bool>();
}