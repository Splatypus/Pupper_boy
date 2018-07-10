using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager Instance;
    public AudioClip defaultTheme;
    public AudioSource source;

    bool isChanging = false; //lock for coroutine music changes

    private void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        source = gameObject.GetComponent<AudioSource>();
	}

    //changes the song to the given audioclip over the given duration
    public void ChangeSong(float duration, AudioClip song) {
        if (isChanging)
            source.clip = song;
        else
            StartCoroutine(FadeMusic(duration, song));
    }

    //actual call to fade the music out then back in as the new song
    IEnumerator FadeMusic(float duration, AudioClip song) {
        isChanging = true;
        float startTime = Time.time;
        float originalVolume = source.volume;
        //fade out
        while (Time.time < startTime + duration) {
            source.volume = Mathf.Lerp(originalVolume, 0, (Time.time - startTime) / duration);
            yield return new WaitForFixedUpdate();
        }
        //then in
        source.clip = song;
        source.Play();
        startTime = Time.time;
        while (Time.time < startTime + duration) {
            source.volume = Mathf.Lerp(0, originalVolume, (Time.time - startTime) / duration);
            yield return new WaitForFixedUpdate();
        }
        //just in case, make sure to set the volume to the original
        source.volume = originalVolume;
        //and unlock the song to be changed again
        isChanging = false;
    }

}
