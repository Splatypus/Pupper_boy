using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Saving : MonoBehaviour {

    //class containing all of the items to save
    [Serializable]
    public class SaveData {
        public int blackieConversationNumber;
    }

    public static Saving Instance;
    public int FilelNum = 0;
    public SaveData data = new SaveData();
    public Queue<UnityAction> callbacks = new Queue<UnityAction>();

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
        //things that want data loaded to them need to add themselves to callback list during awake.
        //then on scene load will load data to them as a one shot effect.
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
            while (callbacks.Count > 0) {
                UnityAction a = callbacks.Dequeue();
                a.Invoke();
            }
        } else {
            Debug.LogError("Failed to find save file: /SaveFile" + FilelNum + ".dat");
        }
    }

    public void AddCallback(UnityAction a) {
        callbacks.Enqueue(a);
    }

}


