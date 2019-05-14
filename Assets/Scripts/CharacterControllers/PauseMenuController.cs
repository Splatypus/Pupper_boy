using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : Controller
{

    CharacterController cc;
    PlayerControllerManager manager;
    DogController moveStats;
    // Use this for initialization
    void Start() {
        cc = GetComponent<CharacterController>();
        manager = GetComponent<PlayerControllerManager>();
        moveStats = GetComponent<DogController>();
    }

    // Update is called once per frame
    void Update() {
        //gravity
        if (cc.isGrounded) {
            manager.v = Vector3.zero;
        } else {
            manager.v.y -= moveStats.gravity * Time.deltaTime;
        }
        //set movement
        cc.Move(manager.v * Time.deltaTime);
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
