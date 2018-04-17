using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public GameObject reward2;

    public BubbleGameManager bubbleGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();	
	}

    //icons to display if in range of player
    public override void OnInRange() {
        //Display(Icons[0]);
    }

    //when bringing soap to bubbles
    public override void ToyInRange(GameObject toy) {
        
        if (questNumber == 0) {
            base.ToyInRange(toy);
            NextQuest();
        }
    }

    //when dialog ends
    public override void OnEndOfDialog(int c) {
        base.OnEndOfDialog(c);
        if (c == 1 || c == 2 || c == 6) {
            bubbleGameRef.GameStartForReward(5);
        } else if (c == 4) {
            bubbleGameRef.GameStartForReward(8);
        } else if (c == 3 || c == 5) {
            if (c==3)
                Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
            else
                Instantiate(reward2, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
            NextQuest();
        }

    }

    //called by bubble machine when the game is finished
    public void FinishedGame(bool didWin) {
        if (questNumber == 1 || questNumber == 2) {
            if (didWin) {//change to winning conversation
                questNumber = 3;
                SetConversationNumber(3);
            } else {//change to losing conversation
                questNumber = 2;
                SetConversationNumber(2);
            }
        } else if (questNumber == 4 && didWin) {
            //beat the new high score
            NextQuest();
        }
    }    
}
