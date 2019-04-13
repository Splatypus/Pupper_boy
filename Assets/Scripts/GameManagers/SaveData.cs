using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UNUSED DUE TO NEW SAVEMANAGER CLASS
 * 
 * **/

//class containing all of the items to save
[Serializable]
public class SaveData {
    //Save Distinction
    public string nameOfSave;
    //PlayerPos - (Probably Will Reset On Load)
    public float charPosX;
    public float charPosY;
    public float charPosZ;

    //Global save data
    public int season; //0=summer, 1=fall, 2=winter, 3=spring



    //Old Data Kept To Not Break References
    public int blackieConversationNumber;
}
