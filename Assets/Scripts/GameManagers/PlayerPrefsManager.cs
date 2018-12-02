using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsManager : MonoBehaviour {

    //The Key To Change
    public string key = "";

    //Set by SettingsManager for referencing
    [HideInInspector]
    public AudioSettingsManager myManager;

    //only allow access for reading my value, loading from save
    private float myValue;
    public float myValueGet { get { return myValue; } }

    //Not Needed anymore, but kept for continuity
    GameObject myObject;

    //Needed delay because references were not being set fast enough (don't remember if there is a better way to do this)
    void Start() {
        Invoke("StartDelayed", .1f);
    }

    void StartDelayed() {
        //Make sure that the key exists
        if (PlayerPrefs.HasKey(key)) {
            if (!myObject)
                myObject = this.gameObject;

            //load the value to the slider
            if (myObject.GetComponent<Slider>())
                myValue = myObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat(key);

            myManager.ChangeMixerVolume(key, myValue);
        }
    }

    //Updating the PlayerPrefs value to slider.value
    public void UpdateSettingFloat(float value) {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        myValue = value;
        print(value);

        myManager.ChangeMixerVolume(key, value);
    }

    //Not Used Anymore but could be helpful for later

    //public void UpdateSettingInt(int value)
    //{
    //    PlayerPrefs.SetInt(key, value);
    //}

    //public void UpdateSettingString(string value)
    //{
    //    PlayerPrefs.SetString(key, value);
    //}
}
