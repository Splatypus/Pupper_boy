using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuManager : MonoBehaviour {

    public GameObject settingsMenu;

     void Start() {

        gameObject.SetActive(false);
        settingsMenu.GetComponentInChildren<AudioSettingsManager>().RemoteStart();
        settingsMenu.SetActive(false);
    }

    public void ShowSettingsMenu() {

    }

    public void QuitGame() {
    }

    public void QuitToMM() {
    }

}