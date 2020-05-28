using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    [Tooltip("The force to apply upwards on objects dropped on the trampoline. (adds force)")]
    public float bounceAmount;
    [Tooltip("The amount of the initial veleocity to keep (inverted).")]
    public float velocitySaved;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();	
	}

    private void OnTriggerEnter(Collider other) {
        //do nothing unless this hits a real collider
        if (other.isTrigger)
            return;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (other.CompareTag("Player")) {
            //if the player collides with this while moving downward, bounce them
            PlayerControllerManager control = other.GetComponent<PlayerControllerManager>();
            if (control.v.y < -0.3f) {
                control.v = new Vector3(control.v.x, bounceAmount + (-velocitySaved * control.v.y), control.v.z);
                audioSource?.Play();
            }
        } else if (rb != null && rb.useGravity) {
            //if an object collides with this while moving downward, bounce it
            if (rb.velocity.y < 1.0f) {
                rb.velocity = new Vector3(rb.velocity.x, bounceAmount + (-velocitySaved * rb.velocity.y), rb.velocity.z);
                audioSource?.Play();
            }
        }
    }

}
