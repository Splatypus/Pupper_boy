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
        public string nameOfSave;
        public int charPosX;
        public int charPosY;
        public int charPosZ;


        //Old Data Kept To Not Break References
        public int blackieConversationNumber;
    }

    //Continue Save
    [Serializable]
    public class ContinueSaveData {
        public int saveSlot;
        public bool[] saveSlotsFilled = new bool[3];
    }

    [Header("Global File Extension")]
    //Global file extension
    public string fileExtension = ".dog";

    public static SaveManager Instance;

    [Header("UI References")]
    //Put the UI for save games under this
    public GameObject saveHolder;

    //The Default UI Element For Save Slots
    public GameObject saveSlotBase;

    //The Game Object To Hold The Save Slots
    public GameObject saveSlotHolder;

    //Save Game Slots To Load Into
    List<GameObject> saveSlots = new List<GameObject>();

    [Header("Save Game Variables")]
    //Max Number of Saves
    public int maxSaves = 3;

    //Loaded data gets set here, then any changes are made here before saving this data
    public static SaveData masterData = new SaveData();

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

    public void CreateSlotUI() {
        for (int i = 0; i < maxSaves; i++) {
            GameObject newSlot = Instantiate(saveSlotBase, saveSlotHolder.gameObject.transform);
            saveSlots.Add(newSlot);

            newSlot.GetComponent<SlotSaveInfo>().slotNumber = i;

            LoadSlotData(saveSlots[i], i);
        }
    }

    //void OnEnable() {
    //    for (int i = 0; i < maxSaves; i++) {
    //        LoadSlotData(saveSlots[i], i);
    //    }
    //}

    //Called On New Game
    public void CreateNewSave(string saveName) {

        print(saveName);
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/Continue" + fileExtension)) {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.Open);
            ContinueSaveData continueData = new ContinueSaveData();
            continueData = (ContinueSaveData)bf.Deserialize(continueFile);
            continueFile.Close();

            int slotToFill = 0;

            for (int i = 0; i < maxSaves; i++) {
                if (continueData.saveSlotsFilled[i] == false) {
                    slotToFill = i;
                    break;
                }
            }

            continueData.saveSlot = slotToFill;
            continueData.saveSlotsFilled[slotToFill] = true;

            continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.Open);

            bf.Serialize(continueFile, continueData);
            continueFile.Close();

            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + slotToFill + fileExtension, FileMode.OpenOrCreate);

            SaveData data = new SaveData();
            data.nameOfSave = saveName;

            bf.Serialize(file, data);
            file.Close();

        }
        else {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.OpenOrCreate);

            ContinueSaveData continueData = new ContinueSaveData();

            continueData.saveSlot = 0;
            continueData.saveSlotsFilled[0] = true;

            bf.Serialize(continueFile, continueData);
            continueFile.Close();

            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + 0 + fileExtension, FileMode.OpenOrCreate);

            print(0);

            SaveData data = new SaveData();
            data.nameOfSave = saveName;

            bf.Serialize(file, data);
            file.Close();
        }
    }

    //Used For Loading Of A Save
    public void LoadGameFromSlot(int slotNumber) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension)) {

            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            //SaveData data = new SaveData();

            masterData = (SaveData)bf.Deserialize(file);
            file.Close();
        }

        Debug.Log(masterData.nameOfSave);
    }

    public void LoadContinueGame() {
        Debug.Log("Continue Does Nothing Yet");
    }

    //Used For Saving Of A Save
    public void SaveGameToSlot(int slotNumber) {
        Debug.Log("Saving a Save Does Nothing Yet");
    }

    //Check Eech Slot For Any Save
    public bool CheckForAnySaves() {

        #region oldCheckingCode
        //
        ////variables to check for any saves
        //int anySaves = 0;
        //
        ////Load to slots
        //for (int i = 0; i < maxSaves; i++) {
        //    anySaves += CheckForSaveGame(saveSlots[i]);
        //}
        //
        ////if there were any save games
        //if (anySaves > 0)
        //    return true;
        //else
        //    return false;
        //
        #endregion

        if (File.Exists(Application.persistentDataPath + "/Continue" + fileExtension)) {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.OpenOrCreate);

            BinaryFormatter bf = new BinaryFormatter();
            ContinueSaveData continueData = new ContinueSaveData();
            continueData = (ContinueSaveData)bf.Deserialize(continueFile);

            for (int i = 0; i < maxSaves; i++) {

                //print(continueData.saveSlotsFilled[i]);

                if (continueData.saveSlotsFilled[i] == true) {
                    continueFile.Close();
                    return true;
                }
            }

            continueFile.Close();
            return false;
        }
        else {
            return false;
        }
    }

    //Loading Data For Save Games
    void LoadSlotData(GameObject saveGameSlot, int slotNumber = 0) {

        /*if (!File.Exists(Application.persistentDataPath + "/Continue" + fileExtension)) {
        
           FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.OpenOrCreate);
        
           BinaryFormatter bf = new BinaryFormatter();
           ContinueSaveData continueData = new ContinueSaveData();
        
           continueData.saveSlot = slotNumber;
           continueData.saveSlotsFilled[slotNumber] = true;
        
           bf.Serialize(continueFile, continueData);
           continueFile.Close();
        }*/

        if (File.Exists(Application.persistentDataPath + "/Continue" + fileExtension)) {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.OpenOrCreate);

            BinaryFormatter bf = new BinaryFormatter();
            ContinueSaveData continueData = new ContinueSaveData();
            continueData = (ContinueSaveData)bf.Deserialize(continueFile);
            continueFile.Close();

            if (continueData.saveSlotsFilled[slotNumber] == true) {


                FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension, FileMode.OpenOrCreate);
                SaveData data = new SaveData();
                data = (SaveData)bf.Deserialize(file);

                saveGameSlot.GetComponentInChildren<Text>().text = data.nameOfSave;

                file.Close();
            }
        }
        else
            print("Oh No It Broke!");
    }
}
