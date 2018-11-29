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

    [Header("Background Sizes")]
    public GameObject[] myBackgrounds;

    [Header("Save Information")]
    string newSaveName = "";
    public string newSaveNameGet { get { return newSaveName; } set { newSaveName = value; } }

    // Use this for initialization
    void Start() {

        audioSettingsRemote.RemoteStart();
        mySaveManager.CreateSlotUI();

        CheckButtonVisibility();
    }

    void OnEnable() {
        CheckButtonVisibility();
    }

    void CheckButtonVisibility() {
        if (mySaveManager.CheckForAnySaves()) {
            myButtons[0].gameObject.SetActive(true);
            myButtons[2].gameObject.SetActive(true);

            ChangeBackgroundSize(6);
        }
        else {
            myButtons[0].gameObject.SetActive(false);
            myButtons[2].gameObject.SetActive(false);

            ChangeBackgroundSize(4);
        }
    }

    void ChangeBackgroundSize(int size) {
        for(int i = 0; i < myBackgrounds.Length; i++) {
            myBackgrounds[i].SetActive(false);
        }
        myBackgrounds[size].SetActive(true);
    }

    //Used to continue on last save
    public void Continue() {
        mySaveManager.LoadContinueGame();
    }

    //Used For Creating a New Save
    public void CreateNewSaveGame() {
        mySaveManager.CreateNewSave(newSaveName);
    }
}
