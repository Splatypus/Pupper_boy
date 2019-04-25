using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMovementController : Controller {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnActivated() {
        base.OnActivated();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = true;
       //Camera.main.GetComponent<FreeCameraLook>().MoveToPosition(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward, 0.0f);
    }

    public override void OnDeactivated() {
        base.OnDeactivated();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = false;
    }
}
