using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchObjectPosition : MonoBehaviour {

    public GameObject trackedObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = trackedObject.transform.position;
	}
}
