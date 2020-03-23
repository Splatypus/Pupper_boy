using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    #region statics
    static string[] previousScenes;
    static string[] nextScenes;

    //go to loading screen, then unloads current and loads in new scene
    public static void LoadingScreenToScene(string sceneName) {
        string[] sceneList = { sceneName };
        LoadingScreenToScenes(sceneList);
    }

    //go to loading screen, then unload the current scene and load in the new ones in order
    public static void LoadingScreenToScenes(string[] sceneNames) {

        //get the currently open scenes (to unload)
        int sceneCount = SceneManager.sceneCount;
        previousScenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++) {
            previousScenes[i] = SceneManager.GetSceneAt(i).name;
        }

        //set the scenes to be loaded
        nextScenes = sceneNames;

        //load "LoadingScreen" scene additive
        SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
    }
    #endregion


    //The text that will be edited with progress info
    public Text loadingText;
    //the smallest amount of time a loading screen is allowed to last
    public float minLoadTime = 2.0f;

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(previousScenes != null);
        Debug.Assert(nextScenes != null);

        StartCoroutine(DoLoadingAsync());

    }

    //updates the UI to a given progress amount (0 is 0%, 1 is 100%)
    void UpdateUI(float progress) {
        loadingText.text = "Loading... " + (progress * 100.0f) + "%";
    }

    //Note that scenes are loaded in 1 at a time despite being an async op. DO NOT change this. Unity scene loading is bugged and must be done this way.
    IEnumerator DoLoadingAsync() { 
        //set up some data to help with progress tracking
        int taskCount = previousScenes.Length + nextScenes.Length;
        int tasksDone = 0;
        float startTime = Time.time;

        //unload previous scenes
        for (int i = 0; i < previousScenes.Length; i++)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(previousScenes[i]);
            //wait for scene to unload, keep UI updated
            while (!unloadOp.isDone) {
                float progress = (unloadOp.progress + tasksDone) / taskCount;
                UpdateUI(progress);
                yield return null;
            }
            //after unloading each scene, update the number of finished tasks
            tasksDone++;
        }
        //load in next scenes
        for (int i = 0; i < nextScenes.Length; i++) {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(nextScenes[i]);
            //wait for scenes to load, keep UI updated
            while (!loadOp.isDone) {
                float progress = (loadOp.progress + tasksDone) / taskCount;
                UpdateUI(progress);
                yield return null;
            }
            //after loading, update number of finished tasks
            tasksDone++;
        }

        //minimum load time
        if(Time.time - startTime < minLoadTime) { 
            yield return new WaitForSeconds(minLoadTime - (Time.time - startTime));
        }

        //unload self
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }

    
}
