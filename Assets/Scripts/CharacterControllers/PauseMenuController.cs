using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : Controller
{

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public override void OnActivated() {
        base.OnActivated();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = true;
    }

    public override void OnDeactivated() {
        base.OnDeactivated();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = false;
    }
}
