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

    //The Default UI Element For Save Slots
    public GameObject saveSlotBase;

    //Save Game Slots To Load Into
    public List<GameObject> saveSlots = new List<GameObject>();

    //Max Number of Saves
    public int maxSaves = 3;

    //Global file extension
    public string fileExtension = ".dog";

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

    public void RemoteStart() {
        for (int i = 0; i < maxSaves; i++) {
            GameObject thing = Instantiate(saveSlotBase, this.gameObject.transform);
            saveSlots.Add(thing);
        }
    }

    //Called On New Game
    public void CreateNewSave() {

        for (int i = 0; i < maxSaves; i++) {

            if (CheckForSaveGame(saveSlots[i]) == 0) {

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + i + fileExtension, FileMode.OpenOrCreate);

                SaveData data = new SaveData();
                bf.Serialize(file, data);
                file.Close();

                break;
            }
        }
    }

    //Used For Loading Of A Save
    public void LoadSaveGame() {
        Debug.Log("Loading a Save Does Nothing Yet");
    }

    //Check Eech Slot For Any Save
    public bool CheckForAnySaves() {

        //variables to check for any saves
        int anySaves = 0;

        //Load to slots
        for (int i = 0; i < maxSaves; i++) {
            anySaves += CheckForSaveGame(saveSlots[i]);
        }

        //if there were any save games
        if (anySaves > 0)
            return true;
        else
            return false;
    }

    //Checking For Save Games
    int CheckForSaveGame(GameObject saveGameSlot, int slotNumber = 0) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension)) {
            saveGameSlot.GetComponentInChildren<Text>().text = "Hi";
            return 1;
        }
        else
            return 0;
    }
}