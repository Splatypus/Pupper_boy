﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    MiniGameManager manager;
    int ID;


    public void SetUp(MiniGameManager m, int i) {
        manager = m;
        ID = i;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //let the manager know that this objective has been reached by the player
    void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Player")) {
            manager.ObjectiveReached(ID);
        }
    }
}