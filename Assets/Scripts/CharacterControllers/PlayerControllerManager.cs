using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour {

    public enum Modes { Walking, Dialog, MovementLock };
    public Controller currentController;
    Controller[] scripts = new Controller[3];

    public void Start() {
        //initial setting of all modes to their correct places
        scripts[(int)Modes.Walking] = gameObject.GetComponent<DogController>();
        scripts[(int)Modes.Dialog] = gameObject.GetComponent<PlayerDialog>();
        scripts[(int)Modes.MovementLock] = gameObject.GetComponent<NoMovementController>();

        //then disable all controller scripts except the default one (walking)
        foreach (Controller c in scripts) {
            //c.OnDeactivated();
            c.enabled = false;
        }
        currentController = scripts[(int)Modes.Walking];
        currentController.enabled = true;

        //scripts[(int)mode].OnActivated();
    }

    //disable the current mode, then change it to the new one and enable the new one
    public void ChangeMode(Modes newMode) {
        //call deactivation function and then disable the current controller
        currentController.OnDeactivated();
        currentController.enabled = false;
        //call activation function and enable next controller
        currentController = scripts[(int)newMode];
        currentController.enabled = true;
        currentController.OnActivated();
    }

    //disables the current mode. Activates a custom controller rather than a preset
    public void ChangeMode(Controller newController) {
        //call deactivation function and then disable the current controller
        currentController.OnDeactivated();
        currentController.enabled = false;
        //call activation function and enable next controller
        currentController = newController;
        currentController.enabled = true;
        currentController.OnActivated();
    }
}


//Controller parent class
public class Controller : MonoBehaviour {
    //OnEnable/OnDisable taken by unity already
    //called when this controller takes control
    public virtual void OnActivated() {

    }

    //called when this controler is turned off
    public virtual void OnDeactivated() {

    }

}
