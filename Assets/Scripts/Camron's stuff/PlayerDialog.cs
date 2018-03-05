using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialog : Controller {

    //UI elements
    public Text textObject;
    public GameObject canvasGA;

    //displaying text
    public float textSpeed;
    string textToShow;
    bool isAllShown;

    //reference to the current Dialog object this is interacting with. Set to null if there is none
    public Dialog npcDialog = null;

    
	
	// Update is called once per frame
	void Update () {
        //if any key is pressed, display the full text of the current text box, or if it already is, then display next
        if (Input.anyKeyDown) {
            //if all the text is not yet in the text box, put it all there
            if (!isAllShown) {
                textObject.text = textToShow;
                isAllShown = true;
            //if it is allready all in the textbox, go on to the next stuff
            } else {
                isAllShown = false;
                npcDialog.Next();
            }
        }
	}


    //notifies the player that dialog has started. Changes controls to the dialog box rather than movement
    public override void OnActivated() {
        //Open dialog window
        canvasGA.SetActive(true);
    }

    //ends dialog, closes window, and reverts controls to normal
    public override void OnDeactivated() {
        textToShow = "";
        isAllShown = false;
        npcDialog = null;
        canvasGA.SetActive(false);
    }

    //sets the text in the active dialog box
    public void SetDialog(ref string text) {
        isAllShown = false;
        textToShow = text;
        StartCoroutine(AnimateText(text));
    }

    //animates the text so that it appears one letter at a time
    IEnumerator AnimateText(string fullText) {
        string str = "";
        for (int i = 0; i < fullText.Length && !isAllShown; i++) {
            str += fullText[i];
            textObject.text = str;
            yield return new WaitForSeconds(1.0f/textSpeed);
        }
        isAllShown = true;
    }


}
