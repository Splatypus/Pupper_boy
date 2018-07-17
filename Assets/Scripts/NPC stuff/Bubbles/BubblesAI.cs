using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public GameObject reward2;

    public enum States {NOBUBBLES, BUBBLES, END};
    public States state = States.NOBUBBLES;

    public BubbleGameManager bubbleGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();	
	}

    //swap icon depending on state when player is in range
    public override void OnInRange() {
        switch (state) {
            case States.NOBUBBLES:
                Display(0);
                break;
            case States.BUBBLES:
                Display(1);
                break;
            default:
                break;
        }
    }

    //if soap is brought in range, destory it and progress quest
    public override void ToyInRange(Interactable toy) {
        base.ToyInRange(toy);
        if (toy.hasTag(Interactable.Tag.Soap)) {
            DestoryObjectInMouth(toy.gameObject);
            if (state == States.NOBUBBLES) {
                state = States.BUBBLES;
                Display(1);
                progressionNum = 1;
            }
        }
    }

    //methods to spawn rewards. Called from dialog editor graph thing
    public void FirstReward() {
        Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }

    public void SecondReward() {
        Instantiate(reward2, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }
    //function to start the game.
    public void StartGame(int scoreToWin) {
        bubbleGameRef.GameStartForReward(scoreToWin);
    }

    //called by bubble machine when the game is finished
    public void FinishedGame(bool didWin) {
        if (didWin) {
            progressionNum = 1;
        } else {
            progressionNum = 0;
        }
    }    
}
