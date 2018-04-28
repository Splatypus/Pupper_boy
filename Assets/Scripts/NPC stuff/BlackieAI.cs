using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackieAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;

    public BlackieGameManager blackieGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();	
	}

    //icons to display if in range of player
    public override void OnInRange() {
        //Display(Icons[0]);
    }

    //when bringing soap to bubbles
    /*public override void ToyInRange(GameObject toy) {
        
        if (questNumber == 0) {
            base.ToyInRange(toy);
            NextQuest();
        }
    }*/

    //when dialog ends
    public override void OnEndOfDialog(int c) {
        base.OnEndOfDialog(c);
        if (c == 1 || c == 3) {
            if (c==3)
            {
                Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
            }
            
            NextQuest();
        }
    }

    
    public override void OnChoiceMade(int choice){
        base.OnChoiceMade(choice);
        if (conversationNumber == 0) {
            if (choice == 0)
            {
                blackieGameRef.GameStartForReward(0);
            }
        }
        else if (conversationNumber == 2)
        {
            if (choice == 0)
            {
                blackieGameRef.GameStartForReward(0);
            }
        }
        else
        {
            
        }

    }

    //called by bubble machine when the game is finished
    public void FinishedGame(bool didWin) {
        if (questNumber == 0) {
            if (didWin) {//change to winning conversation
                NextQuest();
            }
        } else if (questNumber == 2 && didWin) {
            //beat the new high score
            NextQuest();
        }
    }    
}
