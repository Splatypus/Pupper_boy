using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightMusicChange : MonoBehaviour {

    public AudioClip dayTheme;
    public AudioClip nightTheme;
    public AudioSource source;
    public float duration;

	// Use this for initialization
	void Start () {
        if (source == null)
            source = gameObject.GetComponent<AudioSource>();

        //set initial states
        DayNightManager dayNight = DayNightManager.Instance;
        if (dayNight.currentTime == DayNightManager.Times.DAY) {
            source.clip = dayTheme;
        } else {
            source.clip = nightTheme;
        }
        //set up triggers
        dayNight.AddTrigger(DayNightManager.Times.DAY, new UnityAction(OnDay));
        dayNight.AddTrigger(DayNightManager.Times.NIGHT, new UnityAction(OnNight));
    }

    //functions to call when it becomes day or night. Changes the theme to the correct one
    public void OnDay() {
        MusicManager.Instance.ChangeSong(duration, dayTheme);
    }
    public void OnNight() {
        MusicManager.Instance.ChangeSong(duration, nightTheme);
    }
}
