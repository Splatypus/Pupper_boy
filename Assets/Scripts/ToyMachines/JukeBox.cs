using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : AIbase {

    public AudioSource source;
    public AudioClip originalTheme;
    public float fadeDuration;

    public override void OnInRange() {
        base.OnInRange();
        Display(0);
    }

    //Plays the given song as the current theme
    public void PlaySong(AudioClip song) {
        MusicManager.Instance.ChangeSong(fadeDuration, song);
    }
}
