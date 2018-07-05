using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialog : Controller {

    //UI elements
    public Text textObject;
    public Text nameTextObject;
    public Image imageObject;
    public GameObject[] buttons;
    public GameObject canvasGA;

    //displaying text
    public float textSpeed;
    string textToShow;
    bool isAllShown;


    //reference to the current Dialog object this is interacting with. Set to null if there is none
    public Dialog2 npcDialog = null;

	// Update is called once per frame
	void Update () {
        //if any key is pressed, display the full text of the current text box, or if it already is, then display next
        if (Input.anyKeyDown) {
            //if all the text is not yet in the text box, put it all there
            if (!isAllShown) {
                textObject.text = textToShow;
                isAllShown = true;
            //if it is allready all in the textbox, go on to the next stuff, but only if dialog options are not available
            } else if(!buttons[0].activeInHierarchy){
                isAllShown = false;
                npcDialog.Next();
            }
        }
	}


    //notifies the player that dialog has started. Changes controls to the dialog box rather than movement
    public override void OnActivated() {
        //Open dialog window
        canvasGA.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //ends dialog, closes window, and reverts controls to normal
    public override void OnDeactivated() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        textToShow = "";
        isAllShown = false;
        npcDialog = null;
        canvasGA.SetActive(false);
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].SetActive(false);
        }
    }

    //sets the text in the active dialog box
    public void SetDialog(string text) {
        isAllShown = false;
        textToShow = text;
        StartCoroutine(AnimateText(text));
    }

    //sets the option UI up
    public void AddOptions(List<string> opts) {
        for (int i = 0; i < opts.Count; i++) {
            buttons[i].SetActive(true);
            buttons[i].GetComponentInChildren<Text>().text = opts[i];
        }
    }

    //adds a single UI option, at the button specified by "index"
    public void AddOption(int index, string opt){
        buttons[index].SetActive(true);
        buttons[index].GetComponentInChildren<Text>().text = opt;
    }

    //when a button is clicked
    public void OnPress(int num) {
        foreach (GameObject b in buttons) {
            b.SetActive(false);
        }
        npcDialog.OnChoiceMade(num);
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
