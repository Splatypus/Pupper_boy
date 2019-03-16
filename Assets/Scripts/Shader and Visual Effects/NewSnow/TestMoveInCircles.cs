using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveInCircles : MonoBehaviour {


    Vector3 initialpos;
	// Use this for initialization
	void Start () {
        initialpos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = initialpos + new Vector3(Mathf.Cos(Time.time) * 3, Mathf.Sin(Time.time*2)/2, Mathf.Sin(Time.time) * 3);
	}
}
