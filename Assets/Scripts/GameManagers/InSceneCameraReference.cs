using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class is used as a substitute for camera functions in other scenes.
//For example, since the camera only exists in the main backyard scene, nothing in the summer scene can be assigned a reference to it in the editor
public class InSceneCameraReference : MonoBehaviour {

    GameObject cameraReference;
    FreeCameraLook scriptReference;

    // Set up references. If a main scene with a camera is not yet loaded, this should fail
    void Start() {
        cameraReference = GameObject.FindGameObjectWithTag("MainCamera");
        scriptReference = cameraReference.GetComponent<FreeCameraLook>();
        if (cameraReference == null || scriptReference == null) {
            Debug.LogError("Error: InSceneCameraReference failed trying to find main scene camera");
        }
    }

    public GameObject getCamera() {
        return cameraReference;
    }

    //functions to move and control the main camera
    public void SetTargetPosition(GameObject t) { scriptReference.SetTargetPosition(t); }
    public void SetTargetLookAt(GameObject t) { scriptReference.SetTargetLookAt(t); }
    public void RunSetMove(float time) { scriptReference.RunSetMove(time); }
    public void RestoreCamera(float time) { scriptReference.RestoreCamera(time); }
    public void MoveToPosition(Vector3 location, Vector3 lookAt, float duration) { scriptReference.MoveToPosition(location, lookAt, duration); }
}
