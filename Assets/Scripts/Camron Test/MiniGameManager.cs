using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour {

    public GameObject relatedNPC;
    public float timeLimit;


    //public ArrayList highscores = new ArrayList();
    float startTime;


    //called to start the minigame
    public virtual void GameStart() {
        startTime = Time.time;
    }

    //called to start the minigame
    public virtual void GameEnd() {
        //end event, add scores n stuff
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (startTime + timeLimit < Time.time) {
            GameEnd();
        }
	}
}
