using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSoundChange : MonoBehaviour
{

    public AudioClip daySound;
    public float dayVolume;
    public AudioClip nightSound;
    public float nightVolume;
    public AudioSource source;

    // Start is called before the first frame update
    void Awake() {
        if (source == null) 
            source = GetComponent<AudioSource>();
        if (daySound == null) {
            daySound = source.clip;
            dayVolume = source.volume;
        }

        //set up triggers
        EventManager.OnDay += OnDay;
        EventManager.OnNight += OnNight;
    }

    private void OnDestroy() {
        EventManager.OnDay -= OnDay;
        EventManager.OnNight -= OnNight;
    }

    //functions to call when it becomes day or night. Changes the theme to the correct one
    public void OnDay() {
        source.volume = dayVolume;
        source.clip = daySound;
    }
    public void OnNight() {
        source.volume = nightVolume;
        source.clip = nightSound;
    }
}
