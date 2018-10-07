using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    //Save Manager Reference and Audio Settings Reference
    [Header("References")]
    public SaveManager mySaveManager;
    public AudioSettingsManager audioSettingsRemote;

    [Header("Buttons")]
    public Button[] myButtons;

    [Header("Save Information")]
    string newSaveName = "";
    public string newSaveNameGet { get { return newSaveName; } set { newSaveName = value; } }

    // Use this for initialization
    void Start() {

        audioSettingsRemote.RemoteStart();
        mySaveManager.RemoteStart();

        if (mySaveManager.CheckForAnySaves()) {
            myButtons[0].gameObject.SetActive(true);
            myButtons[2].gameObject.SetActive(true);
        }
        else {
            myButtons[0].gameObject.SetActive(false);
            myButtons[2].gameObject.SetActive(false);
        }
    }

    //Used to continue on last save
    public void Continue() {
        mySaveManager.LoadGame();
    }

    //Used For Creating a New Save
    public void CreateNewSaveGame() {
        mySaveManager.CreateNewSave(newSaveName);
    }
}
