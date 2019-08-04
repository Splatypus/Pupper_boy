using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;

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
