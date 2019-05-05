using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDialog : Controller {

    //UI elements
    public TMP_Text textObject;
    public Text nameTextObject;
    public Image imageObject;
    public GameObject[] buttons;
    public GameObject canvasGA;

    //displaying text
    [Header("Displaying Text")]
    [Tooltip("Rate at which text writes, in characters per second")]
    public float textSpeed;
    [Tooltip("Time between each visual update")]
    public float textUpdateTime;
    string textToShow;
    bool isAllShown;


    //reference to the current Dialog object this is interacting with. Set to null if there is none
    [HideInInspector]public Dialog2 npcDialog = null;

	// Update is called once per frame
	void Update () {
        //if any key is pressed, display the full text of the current text box, or if it already is, then display next
        if (Input.anyKeyDown && npcDialog != null) {
            //if all the text is not yet in the text box, put it all there
            if (!isAllShown) {
                textObject.text = textToShow;
                isAllShown = true;
                //if it is allready all in the textbox, go on to the next stuff, but only if dialog options are not available
            } else if (!buttons[0].activeInHierarchy) {
                isAllShown = false;
                npcDialog.Next();
            }
        } else if (Input.GetButtonDown("Cancel")) {
            gameObject.GetComponent<DogController>().escMenu.Hide();
        }
	}


    //notifies the player that dialog has started. Changes controls to the dialog box rather than movement
    public override void OnActivated() {
        
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
        //initially set everything to invis
        textObject.text = fullText;
        textObject.ForceMeshUpdate();
        int fullLength = textObject.textInfo.characterCount;
        for (int i = 0; i < fullLength; i++) {
            //textObject.textInfo.characterInfo[i].color.a = 0;
            int matIndex = textObject.textInfo.characterInfo[i].materialReferenceIndex;
            Color32[] vertColors = textObject.textInfo.meshInfo[matIndex].colors32;
            int vertIndex = textObject.textInfo.characterInfo[i].vertexIndex;
            vertColors[vertIndex + 0].a = 0;
            vertColors[vertIndex + 1].a = 0;
            vertColors[vertIndex + 2].a = 0;
            vertColors[vertIndex + 3].a = 0;
            textObject.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        int previousIndex = 0;
        float startTime = Time.time;
        while (!isAllShown) {
            int numToDisplay = (int)((Time.time - startTime) * textSpeed);
            numToDisplay = Mathf.Min(fullLength, numToDisplay);
            isAllShown = numToDisplay == fullLength;

            for (int i = previousIndex; i < numToDisplay; i++) {
                //textObject.textInfo.characterInfo[i].color.a = 255;
                int matIndex = textObject.textInfo.characterInfo[i].materialReferenceIndex;
                Color32[] vertColors = textObject.textInfo.meshInfo[matIndex].colors32;
                int vertIndex = textObject.textInfo.characterInfo[i].vertexIndex;
                vertColors[vertIndex + 0].a = 255;
                vertColors[vertIndex + 1].a = 255;
                vertColors[vertIndex + 2].a = 255;
                vertColors[vertIndex + 3].a = 255;
                textObject.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            previousIndex = numToDisplay;
            yield return new WaitForSeconds(textUpdateTime);
        }

        /*float startTime = Time.time;
        char[] t = new char[fullText.Length];
        while (!isAllShown) {
            int numToDisplay = (int)((Time.time - startTime) * textSpeed);
            numToDisplay = Mathf.Min(fullText.Length, numToDisplay);
            isAllShown = numToDisplay == fullText.Length;

            fullText.CopyTo(0, t, 0, numToDisplay);
            textObject.text = new string(t);
            yield return new WaitForSeconds(textUpdateTime);
        }*/

        /*
        string str = "";
        for (int i = 0; i < fullText.Length && !isAllShown; i++) {
            str += fullText[i];
            textObject.text = str;
            yield return new WaitForSeconds(textUpdateTime);
        }
        textObject.text = fullText;
        isAllShown = true;
        */
    }

}
