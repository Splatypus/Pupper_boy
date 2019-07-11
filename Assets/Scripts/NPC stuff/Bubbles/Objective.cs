using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    public GameObject canvas;
    BubbleGameManager manager;
    int ID;
    bool isActive = false;


    public void SetUp(BubbleGameManager m, int i) {
        manager = m;
        ID = i;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetIsVisible(bool isVisible) {
        isActive = isVisible;
        canvas.SetActive(isVisible);
    }

    //let the manager know that this objective has been reached by the player
    void OnTriggerEnter(Collider col) {
        if (isActive && col.gameObject.CompareTag("Player")) {
            //play a sound
            AudioSource a = gameObject.GetComponent<AudioSource>();
            if (a != null)
                a.Play();
            manager.ObjectiveReached(ID);
        }
    }
}
