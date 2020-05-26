using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneReferences
{

    //singleton instance
    static CrossSceneReferences instance;
    public static CrossSceneReferences GetInstance() {
        if (instance == null) {
            instance = new CrossSceneReferences();
        }
        return instance;
    }
    public static void Reset() {
        instance = null;
    }

    Dictionary<string, GameObject> references = new Dictionary<string, GameObject>();

    public void PutObject(string key, GameObject g) {
        references.Add(key, g);
    }
    public void RemoveObject(string key) {
        references.Remove(key);
    }
    public GameObject GetObject(string key) {
        return references[key];
    }

}
