using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EscMenuManager : MonoBehaviour {

    public GameObject settingsMenu;
    public GameObject player;

    void Start() {
        FindObjectOfType<DogControllerV2>().escMenu = this;
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.SetActive(false);
        settingsMenu.GetComponentInChildren<AudioSettingsManager>().RemoteStart();
        settingsMenu.SetActive(false);
    }



    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
        settingsMenu.SetActive(false);
        player.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);
    }

    public void ShowSettingsMenu() {

    }

    public void QuitGame() {
       
    }

    public void QuitToMM() {
        AsyncOperation async = SceneManager.LoadSceneAsync(0);
    }

}