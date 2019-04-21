using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsManager : MonoBehaviour
{

    //The Key To Change
    public string key = "";

    public float myValue { get; private set; }

    //Not Needed anymore, but kept for continuity
    GameObject myObject;

    //Needed delay because references were not being set fast enough (don't remember if there is a better way to do this)
    void Start()
    {
        Invoke("StartDelayed", .1f);
    }

    void StartDelayed()
    {
        //Make sure that the key exists
        if (PlayerPrefs.HasKey(key))
        {
            if (!myObject)
                myObject = this.gameObject;

            //load the value to the slider
            if (myObject.GetComponent<Slider>())
                myValue = myObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat(key);

            AudioSystem.ChangeMixerVolume(key, myValue);
        }
    }

    //Updating the PlayerPrefs value to slider.value
    public void UpdateSettingFloat(float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        myValue = value;

        AudioSystem.ChangeMixerVolume(key, myValue);
    }
}