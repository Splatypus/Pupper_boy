using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour {

    //class containing all of the items to save
    [Serializable]
    public class SaveData {
        public int savedThing;
    }

    public static SaveManager Instance;

    //Put the UI for save games under this
    public GameObject saveHolder;

    //Save Game Slots To Load Into
    public GameObject saveSlotOne;
    public GameObject saveSlotTwo;
    public GameObject saveSlotThr;

    public string fileExtension = ".dog";

    //Check to see if there are any save games
    //private bool anySaveGame;
    //public bool anySaveGameGet { get { return anySaveGame; } }

    void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    //Called On New Game
    public void NewSaveGame() {
        if (CheckLoadSlotOne() == 0) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + "1" + fileExtension, FileMode.OpenOrCreate);

            SaveData data = new SaveData();
            bf.Serialize(file, data);
            file.Close();
        }
        if (CheckLoadSlotTwo() == 0) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + "2" + fileExtension, FileMode.OpenOrCreate);

            SaveData data = new SaveData();
            bf.Serialize(file, data);
            file.Close();
        }
        if (CheckLoadSlotThr() == 0) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + "3" + fileExtension, FileMode.OpenOrCreate);

            SaveData data = new SaveData();
            bf.Serialize(file, data);
            file.Close();
        }
    }

    //Used For Loading Of A Save
    public void LoadSaveGame() {
        Debug.Log("Loading a Save Does Nothing Yet");
    }

    //Check Eech Slot For Save
    public bool CheckForSaves() {

        //variables to check for any saves
        int anySaves = 0;

        //Load to slots
        anySaves += CheckLoadSlotOne(saveSlotOne);
        anySaves += CheckLoadSlotTwo(saveSlotTwo);
        anySaves += CheckLoadSlotThr(saveSlotThr);

        //if there were any save games
        if (anySaves > 0)
            return true;
        else
            return false;
    }

    //Checking For Save Games
    int CheckLoadSlotOne(GameObject saveGameSlot = null) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + "1" + fileExtension)) {
            saveGameSlot.GetComponentInChildren<Text>().text = "Hi";
            return 1;
        }
        else
            return 0;
    }
    //Checking For Save Games
    int CheckLoadSlotTwo(GameObject saveGameSlot = null) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + "2" + fileExtension)) {
            saveGameSlot.GetComponentInChildren<Text>().text = "Hi";

            return 1;
        }
        else
            return 0;
    }
    //Checking For Save Games
    int CheckLoadSlotThr(GameObject saveGameSlot = null) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + "3" + fileExtension)) {
            saveGameSlot.GetComponentInChildren<Text>().text = "Hi";

            return 1;
        }
        else
            return 0;
    }
}
