using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class Saving : MonoBehaviour {

    //class containing all of the items to save
    [Serializable]
    public class SaveData {
        public int blackieConversationNumber;
    }

    public static Saving Instance;
    public int FilelNum = 0;
    public SaveData data = new SaveData();
    public List<ISavesData> callbacks = new List<ISavesData>(); 

    void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }
    
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Load();
    }

    //saves the game
    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + FilelNum + ".dat", FileMode.OpenOrCreate);

        bf.Serialize(file, data);
        file.Close();
    }

    //loads the game
    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/SaveFile" + FilelNum + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + FilelNum + ".dat", FileMode.Open);

            data = (SaveData)bf.Deserialize(file);
            file.Close();

            //then inform everything that needs to be informed about the load
            foreach (ISavesData sd in callbacks) {
                if (sd != null)
                    sd.OnLoad();
            }
        } else {
            Debug.LogError("Failed to find save file: /SaveFile" + FilelNum + ".dat");
        }
    }

    public void AddCallback(ISavesData s) {
        if (!callbacks.Contains(s)) {
            callbacks.Add(s);
        }
    }

}

//interface that anything that saves data needs. 
public interface ISavesData {
    void OnLoad();
}


