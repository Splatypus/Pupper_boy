using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Saving : MonoBehaviour {

    public static Saving Instance;
    SaveData data = new SaveData();
 
    void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    //saves the game
    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/SaveFile.dat", FileMode.Open);

        bf.Serialize(file, data);
        file.Close();
    }

    //loads the game
    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/SaveFile.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile.dat", FileMode.Open);

            data = (SaveData)bf.Deserialize(file);
            file.Close();
        }
    }

    //class containing all of the items to save
    [Serializable]
    public class SaveData {
        public float number;
    }

}


