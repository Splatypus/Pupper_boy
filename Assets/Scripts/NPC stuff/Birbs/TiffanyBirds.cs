using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffanyBirds : BirdMovementV2 {

    public TiffyAI tiffanyReference;

    public AudioClip flyAwaySound;

    //when the bird is scared off, let tiffany know
    public override void StartFlight() {
        base.StartFlight();

        if (Random.Range(0, 1) < .5f) {
            GetComponent<AudioSource>().clip = flyAwaySound;
            GetComponent<AudioSource>().Play();
        }

        tiffanyReference.BirdScared();
    }
}
