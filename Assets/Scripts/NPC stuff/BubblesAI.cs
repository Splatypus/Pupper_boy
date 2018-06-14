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

    public override void OnTriggerEnter(Collider col)
    {
        base.OnTriggerEnter(col);

        //if an item is brought to tiffany, and is her quest item, delete it, cand call toyInRange.
        if (col.gameObject.GetComponent<Interactable>().hasTag(Interactable.Tag.TiffyQuestItem)) {
            PuppyPickup inMouth = Player.GetComponent<DogControllerV2>().ppickup;
            if (inMouth.itemInMouth != null && inMouth.itemInMouth == col.gameObject) {
                inMouth.DropItem();
                inMouth.objectsInRange.Remove(col.gameObject);
            }
            ToyInRange(col.gameObject);
            Destroy(col.gameObject);
        }
    }

    public void ToyInRange(GameObject toy){
        if (state == States.NOBUBBLES) {
            state = States.BUBBLES;
            progressionNum = 1;
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
