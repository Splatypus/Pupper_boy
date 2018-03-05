using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : InteractableObject {

   
    PlayerDialog pdialog;
    PlayerControllerManager controlman;
    public string[] dialogTexts;

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
        pdialog.SetDialog(ref dialogTexts[0]);
    }

    //goes on to the next dialog window. Called from the player dialog when more text is needed.
    public void Next() {
        //either display the next dialog or close window
        controlman.ChangeMode(PlayerControllerManager.Modes.Walking);
    }

}
