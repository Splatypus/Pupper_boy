using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightMusicChange : MonoBehaviour {

    public AudioClip dayTheme;
    public float dayVolume;
    public AudioClip nightTheme;
    public float nightVolume;
    public float duration;

	// Use this for initialization
	void Awake () {
        
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
        MusicManager.Instance.ChangeVolume(dayVolume);
        MusicManager.Instance.ChangeSong(duration, dayTheme);
    }
    public void OnNight() {
        MusicManager.Instance.ChangeVolume(nightVolume);
        MusicManager.Instance.ChangeSong(duration, nightTheme);
    }
}
