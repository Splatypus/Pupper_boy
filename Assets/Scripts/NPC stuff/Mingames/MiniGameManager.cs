using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour {

    //public GameObject relatedNPC;
    public float timeLimit;
    public bool isPlaying = false;
    public GameObject PlayerCharacter;
    public Text canvasTimeField; //reference to the text box that displays time. Must be set
    public GameObject[] objectives;

    //public ArrayList highscores = new ArrayList(10);
    public float startTime;


    //called to start the minigame
    public virtual void GameStart() {
        startTime = Time.time;
        isPlaying = true;
        canvasTimeField.gameObject.SetActive(true);
    }

    //called to start the minigame
    public virtual void GameEnd() {
        //end event, add scores n stuff
        isPlaying = false;
        //clear time UI
        canvasTimeField.text = "";
        canvasTimeField.gameObject.SetActive(false);
    }

    public virtual void Start() {
        //set the player character reference on start
        PlayerCharacter = GameObject.FindGameObjectWithTag("Player");
        canvasTimeField.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	public virtual void Update () {
        if (isPlaying) {
            if (startTime + timeLimit < Time.time) {
                GameEnd();
            } else {
                //canvasTimeField.GetComponent<Text>().text = (startTime + timeLimit - Time.time).ToString();
                canvasTimeField.text = "Time: " + ((int)(startTime + timeLimit - Time.time)).ToString();
            }
        }
	}

    public virtual void ObjectiveReached(int ID) {

    }

}
