using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackieGameManager : MiniGameManager {
    /*
    public GameObject bubble_particle_system;
    public int score = 0;
    public int numberOfActiveObjectives;
    List<int> activeObjectives;

    public Text scoreText;
    */
    /*
    [SerializeField] List<int> highscores = new List<int>(); //Sorted where highest score is at position [9] and lowest is at [0]
    public int maxHighScores = 10;
    */
    public int rewardScore;
    
    public GameObject blackieRef;

    //called when the minigame is started
    public override void GameStart() {
        if (!isPlaying) {
            base.GameStart();
            /*
            scoreText.gameObject.SetActive(true);
            scoreText.text = "Score: 0";
            bubble_particle_system.SetActive(true);
            score = 0;
            //activate objectives
            for (int i = 0; i < numberOfActiveObjectives; i++) {
                activeObjectives[i] = SelectObjective();
                objectives[activeObjectives[i]].SetActive(true);
            }*/
        }
    }

    public void GameStartForReward(int scoreGoal) {

        rewardScore = scoreGoal;
        GameStart();
    }

    
    //deactivate all when game is over #TODO: add highscore setting and all that
    public override void GameEnd() {
        base.GameEnd();
        /*
        //reset score text and particle systems
        scoreText.gameObject.SetActive(true);
        scoreText.text = "";
        bubble_particle_system.SetActive(false);
        //disable objectives
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].SetActive(false);
        }
        //if a new high score has been set, update the list of high scores
        if (highscores.Count < maxHighScores) {
            highscores.Add(score);
            highscores.Sort();
        } else if (highscores.Count > 0 && score > highscores[0]) {
            highscores[0] = score;
            highscores.Sort();
        }
        if (score >= rewardScore) {
            bubblesRef.GetComponent<BubblesAI>().FinishedGame(score >= rewardScore);
        }*/

        blackieRef.GetComponent<BlackieAI>().FinishedGame(true);
    }

    /*
    //called when an objective (bubble) is reached
    public override void ObjectiveReached(int index) {
        score++;
        scoreText.text = "Score: " + score;
        NewObjective(index);
    }*/

    /*
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
        //bubble_particle_system.transform.forward = objectives[i].transform.position - transform.position;
        transform.forward = objectives[i].transform.position - transform.position;
        return i;
    }*/

    /*
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
    }*/


	// Use this for initialization
	public override void Start () {
        //Do base start. Find player character, etc
        base.Start();
        //scoreText.gameObject.SetActive(false);
        //Hide bubble particle system
        //bubble_particle_system.SetActive(false); 
        //Disable objectives to start
        //for (int i = 0; i < objectives.Length; i++) {
          //  objectives[i].GetComponent<Objective>().SetUp(this, i);
          //  objectives[i].SetActive(false);
        //}
        //set up objective list
        //activeObjectives = new List<int>(numberOfActiveObjectives);
        //for (int i = 0; i < numberOfActiveObjectives; i++) { //yes this sucks, but lists dont allow for setting an initial length.
          //  activeObjectives.Add(0);
        //}
    }

    void OnTriggerEnter(Collider col) {
        /*if (col.gameObject.CompareTag("Player") && !isPlaying) {
            GameStart();
        }*/
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
    }
}
