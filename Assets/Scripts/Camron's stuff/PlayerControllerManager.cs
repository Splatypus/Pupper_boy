using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour {

    public enum Modes { Walking, Dialog, Flight };
    public Modes mode = Modes.Walking;
    public MonoBehaviour[] scripts;

    public void Start() {
        //initial setting of all modes to their correct places
        scripts[(int)Modes.Walking] = gameObject.GetComponent<DogControllerV2>();
        scripts[(int)Modes.Dialog] = gameObject.GetComponent<PlayerDialog>();
        scripts[(int)Modes.Flight] = gameObject.GetComponent<FlightMode>();

        //then disable all controller scripts except the default one (walking)
        foreach (MonoBehaviour m in scripts) {
            m.enabled = false;
        }
        scripts[(int)Modes.Walking].enabled = true;
    }

    //disable the current mode, then change it to the new one and enable the new one
    public void ChangeMode(Modes newMode) {
        scripts[(int)mode].enabled = false;
        mode = newMode;
        scripts[(int)mode].enabled = true;

    }
	
}
