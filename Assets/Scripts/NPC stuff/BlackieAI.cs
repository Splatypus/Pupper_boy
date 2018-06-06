using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public bool hasDoneReward = false;

    public BlackieMiniGame blackieGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();	
	}

    //icons to display if in range of player
    public override void OnInRange() {
        //Display(Icons[0]);
    }

    //when bringing items do nothing because fuck you no items why was I dumb enough to put this in the parent class
    public override void ToyInRange(GameObject toy) {
        
    }

    //when dialog ends
    public override void OnEndOfDialog(int c) {
        base.OnEndOfDialog(c);
        switch (c) {
            case 1:
            case 2:
            case 3:
                blackieGameRef.puzzleNumber = c;
                if (blackieGameRef.conversationNumber == 0 || blackieGameRef.conversationNumber == 1)
                    blackieGameRef.conversationNumber = 2;
                break;
            case 4:
                if (!hasDoneReward){
                    hasDoneReward = true;
                    Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
                }
                break;
            default:
                break;
        }
    }

    public override void OnChoiceMade(int choice){
        base.OnChoiceMade(choice);
        if (conversationNumber == 0 && choice == 0){
            blackieGameRef.puzzleNumber = 0;
            if(blackieGameRef.conversationNumber == 0 || blackieGameRef.conversationNumber == 1)
                blackieGameRef.conversationNumber = 2;
        }

    }

    //called by game machine when a game ends
    public void FinishedGame() {
        if (conversationNumber < 4) {
            conversationNumber++;
        }
    }    
}
