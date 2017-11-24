using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour {

    //public GameObject relatedNPC;
    public float timeLimit;
    public bool isPlaying = false;

    //public ArrayList highscores = new ArrayList(10);
    public float startTime;


    //called to start the minigame
    public virtual void GameStart() {
        startTime = Time.time;
        isPlaying = true;
    }

    //called to start the minigame
    public virtual void GameEnd() {
        //end event, add scores n stuff
        isPlaying = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (startTime + timeLimit < Time.time) {
            GameEnd();
        }
	}
}
