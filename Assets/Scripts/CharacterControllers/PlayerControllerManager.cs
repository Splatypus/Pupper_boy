using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour
{

    public enum PlayerControllerMode { Walking, Dialog, Flight, MovementLock };
    public PlayerControllerMode myCurrentMode;

    private Controller currentController;
    public Controller CurrentController
    {
        get => currentController;
        set
        {
            myControllers.TryGetValue(myCurrentMode, out Controller outContr);
            outContr.enabled = false;

            currentController = value;

            myControllers.TryGetValue(myCurrentMode, out outContr);
            outContr.enabled = true;
        }
    }
    private Controller outController;

    private Dictionary<PlayerControllerMode, Controller> myControllers = new Dictionary<PlayerControllerMode, Controller>();
    public Controller controllerOverride;


    //Controller[] scripts = new Controller[4];

    private void Start()
    {
        ///<summary>
        ///Prevoius Use
        /// </summary>
        ///scripts[(int)PlayerControllerMode.Walking] = gameObject.GetComponent<DogControllerV2>();
        ///scripts[(int)PlayerControllerMode.Dialog] = gameObject.GetComponent<PlayerDialog>();
        ///scripts[(int)PlayerControllerMode.Flight] = gameObject.GetComponent<FlightMode>();
        ///scripts[(int)PlayerControllerMode.MovementLock] = gameObject.GetComponent<NoMovementController>();
        ///initial setting of all modes to their correct places

        myControllers.Add(PlayerControllerMode.Walking, gameObject.GetComponent<MovementController>());
        myControllers.Add(PlayerControllerMode.Dialog, gameObject.GetComponent<PlayerDialog>());
        myControllers.Add(PlayerControllerMode.Flight, gameObject.GetComponent<FlightMode>());
        myControllers.Add(PlayerControllerMode.MovementLock, gameObject.GetComponent<NoMovementController>());

        //Get rid of Unused Controllers
        Controller[] allControllers = GetComponents<Controller>();
        foreach(Controller c in allControllers)
        {
            if (!myControllers.ContainsValue(c))
            {
                Destroy(c);
            }
        }

        //then disable all controller scripts except the default one (walking)
        foreach (KeyValuePair<PlayerControllerMode, Controller> keyValuePair in myControllers)
        {
            //c.OnDeactivated();
            //c.enabled = false;

            keyValuePair.Value.enabled = false;
        }
        if (!controllerOverride)
        {
            myControllers.TryGetValue(PlayerControllerMode.Walking, out outController);
            CurrentController = outController;
            CurrentController.enabled = true;
        }
    }

    //disable the current mode, then change it to the new one and enable the new one
    public void ChangeMode(PlayerControllerMode newMode)
    {
        myCurrentMode = newMode;
        myControllers.TryGetValue(myCurrentMode, out outController);
        CurrentController = outController;


      ///<summary>
      ///Previous Use
      ///</summary>
      ///if (!controllerOverride)
      /// {
      ///     //call deactivation function and then disable the current controller
      ///     currentController.OnDeactivated();
      ///     currentController.enabled = false;
      ///     //call activation function and enable next controller
      ///     currentController = scripts[(int)newMode];
      ///     currentController.enabled = true;
      ///     currentController.OnActivated();
      ///  }
    }
    /// <summary>
    /// Previous Use
    /// </summary>
    /// <param name="newController"></param>
    ///disables the current mode. Activates a custom controller rather than a preset
    ///public void ChangeMode(Controller newController)
    ///{
    ///    if (!controllerOverride)
    ///    {//call deactivation function and then disable the current controller
    ///        currentController.OnDeactivated();
    ///        currentController.enabled = false;
    ///        //call activation function and enable next controller
    ///        currentController = newController;
    ///        currentController.enabled = true;
    ///        currentController.OnActivated();
    ///    }
    ///}
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
