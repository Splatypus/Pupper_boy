using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {

    public float bounceAmount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (other.CompareTag("Player")) {
            DogControllerV2 control = other.GetComponent<DogControllerV2>();
            control.v = new Vector3(control.v.x, bounceAmount, control.v.z);
        } else if (rb != null) {
            rb.velocity = new Vector3(rb.velocity.x, bounceAmount, rb.velocity.z);
        }
    }

}
