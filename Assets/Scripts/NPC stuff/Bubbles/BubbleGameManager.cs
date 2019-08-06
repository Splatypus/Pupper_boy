using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleGameManager : MonoBehaviour {//: MiniGameManager {


    [Header("Setup")]
    public Text canvasTimeField; //reference to the text box that displays time. Must be set
    public Text scoreText;
    public Objective[] objectives;
    public int numberOfActiveObjectives;
    public GameObject bubble_particle_system;
    public BubblesAI bubblesRef;

    [Header("Inital Constraints")]
    public float timeLimit;
    public int rewardScore;

    //private
    bool isPlaying = false;
    List<int> activeObjectives;
    int score = 0;
    float startTime;

    

    // Use this for initialization
    public void Start() {
        //set references on start
        canvasTimeField.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        //Hide bubble particle system
        bubble_particle_system.SetActive(false);
        //Disable objectives to start
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].SetUp(this, i);
            objectives[i].SetIsVisible(false);
        }
        //set up objective list
        activeObjectives = new List<int>(numberOfActiveObjectives);
        for (int i = 0; i < numberOfActiveObjectives; i++) {
            activeObjectives.Add(0);
        }
    }

    public void OnDestroy() {
        EventManager.OnTalk -= ClearUI;
        EventManager.OnFenceDig -= ClearUI;
    }

    // Update is called once per frame
    public void Update() {
        if (isPlaying) {
            if (startTime + timeLimit < Time.time) {
                GameEnd();
            } else {
                //canvasTimeField.GetComponent<Text>().text = (startTime + timeLimit - Time.time).ToString();
                canvasTimeField.text = "Time: " + ((int)(startTime + timeLimit - Time.time)).ToString();
            }
        }
    }

    //called when the minigame is started
    public void GameStart() {
        if (!isPlaying) {
            startTime = Time.time;
            isPlaying = true;
            canvasTimeField.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            scoreText.text = "Score: 0";
            bubble_particle_system.SetActive(true);
            score = 0;
            //activate objectives
            for (int i = 0; i < numberOfActiveObjectives; i++) {
                activeObjectives[i] = SelectObjective();
                objectives[activeObjectives[i]].SetIsVisible(true);
            }
        }
    }

    public void GameStartForReward(int scoreGoal) {

        rewardScore = scoreGoal;
        GameStart();
    }

    //deactivate all when game is over #TODO: add highscore setting and all that
    public void GameEnd() {
        //end event, add scores n stuff
        isPlaying = false;
        //clear particle system
        bubble_particle_system.SetActive(false);
        //disable objectives
        for (int i = 0; i < objectives.Length; i++) {
            objectives[i].SetIsVisible(false);
        }
        bubblesRef.FinishedGame(score >= rewardScore);

        //wait to clear the UI until you talk to an npc or you dig out
        EventManager.OnTalk += ClearUI;
        EventManager.OnFenceDig += ClearUI;
    }


    //hides time and score UI
    public void ClearUI(GameObject npc) {
        ClearUI();
    }
    public void ClearUI() {
        //clear time and score
        canvasTimeField.text = "";
        canvasTimeField.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        scoreText.text = "";
    }

    //called when an objective (bubble) is reached
    public void ObjectiveReached(int index) {
        score++;
        scoreText.text = "Score: " + score;
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
        //bubble_particle_system.transform.forward = objectives[i].transform.position - transform.position;
        transform.forward = objectives[i].transform.position - transform.position;
        return i;
    }

    //deactivates the objective index and selects a new one to activate
    public void NewObjective(int index) {
        int i = SelectObjective();
        objectives[i].SetIsVisible(true);
        //change the activeObjectives array to reflect that i is now active, rather than whatever we had before
        for (int j = 0; j < numberOfActiveObjectives; j++) {
            if (activeObjectives[j] == index) {
                objectives[activeObjectives[j]].SetIsVisible(false);
                activeObjectives[j] = i;
            }
        }
    }


    void OnTriggerEnter(Collider col) {
        /*if (col.gameObject.CompareTag("Player") && !isPlaying) {
            GameStart();
        }*/
    }
}
