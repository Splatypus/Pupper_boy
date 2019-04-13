using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SaveManager {
    //statics
    private readonly string SAVE_DATA_FILE = "SaveData";
    private readonly string SAVE_META_FILE = "SaveDetails";
    private readonly string FILE_EXTENSION = ".dog";

    //singleton instance
    static SaveManager instance;
    public static SaveManager getInstance() {
        if (instance == null) {
            instance = new SaveManager();
        }
        return instance;
    }

    //General save data
    [Serializable]
    private class MetaSaveData {
        public int recentFile;
        public List<int> saveIDs;
        public List<string> saveNames;
        [SerializeField]
        int saveIDcounter;

        public MetaSaveData() {
            recentFile = -1;
            saveIDs = new List<int>();
            saveNames = new List<string>();
            saveIDcounter = 0;
        }
        //adds a save slot of the given name. Returns the generated ID of that slot
        public int AddSaveSlot(string name) {
            saveIDcounter += 1;
            saveIDs.Add(saveIDcounter);
            saveNames.Add(name);
            recentFile = saveIDcounter;
            return saveIDcounter;
        }
        public void RemoveSaveSlot(int id) {
            int idlocation = -1;
            for (int i = 0; i < saveIDs.Count; i++) {
                if (saveIDs[i] == id) {
                    idlocation = i;
                }
            }
            if (idlocation >= 0) {
                saveIDs.RemoveAt(idlocation);
                saveNames.RemoveAt(idlocation);
            }
        }
    }
    MetaSaveData metaSaveData;

    //Loaded save data
    [Serializable]
    private class LoadedSaveData {
        public int id;
        public string name;
        public List<string> keys;
        public List<int> values;

        public LoadedSaveData(int id, string name) {
            this.id = id;
            this.name = name;
            keys = new List<string>();
            values = new List<int>();
        }

        public void ParseKeyValues(Dictionary<string, int> d) {
            keys = new List<string>(d.Keys);
            values = new List<int>(d.Values);
        }
        public Dictionary<string, int> MakeDictionary() {
            Dictionary<string, int> d = new Dictionary<string, int>(keys.Count * 2);
            for (int i = 0; i < keys.Count; i++) {
                d.Add(keys[i], values[i]);
            }
            return d;
        }
    }
    LoadedSaveData loadedSaveData = null;
    int currentOpenFile;
    Dictionary<string, int> storedInts;

    //constructor. Loads in general save data
    private SaveManager() {
        string dataPath = Path.Combine(Application.persistentDataPath, SAVE_META_FILE + FILE_EXTENSION);

        if (File.Exists(dataPath)) { 
            using (StreamReader reader = File.OpenText(dataPath)) {
                string jsonString = reader.ReadToEnd();
                metaSaveData = JsonUtility.FromJson<MetaSaveData>(jsonString);
            }
        } else {
            metaSaveData = new MetaSaveData();
        }

        currentOpenFile = -1;

    }

    #region gets/sets
    public int GetLastOpenID() {
        return metaSaveData.recentFile;
    }
    public int GetNumberOfSaves() {
        return metaSaveData.saveIDs.Count;
    }
    public KeyValuePair<int, string> GetSlotInfoAtIndex(int index) {
        return new KeyValuePair<int, string>(metaSaveData.saveIDs[index], metaSaveData.saveNames[index]);
    }
    #endregion

    #region public functions
    //creates a new save file of the given name and no data loaded
    public void CreateFile(string name) {
        //add save slot
        int id = metaSaveData.AddSaveSlot(name);
        currentOpenFile = id;
        SaveMeta();
        //set up currently loaded data
        loadedSaveData = new LoadedSaveData(id, name);
        storedInts = new Dictionary<string, int>();
        //create file
        string path = Path.Combine(Application.persistentDataPath, SAVE_DATA_FILE + id + FILE_EXTENSION);
        FileStream f = File.Create(path);
        f.Close();

        //and save it
        SaveFile();
    }

    //deletes the file at the given index
    public void DeleteFile(int ID) {
        //reset continue file if its deleted
        if (metaSaveData.recentFile == ID) {
            metaSaveData.recentFile = -1;
        }
        //delete file
        string path = Path.Combine(Application.persistentDataPath, SAVE_DATA_FILE + ID + FILE_EXTENSION);
        if (File.Exists(path)) {
            File.Delete(path);
            metaSaveData.RemoveSaveSlot(ID);
            SaveMeta();
            return;
        }
        throw new FileNotFoundException();
    }

    //loads a file, setting the current JSON data to its contents
    public void LoadFile(int ID) {
        string dataPath = Path.Combine(Application.persistentDataPath, SAVE_DATA_FILE + ID + FILE_EXTENSION);

        if (File.Exists(dataPath)) {
            using (StreamReader reader = File.OpenText(dataPath)) {
                string jsonString = reader.ReadToEnd();
                loadedSaveData = JsonUtility.FromJson<LoadedSaveData>(jsonString);
            }
            storedInts = loadedSaveData.MakeDictionary();
            currentOpenFile = ID;
            metaSaveData.recentFile = ID;
            
            return;
        }
        throw new FileNotFoundException();
    }

    //saves the current JSON data
    public void SaveFile() {
        if (loadedSaveData == null) {
            return;
        }
        //prep loadedSaveData
        loadedSaveData.ParseKeyValues(storedInts);

        //save json
        string path = Path.Combine(Application.persistentDataPath, SAVE_DATA_FILE + currentOpenFile + FILE_EXTENSION);
        string jsonString = JsonUtility.ToJson(loadedSaveData);
        using (StreamWriter writer = File.CreateText(path)) {
            writer.Write(jsonString);
        }

    }

    //puts an string int pair into data. To save, call SaveFile afterward.
    //replaces anything with that key if data already exists
    public void PutInt(string key, int value) {
        storedInts[key] = value;
    }

    //gets data from the currently loaded savefile. Returns null if nothing is stored at that key
    public int GetInt(String key, int defaultValue) {
        int result = defaultValue;
        if (storedInts.TryGetValue(key, out result)) {
            return result;
        } else {
            return defaultValue;
        }
    }
    #endregion


    #region private functions
    private void SaveMeta() {
        string path = Path.Combine(Application.persistentDataPath, SAVE_META_FILE + FILE_EXTENSION);
        string jsonString = JsonUtility.ToJson(metaSaveData);
        using (StreamWriter writer = File.CreateText(path)) {
            writer.Write(jsonString);
        }
    }
    #endregion

}
