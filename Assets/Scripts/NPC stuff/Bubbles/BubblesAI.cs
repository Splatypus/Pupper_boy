using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesAI : AIbase {
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "BubblesSummerProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "BubblesSummerPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "BubblesSummerState"; } }
    const int NOBUBBLES = 0;
    const int BUBBLES = 1;
    const int END = 2;


    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public GameObject reward2;

    public BubbleGameManager bubbleGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();	
	}

    //swap icon depending on state when player is in range
    public override void OnInRange(GameObject player) {
        base.OnInRange(player);
        switch (characterState) {
            case NOBUBBLES:
                Display(0);
                break;
            case BUBBLES:
                Display(1);
                break;
            default:
                break;
        }
    }

    //if soap is brought in range, destory it and progress quest
    public override void ToyInRange(BasicToy toy) {
        base.ToyInRange(toy);
        if (toy.HasTag(BasicToy.Tag.Soap) && characterState == NOBUBBLES) {
            progressionNum = 1;
        }
    }
    public void AfterSoapTaken() {
        characterState = BUBBLES;
        Display(1);
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
            ChangeAndSaveProgressionNum(1);
        } else {
            ChangeAndSaveProgressionNum(0);
        }
    }

    //unlocks fences to designated area
    public void UnlockFences(int y) {
        FenceUnlockManager.Instance.EnableIntoYard(y);
        FenceUnlockManager.Instance.EnableAll();
    }
}
