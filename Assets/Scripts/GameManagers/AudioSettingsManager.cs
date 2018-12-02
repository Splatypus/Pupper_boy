using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour {

    //Main AudioMixer
    public AudioMixer masterMixer;

    //The Sliders
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public void RemoteStart() {
        print("Remote Start");
        //mute music at the start to account for needed delay
        masterMixer.SetFloat("musicVol", -60f);

        //Set references to self for the Sliders
        masterSlider.GetComponent<PlayerPrefsManager>().myManager = this;
        musicSlider.GetComponent<PlayerPrefsManager>().myManager = this;
        sfxSlider.GetComponent<PlayerPrefsManager>().myManager = this;

        ChangeMixerVolume("masterVol", PlayerPrefs.GetFloat("masterVol"));
        ChangeMixerVolume("musicVol", PlayerPrefs.GetFloat("musicVol"));
        ChangeMixerVolume("sfxVol", PlayerPrefs.GetFloat("sfxVol"));
    }

    public void ChangeMixerVolume(string mixKey, float value) {
        masterMixer.SetFloat(mixKey, Mathf.Log10(value) * 33);
    }
}