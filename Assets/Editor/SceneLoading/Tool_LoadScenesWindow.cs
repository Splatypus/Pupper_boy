using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

public class Tool_LoadScenesWindow : EditorWindow
{
   Dictionary<string, bool> editorScenes = new Dictionary<string, bool>();
   List<string> keys = new List<string>();

   [MenuItem("Tools/Scene Loading &#e")]
   static void Init()
   {
      var window = GetWindow<Tool_LoadScenesWindow>(true, "Scene Loading");
      EditorBuildSettings.sceneListChanged += () => { window.RefreshDictionary(); };
      window.Show();
   }

   void RefreshDictionary()
   {
      editorScenes = new Dictionary<string, bool>();
      keys = new List<string>();

      foreach (var item in EditorBuildSettings.scenes)
         editorScenes.Add(item.path, false);

      keys = editorScenes.Keys.ToList();

      Repaint();
   }

   private void OnInspectorUpdate()
   {
      if ((editorScenes.Count > 0) == false)
         RefreshDictionary();
   }

   private void OnGUI()
   {
      if ((editorScenes.Count > 0) == false)
         return;

      if (EditorApplication.isPlaying || EditorApplication.isPaused)
         return;

      GUILayout.Space(5);

      GUILayout.BeginVertical(); //-----

      GUILayout.Label("All Scenes: ", EditorStyles.boldLabel);

      GUILayout.Space(10);

      GUILayout.BeginHorizontal(); //-----

      GUILayout.Space(25);

      GUILayout.BeginVertical(); //-----

      foreach (var key in keys)
         editorScenes[key] = GUILayout.Toggle(editorScenes[key], "   " + Path.GetFileNameWithoutExtension(key));

      GUILayout.EndVertical(); ///-----

      GUILayout.EndHorizontal(); ///-----

      GUILayout.FlexibleSpace(); // ----- //

      GUILayout.BeginHorizontal(); //-----

      if (GUILayout.Button("Load All"))
      {
         bool firstScene = true;
         foreach (var key in keys)
         {
            if (editorScenes[key] == false)
               continue;

            if (firstScene)
            {
               firstScene = false;
               EditorSceneManager.OpenScene(key);
            }
            else
               EditorSceneManager.OpenScene(key, OpenSceneMode.Additive);
         }

         editorScenes.Clear();
      }

      if (GUILayout.Button("Load All Addative"))
      {
         foreach (var key in keys)
         {
            if (editorScenes[key] == false)
               continue;

            EditorSceneManager.OpenScene(key, OpenSceneMode.Additive);
         }

         editorScenes.Clear();
      }

      GUILayout.EndHorizontal(); ///-----

      GUILayout.EndVertical(); ///-----
   }
}

class Tool_AddSceneToBuildSettings : EditorWindow
{
   [MenuItem("Assets/Add Scene To Build Settings")]
   static void AddSceneToBuildSettings()
   {
      var selection = Selection.activeObject as SceneAsset;

      var buildSettingsScenes = EditorBuildSettings.scenes;
      var sceneToAdd = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(selection), true);

      if (buildSettingsScenes.Contains(sceneToAdd) == false)
      {
         bool contains = false;
         foreach (var item in buildSettingsScenes)
         {
            if (Path.GetFileName(item.path) == Path.GetFileName(sceneToAdd.path))
            {
               contains = true;
               break;
            }
         }
         if (contains == false)
         {
            var temp = buildSettingsScenes.ToList();
            temp.Add(sceneToAdd);
            buildSettingsScenes = temp.ToArray();
         }
      }

      EditorBuildSettings.scenes = buildSettingsScenes;
   }

   [MenuItem("Assets/Add Scene To Build Settings", validate = true)]
   static bool AddSceneToBuildSettingsValidate()
   {
      var selection = Selection.activeObject;

      if (selection != null)
         if (selection as SceneAsset)
         {
            var buildSettingsScenes = EditorBuildSettings.scenes;
            var sceneToAdd = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(selection), true);

            foreach (var item in buildSettingsScenes)
               if (Path.GetFileName(item.path) == Path.GetFileName(sceneToAdd.path))
                  return false;

            return true;
         }
         else
            return false;
      else
         return false;
   }

   [MenuItem("Assets/Remove Scene From Build Settings")]
   static void RemoveSceneFromBuildSettings()
   {
      var selection = Selection.activeObject as SceneAsset;

      var buildSettingsScenes = EditorBuildSettings.scenes;
      var sceneToAdd = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(selection), true);

      bool contains = false;
      int index = 0;
      for (int i = 0; i < buildSettingsScenes.Length; i++)
      {
         if (Path.GetFileName(buildSettingsScenes[i].path) == Path.GetFileName(sceneToAdd.path))
         {
            index = i;
            contains = true;
            break;
         }
      }
      if (contains == true)
      {
         var temp = buildSettingsScenes.ToList();
         temp.RemoveAt(index);
         buildSettingsScenes = temp.ToArray();
      }

      EditorBuildSettings.scenes = buildSettingsScenes;
   }

   [MenuItem("Assets/Remove Scene From Build Settings", validate = true)]
   static bool RemoveSceneFromBuildSettingsValidate()
   {
      var selection = Selection.activeObject;

      if (selection != null)
         if (selection as SceneAsset)
         {
            var buildSettingsScenes = EditorBuildSettings.scenes;
            var sceneToAdd = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(selection), true);

            foreach (var item in buildSettingsScenes)
               if (Path.GetFileName(item.path) == Path.GetFileName(sceneToAdd.path))
                  return true;

            return false;
         }
         else
            return false;
      else
         return false;
   }
}