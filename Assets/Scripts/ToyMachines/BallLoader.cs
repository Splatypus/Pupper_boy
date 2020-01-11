using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When a tennis ball not being held by doggo collides with this object, attempt to load it into the main launcher.

public class BallLoader : MonoBehaviour
{
    public BallLauncher owningLauncher;

    public void OnTriggerEnter(Collider other) {
        PuppyPickup.IPickupItem pickup = other.GetComponent<PuppyPickup.IPickupItem>();
        PuppyPickup mouth = GameObject.FindGameObjectWithTag("Player").GetComponent<DogController>().mouth;
        if (pickup == null || mouth?.itemInMouth == other.gameObject)
            return;

        owningLauncher.LoadBall(other.gameObject);
    }
}
