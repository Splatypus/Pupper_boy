using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialog : MonoBehaviour {

    //UI
    public Text textObject;
    public GameObject canvasGA;

    //player controller
    bool isActive = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //notifies the player that dialog has started. Changes controls to the dialog box rather than movement
    public void StartDialog() {
        //switch player controls
        isActive = true;
        //Open dialog window
        canvasGA.SetActive(true);
    }

    //ends dialog, closes window, and reverts controls to normal
    public void EndDialog() {
        isActive = false;
        canvasGA.SetActive(false);
    }

    //sets the text in the active dialog box
    public void SetDialog(ref string text) {
        if (isActive) {
            textObject.text = text;
        }
    }


}
