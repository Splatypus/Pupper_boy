using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : InteractableObject {

   
    PlayerDialog pdialog;
    PlayerControllerManager controlman;
    public string[] dialogTexts;
    public int[] dialogStarts; //the indecies in the dialogTexts array in which each conversation starts
    public int conversationNumber; //this indicates which conversation you're on. 
    public int textBoxNumber; //this indicates which text box you're on within the full array of dialog. 

    // Use this for initialization
    void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        controlman = player.GetComponent<PlayerControllerManager>();
        pdialog = player.GetComponent<PlayerDialog>(); //find player dialog script on the player and set this to refrence it
	}

    public override void OnInteract() {
        //change player mode to dialog mode when they interact with this npc
        controlman.ChangeMode(PlayerControllerManager.Modes.Dialog);
        pdialog.npcDialog = this;
        textBoxNumber = dialogStarts[conversationNumber];
        pdialog.SetDialog(ref dialogTexts[textBoxNumber]);
    }

    //goes on to the next dialog window. Called from the player dialog when more text is needed.
    public void Next() {
        //either display the next dialog or close window
        textBoxNumber += 1;
        if (conversationNumber < dialogStarts.Length && textBoxNumber < dialogStarts[conversationNumber + 1] && textBoxNumber <= dialogTexts.Length) {//check to see if the end of this dialog section has been reached
            pdialog.SetDialog(ref dialogTexts[textBoxNumber]); //show the next box
        } else {
            controlman.ChangeMode(PlayerControllerManager.Modes.Walking); //return to walking
        }
        
    }

    public bool SetConversationNumber(int c) { //sets the current conversation number to whatever you pass in. False return if failed or reached the end 
        if (c < 0 || c > dialogStarts.Length) {
            return false;
        }
        conversationNumber = c;
        return true;
    }

    public bool SetConversationNumber() {//if no arguements, it increments it by 1 rather than setting
        if (conversationNumber + 1 > dialogStarts.Length) {
            return false;
        }
        conversationNumber += 1;
        return true;
    }

}
