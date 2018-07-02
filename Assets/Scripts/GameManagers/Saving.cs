using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Saving : MonoBehaviour {

    public static Saving Instance;

 
    void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    //saves the game
    public void Save() {

    }

}
