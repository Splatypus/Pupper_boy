using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EscMenuManager : MonoBehaviour {

    public GameObject settingsMenu;
    public GameObject player;

    PlayerControllerManager.Modes savedMode;

    void Start() {
        FindObjectOfType<DogController>().escMenu = this;
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.SetActive(false);
        settingsMenu.GetComponentInChildren<AudioSettingsManager>().RemoteStart();
        settingsMenu.SetActive(false);
    }



    public void Show() {
        gameObject.SetActive(true);
        savedMode = player.GetComponent<PlayerControllerManager>().currentMode;
        player.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Pause);
    }

    public void Hide() {
        gameObject.SetActive(false);
        settingsMenu.SetActive(false);
        player.GetComponent<PlayerControllerManager>().ChangeMode(savedMode);
    }

    public void ShowSettingsMenu() {

    }

    public void QuitGame() {
       
    }

    public void QuitToMM() {
        AsyncOperation async = SceneManager.LoadSceneAsync("DankAssTitleScreen");
    }

}