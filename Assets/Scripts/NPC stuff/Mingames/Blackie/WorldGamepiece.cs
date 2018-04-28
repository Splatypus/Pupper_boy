using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGamepiece : Interactable {

    Rigidbody rb;
    bool inZone;
    bool isHeld;
    BlackieMiniGame gameSource;
    public GameObject targeter;
    float distance;
    Vector3 offsets;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        //set up the targeter gameobject
        targeter = Instantiate(targeter);
        gameSource = FindObjectOfType<BlackieMiniGame>();
        distance = gameSource.tileDis;
        offsets = gameSource.gameObject.transform.position;
        offsets.x %= distance;
        offsets.z %= distance;
        targeter.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (inZone && isHeld) {
            //set the targeter's location to snap to the grid
            Vector3 preSnap = transform.position;
            preSnap.x -= distance / 2.0f;
            preSnap.z -= distance / 2.0f;
            targeter.transform.position = new Vector3(preSnap.x - (preSnap.x % distance) + offsets.x, 0.5f, preSnap.z - (preSnap.z% distance) + offsets.z);
        }
	}

    bool checkShowTarget() {
        return isHeld && inZone;
    }

    public override void onPickup()
    {
        base.onPickup();
        isHeld = true;
        targeter.SetActive(checkShowTarget());
        //rb.isKinematic = false;
    }

    public override void onDrop()
    {
        base.onDrop();
        isHeld = false;
        targeter.SetActive(checkShowTarget());
        //respawn item?
        //rb.isKinematic = true;
    }

    //let set inZone variable if we enter or leave the zone with this object
    public void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("GameArea")) { 
            inZone = true;
            targeter.SetActive(checkShowTarget());
        } else if (other.gameObject.GetComponent<DigZone>() != null) {
            //respawn?
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("GameArea")) { 
            inZone = false;
            targeter.SetActive(checkShowTarget());
        }
    }
}
