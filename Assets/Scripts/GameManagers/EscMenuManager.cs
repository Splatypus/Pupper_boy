using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuManager : MonoBehaviour {

    public GameObject settingsMenu;

     void Start() {
        FindObjectOfType<DogControllerV2>().escMenu = this;
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
    }

    public void ShowSettingsMenu() {

    }

    public void QuitGame() {
    }

    public void QuitToMM() {
    }

}