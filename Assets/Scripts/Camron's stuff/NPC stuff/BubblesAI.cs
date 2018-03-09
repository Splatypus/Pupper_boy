using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesAI : AIbase {


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;

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
    public override void ToyInRange() {
        base.ToyInRange();
        if (questNumber == 0) {
            NextQuest();
        }
    }

    //when dialog ends
    public override void OnEndOfDialog(int c) {
        base.OnEndOfDialog(c);
        if (c == 1 || c == 2 || c == 4) {
            bubbleGameRef.GameStart();
        } else if (c == 3) {
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
        }
    }    
}
