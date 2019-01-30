using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for enabling and disabling dig zones as needed
public class FenceUnlockManager : MonoBehaviour {

    public bool startAllDisabled;

    DigZone[] allDigZones;

	// Use this for initialization
	void Start () {
        allDigZones = GetComponentsInChildren<DigZone>();
        if (startAllDisabled) {
            DisableAll();
        }
	}

    //enables all dig zones for digging again
    public void EnableAll() {
        foreach (DigZone d in allDigZones) {
            d.GetComponent<Collider>().enabled = true;
        }
    }

    //disables all dig zones
    public void DisableAll() {
        foreach (DigZone d in allDigZones) {
            d.GetComponent<Collider>().enabled = false;
        }
    }

    //Enables all dig zones into (and out of) the designated yard
    public void EnableIntoYard(int y) {
        print("Enabling digzones into " + DigZone.yardNames[y]);
        foreach (DigZone d in allDigZones) {
            print("is looping");
            if ((int)d.enteringYard == y) {
                print("setting active");
                d.GetComponent<Collider>().enabled = true;
                d.other_side.GetComponent<Collider>().enabled = true;
            }
        }
    }
    //same function but when actually given the enum rather than an int. However, cannot be assigned as a function in dialog editor
    public void EnableIntoYard(DigZone.Yards y) {
        EnableIntoYard((int)y);
    }


}