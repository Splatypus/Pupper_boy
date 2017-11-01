using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class birdTracker : MonoBehaviour {

    public int birdsDiving = 5;
    private bool birdsLeft = true;
    bool has_spawned = false;
    public GameObject dog;
    public GameObject location;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (birdsDiving <= 0)
        {
            birdsLeft = false;
        }
        if (birdsLeft == false && !has_spawned)
        {
            Instantiate(dog, location.transform.position, location.transform.rotation);
            birdsLeft = true;
            has_spawned = true;
        }
	}
}
