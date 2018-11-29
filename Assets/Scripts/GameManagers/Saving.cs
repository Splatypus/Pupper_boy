using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Saving : MonoBehaviour {

    public static Saving Instance;
    public int FilelNum = 0;
    public bool ShouldLoad = false; //set to true if data should be loaded on scene load, false if it should not be
    public bool ShouldSave = true;
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
        if (ShouldLoad)
            Load();
        else
            callbacks.Clear();
    }

    //saves the game
    public void Save() {
        if (ShouldSave) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveFile" + FilelNum + ".dat", FileMode.OpenOrCreate);

            bf.Serialize(file, data);
            file.Close();
        }
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


