using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesAI : AIbase {
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "BubblesSummerProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "BubblesSummerPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "BubblesSummerState"; } }
    public const int START = 0;
    public const int FIRST_REWARD_GIVEN = 1;
    public const int BUBBLES = 2;
    public const int END = 3;
    
    public GameObject playerStartPosition;
    public GameObject rewardSpawn; //location at which the reward is spawned
    public GameObject reward;
    public GameObject reward2;

    public BubbleGameManager bubbleGameRef;

    // Use this for initialization
    new public void Start () {
        base.Start();
        if (progressionNum == 2) {
            progressionNum = 0; //if we somehow saved this number while a game was in progress, reset it to 0
        }
        //if bubbles has given the bandana out, but tiffany has not yet taken it, then spawn it
        if (characterState >= FIRST_REWARD_GIVEN && SaveManager.getInstance().GetInt("TiffanySummerState", 0) < 3) {
            FirstReward();
        }
    }

    //swap icon depending on state when player is in range
    public override void OnInRange(GameObject player) {
        base.OnInRange(player);
        switch (characterState) {
            case FIRST_REWARD_GIVEN:
                Display(0);
                break;
            case BUBBLES:
            case START:
                Display(1);
                break;
            default:
                break;
        }
    }

    //if soap is brought in range, destory it and progress quest
    public override void ReactToItem(BasicToy toy) {
        base.ReactToItem(toy);
        if (toy != null && toy.HasTag(BasicToy.Tag.Soap) && characterState == FIRST_REWARD_GIVEN) {
            progressionNum = 1;
        } else {
            progressionNum = 0;
        }
    }
    public void AfterSoapTaken() {
        characterState = BUBBLES;
        Display(1);
    }

    //methods to spawn rewards. Called from dialog editor graph thing
    public void FirstReward() {
        characterState = FIRST_REWARD_GIVEN;
        Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }

    public void SecondReward() {
        Instantiate(reward2, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }
    //fades the screen and moves the player into the right position for the game
    public void PrepareDoggoPosition() {
        ScreenEffects.GetInstance().FadeToBlack(1.0f, () => {
            Player.transform.position = playerStartPosition.transform.position;
            Player.transform.rotation = playerStartPosition.transform.rotation;
            Camera.main.GetComponent<FreeCameraLook>().CenterCamera();
            ScreenEffects.GetInstance().ReverseFade(1.0f);
        });
    }
    //function to start the game.
    public void StartGame(int scoreToWin) {
        bubbleGameRef.GameStartForReward(scoreToWin);
        progressionNum = 2; //indicates game is in progress.
        //unlock camera control when we got to start the game.
        Camera.main.GetComponent<FreeCameraLook>().controlLocked = false;
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
