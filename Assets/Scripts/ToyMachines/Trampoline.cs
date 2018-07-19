using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    public float bounceAmount;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();	
	}

    private void OnTriggerEnter(Collider other) {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (other.CompareTag("Player")) {
            //if the player collides with this while moving downward, bounce them
            DogControllerV2 control = other.GetComponent<DogControllerV2>();
            if (control.v.y < -1.0f) {
                control.v = new Vector3(control.v.x, bounceAmount, control.v.z);
                audioSource.Play();
            }
        } else if (rb != null && rb.useGravity) {
            //if an object collides with this while moving downward, bounce it
            if (rb.velocity.y < 1.0f) {
                rb.velocity = new Vector3(rb.velocity.x, bounceAmount, rb.velocity.z);
                audioSource.Play();
            }
        }
    }

}
