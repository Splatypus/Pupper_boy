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

    #region Variables
    //class containing all of the items to save
    [Serializable]
    public class SaveData {
        //Save Distinction
        public string nameOfSave;
        //PlayerPos - (Probably Will Reset On Load)
        public float charPosX;
        public float charPosY;
        public float charPosZ;



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
    [Tooltip("Global file extension for saves.")]
    public string fileExtension = ".dog";

    public static SaveManager Instance;

    [Header("UI References")]

    [Tooltip("The default UI prefab of a save slot.")]
    public GameObject saveSlotBase;

    [Tooltip("UI reference to create the save slots as children to.")]
    public GameObject saveSlotHolder;

    [Tooltip("UI references to the visual slots, created at runtime.")]
    List<GameObject> saveSlots = new List<GameObject>();

    [Header("Save Game Variables")]
    [Tooltip("Maximum number of saves allowed to the player.")]
    public int maxSaves = 3;

    [Header("Player Variables")]
    [Tooltip("Only used when in backyard.")]
    public GameObject playerDoggo;

    //Loaded data gets set here, then any changes are made here before saving this data
    public static SaveData masterData = new SaveData();

    #endregion

    #region Initialization

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

    void OnEnable() {
        if (!playerDoggo) {
            if (SceneManager.GetSceneByName("Backyard").isLoaded) {

                InvokeRepeating("CheckPlayerLocation", 1, 1);
            }
            else {
                InvokeRepeating("FindPlayerDoggo", 3, 3);
            }
        }
    }

    #endregion

    #region Save Game Management
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
            continueData.saveSlotsFilled[1] = false;
            continueData.saveSlotsFilled[2] = false;

            bf.Serialize(continueFile, continueData);
            continueFile.Close();

            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + 0 + fileExtension, FileMode.OpenOrCreate);

            SaveData data = new SaveData();
            data.nameOfSave = saveName;

            bf.Serialize(file, data);
            file.Close();
        }
    }

    //Used For Loading Of A Save
    public void LoadGameFromSlot(int slotNumber) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension)) {

            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

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

        if (File.Exists(Application.persistentDataPath + "/Continue" + fileExtension)) {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.Open);

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

        if (File.Exists(Application.persistentDataPath + "/Continue" + fileExtension)) {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.Open);

            BinaryFormatter bf = new BinaryFormatter();
            ContinueSaveData continueData = new ContinueSaveData();
            continueData = (ContinueSaveData)bf.Deserialize(continueFile);
            continueFile.Close();

            if (continueData.saveSlotsFilled[slotNumber] == true) {

                FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + slotNumber + fileExtension, FileMode.Open);
                SaveData data = new SaveData();
                data = (SaveData)bf.Deserialize(file);

                saveGameSlot.GetComponentInChildren<Text>().text = data.nameOfSave;

                file.Close();
                print("Save Slot Exists: " + slotNumber);
            }
            else {
                print("No Save Data In Slot: " + slotNumber);
                saveGameSlot.GetComponentInChildren<Text>().text = "No Save In Slot";

            }
        }
        else {
            saveGameSlot.GetComponentInChildren<Text>().text = "No Save In Slot";
        }
    }

    public void DeleteSave(int slotToDelete) {

        if (File.Exists(Application.persistentDataPath + "/SaveFile" + slotToDelete + fileExtension)) {

            FileStream continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.OpenOrCreate);

            BinaryFormatter bf = new BinaryFormatter();
            ContinueSaveData continueData = new ContinueSaveData();
            continueData = (ContinueSaveData)bf.Deserialize(continueFile);
            continueFile.Close();

            continueFile = File.Open(Application.persistentDataPath + "/Continue" + fileExtension, FileMode.OpenOrCreate);

            continueData.saveSlotsFilled[slotToDelete] = false;
            bf.Serialize(continueFile, continueData);

            continueFile.Close();

            if(continueData.saveSlotsFilled[0] == false && continueData.saveSlotsFilled[1] == false && continueData.saveSlotsFilled[2] == false) {
                File.Delete(Application.persistentDataPath + "/Continue" + fileExtension);
            }

            File.Delete(Application.persistentDataPath + "/SaveFile" + slotToDelete + fileExtension);
        }

        LoadSlotData(saveSlots[slotToDelete], slotToDelete);
    }

    #endregion

    #region Data Management In Live Game

    void CheckPlayerLocation() {
        masterData.charPosX = playerDoggo.transform.position.x;
        masterData.charPosY = playerDoggo.transform.position.y;
        masterData.charPosZ = playerDoggo.transform.position.z;

        print(playerDoggo.transform.position);
    }

    void FindPlayerDoggo() {
        if (!playerDoggo) {
            if (SceneManager.GetSceneByName("Backyard").isLoaded) {

                playerDoggo = FindObjectOfType<PlayerDialog>().gameObject;

                CancelInvoke();
                //InvokeRepeating("CheckPlayerLocation", 1, 1);
            }
        }
    }

    #endregion
}
