using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSoundsWithDelay : MonoBehaviour
{
    AudioSource source;
    public AudioClip[] sounds;
    public float durationMin = 15.0f;
    public float durationMax = 15.0f;
    public bool playOnAwake = true;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        if (playOnAwake)
            StartSounds();
    }

    public void StartSounds() {
        StartCoroutine(SoundLoop());
    }

    IEnumerator SoundLoop() {
        yield return new WaitForSeconds(Random.Range(durationMin, durationMax));
        source.clip = sounds[Random.Range(0, sounds.Length)];
        source.Play();
        StartCoroutine(SoundLoop());
    }

}
