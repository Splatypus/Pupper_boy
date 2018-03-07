using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour {

    public enum Modes { Walking, Dialog, Flight };
    public Modes mode = Modes.Walking;
    Controller[] scripts = new Controller[3];

    public void Start() {
        //initial setting of all modes to their correct places
        scripts[(int)Modes.Walking] = gameObject.GetComponent<DogControllerV2>();
        scripts[(int)Modes.Dialog] = gameObject.GetComponent<PlayerDialog>();
        scripts[(int)Modes.Flight] = gameObject.GetComponent<FlightMode>();

        //then disable all controller scripts except the default one (walking)
        foreach (Controller c in scripts) {
            c.enabled = false;
        }
        scripts[(int)mode].enabled = true;
    }

    //disable the current mode, then change it to the new one and enable the new one
    public void ChangeMode(Modes newMode) {
        //call deactivation function and then disable the current controller
        scripts[(int)mode].OnDeactivated();
        scripts[(int)mode].enabled = false;
        mode = newMode;
        //call activation function and enable next controller
        scripts[(int)mode].enabled = true;
        scripts[(int)mode].OnActivated();
    }
}


//Controller parent class
public class Controller : MonoBehaviour {
    //called when this controller takes control
    public virtual void OnActivated() {

    }

    //called when this controler is turned off
    public virtual void OnDeactivated() {

    }

}
