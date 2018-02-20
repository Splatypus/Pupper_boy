using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour {

   
    PlayerDialog pdialog;
    PlayerControllerManager controlman;

    // Use this for initialization
    void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        controlman = player.GetComponent<PlayerControllerManager>();
        pdialog = player.GetComponent<PlayerDialog>(); //find player dialog script on the player and set this to refrence it
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
