using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    //Save Manager reference
    public SaveManager mySaveManager;

    public Button[] myButtons;


    // Use this for initialization
    void Start() {
        if (mySaveManager.CheckForSaves())
            myButtons[0].gameObject.SetActive(true);
        else
            myButtons[0].gameObject.SetActive(false);
    }

    //Used to continue on last save
    public void Continue() {
        mySaveManager.LoadSaveGame();
    }

    //Used For Creating a New Save
    public void CreateNewSaveGame() {
        mySaveManager.NewSaveGame();
    }
}
