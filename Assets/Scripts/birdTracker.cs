using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdTracker : MonoBehaviour {

    public int birdsDiving = 5;
    private bool birdsLeft = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (birdsDiving <= 0)
        {
            birdsLeft = false;
        }
	}
}
