using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    #region statics
    static string[] nextScenes;

    //go to loading screen, then unloads current and loads in new scene
    public static void LoadingScreenToScene(string sceneName) {
        string[] sceneList = { sceneName };
        LoadingScreenToScenes(sceneList);
    }

    //go to loading screen, then unload the current scene and load in the new ones in order
    public static void LoadingScreenToScenes(string[] sceneNames) {

        //set the scenes to be loaded
        nextScenes = sceneNames;

        //load "LoadingScreen" scene additive
        SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Single);
    }
    #endregion

    //Screen Canvas
    public GameObject loadingCanvas;
    //The text that will be edited with progress info
    public Text loadingText;
    //the smallest amount of time a loading screen is allowed to last
    public float minLoadTime = 2.0f;

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(nextScenes != null);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(loadingCanvas);

        StartCoroutine(DoLoadingAsync());

    }

    //updates the UI to a given progress amount (0 is 0%, 1 is 100%)
    void UpdateUI(float progress) {
        loadingText.text = "Loading... " + (progress * 100.0f) + "%";
    }

    //Note that scenes are loaded in 1 at a time despite being an async op. DO NOT change this. Unity scene loading is bugged and must be done this way.
    IEnumerator DoLoadingAsync() {
        //load base scene (if not already loaded
        if (!SceneManager.GetSceneByName(nextScenes[0]).isLoaded)
        {
            AsyncOperation baseOp = SceneManager.LoadSceneAsync(nextScenes[0], LoadSceneMode.Single);
            baseOp.allowSceneActivation = false;
            while (baseOp.progress < 0.9f)
            {
                UpdateUI(baseOp.progress / nextScenes.Length);
                yield return new WaitForEndOfFrame();
            }
            baseOp.allowSceneActivation = true;
        }

        //then load extra scenes (if not already loaded)
        for (int i = 1; i < nextScenes.Length; i++) {
            if (SceneManager.GetSceneByName(nextScenes[i]).isLoaded) {
                continue;
            }
            AsyncOperation seasonOP = SceneManager.LoadSceneAsync(nextScenes[i], LoadSceneMode.Additive);
            while (seasonOP.progress < 0.9f) {
                UpdateUI( (seasonOP.progress + i) / nextScenes.Length);
                yield return new WaitForEndOfFrame();
            }
            seasonOP.allowSceneActivation = true;
        }

        //wait a small amount, then remove this object along with the loading screen
        UpdateUI(1.0f);
        yield return new WaitForSeconds(0.1f);
        Destroy(loadingCanvas);
        Destroy(this);
    }

    
}
