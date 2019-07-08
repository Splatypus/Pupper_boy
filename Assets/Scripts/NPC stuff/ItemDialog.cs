using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDialog : Dialog2
{

    //no set camera position for inspecting objects. Simply lock or unlock it
    public override void SetCameraPosition() {
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = true;
    }
    public override void RestoreCameraPosition() {
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = false;
    }

    //some functions removed from parent class since these items should not be able to be interacted with based on collision. 
    //Dialog progress also never needs to be saved

    public override void SaveDialogProgress() {
        //nothing
    }

    public override void LoadDialogProgress() {
        currentNode = startNode;
        //nothing
    }

    public override void OnTriggerEnter(Collider other) {
        //nothing
    }

    public override void OnTriggerExit(Collider other) {
        //nothing
    }
}
