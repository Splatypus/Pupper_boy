using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceMinigame : MiniGameManager {

    public int lapsNeeded;
    int score = 0;
    int lapsDone = 0;

    [SerializeField]
    List<float> highscores = new List<float>(); //Sorted where highest time taken is at position [9] and lowest is at [0]
    public int maxHighScores = 10;

    //called when the minigame is started
    public override void GameStart() {
        base.GameStart();
        //set score to 0 and activate the first objective
        score = 0;
        objectives[0].SetActive(true);
    }

    //deactivate all when game is over #TODO: add highscore setting and all that
    public override void GameEnd() {
        float timeTaken = startTime + timeLimit - Time.time;
        if (timeTaken > timeLimit) {
            //Failure condition
            print("Too Slow :(");
        } else {
            //Victory Condition
            print("Finished in time!");
            //if a new high score has been set, update the list of high scores by replacing the slowest time
            if (highscores.Count < maxHighScores) {
                highscores.Add(timeTaken);
                highscores.Sort();
            } else if (highscores.Count > 0 && timeTaken < highscores[maxHighScores -1]) {
                highscores[maxHighScores -1] = timeTaken;
                highscores.Sort();
            }
        }
        base.GameEnd();
        //disable objectives
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].SetActive(false);
        }
    }

    //called when an objective (bubble) is reached
    public override void ObjectiveReached(int index) {
        objectives[score].SetActive(false);
        score++;
        //if the last objective is reached, set the lap counter up by one
        if (score > objectives.Length) {
            score = 0;
            lapsDone++;
            //then if the correct number of laps are compleated, end the game, otherwise activate the next objective
            if (lapsDone > lapsNeeded) {
                GameEnd();
            } else {
                objectives[index].SetActive(true);
            }
        }
    }


    // Use this for initialization
    public override void Start() {
        //Do base start. Find player character, etc
        base.Start();
        //Disable objectives to start
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].GetComponent<Objective>().SetUp(this, i);
            objectives[i].SetActive(false);
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Player") && !isPlaying) {
            GameStart();
        }
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
    }
}
