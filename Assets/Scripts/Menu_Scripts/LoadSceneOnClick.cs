using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

    public GameObject loadingScreen;
    public Text loadingText;
    public GameObject mainMenuPanel;

    public void LoadByIndex(int sceneIndex) {

        loadingScreen.SetActive(true);
        mainMenuPanel.SetActive(false);
        StartCoroutine(DotLoading(0.5f));
        StartCoroutine(LoadNewSceneAsync(sceneIndex));

    }

    IEnumerator DotLoading(float duration) {
        int dots = 0;
        while (true) {
            yield return new WaitForSeconds(duration);
            if (dots == 3)
                dots = 0;
            else
                dots += 1;

            string text = "Loading";
            for (int i = 0; i < dots; i++) {
                text += ".";
            }
            loadingText.text = text;
        }
    }

    IEnumerator LoadNewSceneAsync(int index){
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        while (!async.isDone)
            yield return null;
        StopCoroutine("DotLoading");
    }
}
