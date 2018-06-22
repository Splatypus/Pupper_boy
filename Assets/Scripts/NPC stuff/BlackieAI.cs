using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public bool hasDoneReward = false;

    public BlackieMiniGame2 blackieGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();	
	}

    //icons to display if in range of player
    public override void OnInRange() {
        //Display(Icons[0]);
    }

    //loads the puzzle indicated into the puzzle machine
    public void SetUpPuzzleMachine(int puzzleNum) {
        blackieGameRef.puzzleNumber = puzzleNum;
        blackieGameRef.SetUpMachine();
    }

    //spawns reward
    public void SpawnReward() {
        if (!hasDoneReward) {
            hasDoneReward = true;
            Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
        }
    }

    //called by game machine when a game ends
    public void FinishedGame() {
        progressionNum = 1;
    }    
}
