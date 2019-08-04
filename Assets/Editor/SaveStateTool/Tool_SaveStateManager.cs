using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

public class Tool_SaveStateManager : EditorWindow
{
   // References
   static string PathToSaves { get => Application.persistentDataPath; }
   static string PathToSnapshots
   {
      get
      {
         StringBuilder sb = new StringBuilder(Path.GetDirectoryName(Application.persistentDataPath), 250);
         sb.Append('/' + "Snapshots");
         sb.Replace('\\', '/');
         var dir = sb.ToString();

         Directory.CreateDirectory(dir);

         return dir;
      }
   }

   // Variables
   const int maxSnapshotNameLength = 50;
   Dictionary<string, string> allSnapshots = new Dictionary<string, string>();

   // Temp Variables
   private string newSnapshotName = "";



   [MenuItem("Tools/Save State Manager &#t")]
   static void Init()
   {
      var window = GetWindow<Tool_SaveStateManager>(true, "Save Snapshot Manager");
      EditorBuildSettings.sceneListChanged += () => { window.RefreshSnapshotList(); };
      window.Show();
   }

   void RefreshSnapshotList()
   {
      //if (shouldRefresh == false)
      //return;

      allSnapshots = new Dictionary<string, string>();

      var snapshotDirs = Directory.GetDirectories(PathToSnapshots);

      foreach (var item in snapshotDirs)
      {
         var dirName = item.Replace("\\", "/");
         allSnapshots.Add(dirName.Split('/').Last(), dirName);
      }

      Repaint();
   }

   private void OnInspectorUpdate()
   {
      if ((allSnapshots.Count > 0) == false)
         RefreshSnapshotList();
   }

   private void OnGUI()
   {
      if (EditorApplication.isPlaying || EditorApplication.isPaused)
         return;

      GUILayout.Space(5);

      GUILayout.BeginVertical(); //-----

      GUILayout.BeginHorizontal(); //-----

      GUILayout.Label("Save Snapshots: ", EditorStyles.boldLabel);

      if (GUILayout.Button("Delete All Cached", GUILayout.MaxWidth(130)))
      {
         var popup = ConfirmationPopup.ShowConfirmationPopup("Are You Sure You Want To Delete All Cached Snapshots?");
         popup.OnConfirm.AddListener(() => { DeleteAllCachedSnapshot(); });
      }

      if (GUILayout.Button("Refresh", GUILayout.MaxWidth(90)))
         allSnapshots.Clear();

      GUILayout.EndHorizontal(); ///-----

      GUILayout.Space(10);

      GUILayout.BeginHorizontal(); //-----

      GUILayout.Space(25);

      GUILayout.BeginVertical(); //-----

      List<string> keys = new List<string>(allSnapshots.Keys.Where(item => (item.Contains("Cached") == false)));
      List<string> keysOfCached = new List<string>(allSnapshots.Keys.Where(item => item.Contains("Cached")));
      keys.OrderByDescending(item => item);
      keysOfCached.OrderByDescending(item => item);

      foreach (var key in keys)
         DrawSnapshot(key);

      GUILayout.FlexibleSpace();//-----//

      foreach (var key in keysOfCached)
         DrawSnapshot(key);

      GUILayout.EndVertical(); ///-----

      GUILayout.EndHorizontal(); ///-----

      GUILayout.FlexibleSpace(); //-----//

      GUILayout.BeginHorizontal(); //-----

      newSnapshotName = GUILayout.TextField(newSnapshotName);

      if (newSnapshotName.Length > maxSnapshotNameLength)
         newSnapshotName = newSnapshotName.Substring(0, maxSnapshotNameLength);

      newSnapshotName = Regex.Replace(newSnapshotName, @"[^0-9a-zA-Z ]+", string.Empty);

      if (GUILayout.Button("Create Snapshot", GUILayout.MaxWidth(150)))
      {
         CreateSnapshot(newSnapshotName);
         newSnapshotName = "";
      }

      GUILayout.EndHorizontal(); ///-----

      GUILayout.Space(25);

      GUILayout.EndVertical(); ///-----
   }

   private void DrawSnapshot(string key)
   {
      GUILayout.BeginHorizontal(); //-----

      var snapshotName = Path.GetFileNameWithoutExtension(key);

      GUILayout.Label(snapshotName);

      GUILayout.FlexibleSpace(); // --- //

      if (GUILayout.Button("Delete"))
      {
#if NET_4_6
         var popup = ConfirmationPopup.ShowConfirmationPopup($"Are You Sure You Would Like To Delete The \"{snapshotName}\" Snapshot?");
#else
           var popup = ConfirmationPopup.ShowConfirmationPopup("Are You Sure You Would Like To Delete The \"" + snapshotName + "\" Snapshot?");
#endif
         popup.OnConfirm.AddListener(() => { DeleteSnapshot(key); });
      }

      GUILayout.Space(25);

      if (GUILayout.Button("Load Snapshot"))
      {
         if (Directory.Exists(allSnapshots[key]))
            LoadSnapshot(key);
         else
            allSnapshots.Clear();
      }

      GUILayout.EndHorizontal(); ///-----
   }

   private void DeleteAllCachedSnapshot()
   {
      List<string> keys = new List<string>(allSnapshots.Keys);

      foreach (var item in keys)
         if (allSnapshots[item].Contains("Cached Snapshot -"))
            Directory.Delete(allSnapshots[item], true);

      allSnapshots.Clear();
   }

   private void DeleteSnapshot(string path)
   {
      Directory.Delete(allSnapshots[path], true);
      allSnapshots.Clear();
   }

   private void CreateSnapshot(string snapshotName)
   {
      var remoteSaves = new DirectoryInfo(PathToSaves).GetFiles("*.dog");

      //Debug.Log(PathToSnapshots + "/" + snapshotName);

      var newSnapshot = Directory.CreateDirectory(PathToSnapshots + "/" + snapshotName);

      foreach (var item in remoteSaves)
         File.Copy(item.FullName, newSnapshot.FullName + "/" + item.Name, true);

      allSnapshots.Clear();
   }

   private void LoadSnapshot(string snapshotToCopy)
   {
      if (snapshotToCopy.Contains("Cached Snapshot") == false)
      {
         string dtNow = DateTime.Now.ToString();
         StringBuilder sbNow = new StringBuilder("", 50);

         var dtNowSplit = dtNow.Split(' ');
         var time = dtNowSplit[1] + dtNowSplit[2];
         var date = dtNowSplit[0];

         dtNowSplit = time.Split(':');
         sbNow.Append(dtNowSplit[0] + '-' + dtNowSplit[1] + '-' + dtNowSplit[2].Substring(2));
         sbNow.Append(" ");
         sbNow.Append(date);

         sbNow.Replace('/', '-');
         sbNow.Replace(' ', '_');

#if NET_4_6
         CreateSnapshot($"Cached Snapshot - {sbNow.ToString()}");
#else
         CreateSnapshot("Cached Snapshot - " + sbNow.ToString() + "");
#endif
      }

      var remoteSaves = new DirectoryInfo(PathToSaves).GetFiles("*.dog") ?? null;

      if (remoteSaves != null)
         for (int i = 0; i < remoteSaves.Length; i++)
            remoteSaves[i].Delete();

      //Debug.Log(PathToSnapshots + "/" + snapshotToCopy);

      var snapshotFiles = new DirectoryInfo(PathToSnapshots + "/" + snapshotToCopy).GetFiles();

      foreach (var item in snapshotFiles)
         File.Copy(item.FullName, PathToSaves + "/" + item.Name, true);

      allSnapshots.Clear();
   }
}
