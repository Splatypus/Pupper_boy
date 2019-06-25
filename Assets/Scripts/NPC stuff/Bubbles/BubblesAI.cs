using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesAI : AIbase {
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "BubblesSummerProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "BubblesSummerPN"; } }
    readonly string STATE_KEY = "BubblesSummerState";

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

    public override void LoadDialogProgress() {
        base.LoadDialogProgress();
        state = (States)SaveManager.getInstance().GetInt(STATE_KEY, 0);
    }

    public override void SaveDialogProgress() {
        SaveManager.getInstance().PutInt(STATE_KEY, (int)state);
        base.SaveDialogProgress();
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            GameObject toy = player.GetComponentInChildren<PuppyPickup>().itemInMouth;
            if (toy != null && toy.GetComponent<BasicToy>().HasTag(BasicToy.Tag.Soap) && state == States.NOBUBBLES) {
                DestoryObjectInMouth(toy.gameObject);
                state = States.BUBBLES;
                Display(1);
                ChangeAndSaveProgressionNum(1);
            }
        }
    }

    //if soap is brought in range, destory it and progress quest
    public override void ToyInRange(BasicToy toy) {
        base.ToyInRange(toy);
        if (toy.HasTag(BasicToy.Tag.Soap)) {
            DestoryObjectInMouth(toy.gameObject);
            if (state == States.NOBUBBLES) {
                state = States.BUBBLES;
                Display(1);
                ChangeAndSaveProgressionNum(1);
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
