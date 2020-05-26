using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthAgent : MonoBehaviour
{
    [Header("Setup")]
    public StealthManager manager;
    public AudioSource sound;

    private void OnTriggerEnter(Collider other)
    {

        //Only care about the player
        if (!other.CompareTag("Player")) {
            return;
        }

        //Play a sound if we have one
        if (sound) {
            sound.Play();
        }

        //notify manager
        manager.FoundPlayer(this, other.gameObject);
    }
}
