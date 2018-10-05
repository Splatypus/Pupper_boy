using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsManager : MonoBehaviour {

    //The Key To Change
    public string key = "";

    public GameObject myObject;

    void Start()
    {
        //Make sure that the key exists
        if (PlayerPrefs.HasKey(key))
        {
            if (!myObject)
                myObject = this.gameObject;

            //check to see if its a slider, and if it is, load the value
            if (myObject.GetComponent<Slider>())
                myObject.GetComponent<Slider>().normalizedValue = PlayerPrefs.GetFloat(key);
        }
    }

    public void UpdateSettingFloat(float value) {
        PlayerPrefs.SetFloat(key, value);
    }

    public void UpdateSettingInt(int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public void UpdateSettingString(string value)
    {
        PlayerPrefs.SetString(key, value);
    }
}
