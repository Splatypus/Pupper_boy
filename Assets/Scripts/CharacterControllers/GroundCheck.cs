using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    public DogControllerV2 control;

	// Use this for initialization
	void Start () {
        if (control == null) {
            control = GameObject.FindGameObjectWithTag("Player").GetComponent<DogControllerV2>();
        }
	}

    private void OnTriggerEnter(Collider other) {
        //if (other.CompareTag("Ground"))
        if(!other.isTrigger)
            control.OnGroundEnter();
    }

    private void OnTriggerExit(Collider other) {
        //if (other.CompareTag("Ground"))
        if (!other.isTrigger)
            control.OnGroundExit();
    }
}
