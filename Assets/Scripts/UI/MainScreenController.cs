using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenController : MonoBehaviour {

	public void LoadScene(string sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Quit() {

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

    #else
		Application.Quit();

    #endif

    }
}
