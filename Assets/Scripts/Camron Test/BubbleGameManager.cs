using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGameManager : MiniGameManager {



    public GameObject[] objectives;
    public int score = 0;
    public int numberOfActiveObjectives;
    List<int> activeObjectives;

    List<int> highscores = new List<int>(10); //Sorted where highest score is at position [9] and lowest is at [0]

    //called when the minigame is started
    public override void GameStart() {
        base.GameStart();
        score = 0;
        //activate objectives
        for (int i = 0; i < numberOfActiveObjectives; i++) {
            activeObjectives[i] = SelectObjective();
            objectives[activeObjectives[i]].SetActive(true);
        }
    }

    //deactivate all when game is over #TODO: add highscore setting and all that
    public override void GameEnd() {
        base.GameEnd();
        //disable objectives
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].SetActive(false);
        }
        //if a new high score has been set, update the list of high scores
        if (score > highscores[0]) {
            highscores[0] = score;
        }
        highscores.Sort();
    }

    //called when an objective (bubble) is reached
    public void ObjectiveReached(int index) {
        score++;
        print("Score: " + score);
        NewObjective(index);
    }

    //Select an objective that is not currently selected. Return its index
    public int SelectObjective() {
        //generate a random number between 0 and the number of objectives to use that is not in the array of already active objectives
        int i = (int)Random.Range(0, objectives.Length - numberOfActiveObjectives);
        activeObjectives.Sort();
        for(int ex = 0; ex < numberOfActiveObjectives; ex++) {
            if (i >= activeObjectives[ex]) {
                i++;
            } else {
                break;
            }
        }
        return i;
    }

    //deactivates the objective index and selects a new one to activate
    public void NewObjective(int index) {
        int i = SelectObjective();
        objectives[i].SetActive(true);
        //change the activeObjectives array to reflect that i is now active, rather than whatever we had before
        for (int j = 0; j < numberOfActiveObjectives; j++) {
            if (activeObjectives[j] == index) {
                objectives[activeObjectives[j]].SetActive(false);
                activeObjectives[j] = i;
            }
        }
    }


	// Use this for initialization
	void Start () {
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].GetComponent<Objective>().SetUp(this, i);
            objectives[i].SetActive(false);
        }
        activeObjectives = new List<int>(numberOfActiveObjectives);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
