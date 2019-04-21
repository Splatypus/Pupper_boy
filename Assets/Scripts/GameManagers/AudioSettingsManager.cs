using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Slider References")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public void RemoteStart()
    {
        //mute music at the start to account for needed delay
        //masterMixer.SetFloat("musicVol", -60f);
        AudioSystem.ChangeMixerVolume("musicVol", -60f);

        AudioSystem.ChangeMixerVolume("masterVol", PlayerPrefs.GetFloat("masterVol"));
        AudioSystem.ChangeMixerVolume("musicVol", PlayerPrefs.GetFloat("musicVol"));
        AudioSystem.ChangeMixerVolume("sfxVol", PlayerPrefs.GetFloat("sfxVol"));
    }
}