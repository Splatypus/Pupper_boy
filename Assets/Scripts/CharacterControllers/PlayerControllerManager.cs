using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour {

    public enum Modes { Walking, Dialog, MovementLock, Dragging, Pause };
    public Controller currentController;
    Controller[] scripts = new Controller[5];

    //velocity - shared between all control scripts
    [HideInInspector] public Vector3 v;
    [HideInInspector] public Modes currentMode;

    public void Start() {
        //initial setting of all modes to their correct places
        scripts[(int)Modes.Walking] = gameObject.GetComponent<DogController>();
        scripts[(int)Modes.Dialog] = gameObject.GetComponent<PlayerDialog>();
        scripts[(int)Modes.MovementLock] = gameObject.GetComponent<NoMovementController>();
        scripts[(int)Modes.Dragging] = gameObject.GetComponent<DraggingController>();
        scripts[(int)Modes.Pause] = gameObject.GetComponent<PauseMenuController>();

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
    public Controller ChangeMode(Modes newMode) {
        currentMode = newMode;
        //call deactivation function and then disable the current controller
        currentController.OnDeactivated();
        currentController.enabled = false;
        //call activation function and enable next controller
        currentController = scripts[(int)newMode];
        currentController.enabled = true;
        currentController.OnActivated();
        return currentController;
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
