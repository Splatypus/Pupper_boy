using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleBoat : MonoBehaviour {

    float initialHeight;
    public float waveSpeed;
    public float waveHeight;

    // Use this for initialization
	void Start () {
        initialHeight = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x,
                                            initialHeight + Mathf.Sin(Time.time * waveSpeed + transform.position.x + transform.position.z) * waveHeight,
                                            transform.position.z);
	}
}
