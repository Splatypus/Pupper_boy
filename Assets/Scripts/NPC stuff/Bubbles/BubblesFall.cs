using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesFall : AIbase
{
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "BubblesFallProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "BubblesFallPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "BubblesFallState"; } }
    const int START = 0;
    const int FINISHED = 1;

    public FallBubbleGameManager gameManager;
    public FallManager manager;

    public override void Start()
    {
        base.Start();
        gameManager.Init(OnGameEnd);

        //if done, notify manager
        if (characterState == FINISHED) {
            manager.bubblesDone = true;
        }
    }

    public void OnGameEnd(bool didWin) {
        //0 is still going
        //1 is fail
        //2 is win
        //3 is win as last dog
        if (didWin)
        {
            manager.bubblesDone = true;
            characterState = FINISHED;
            if (manager.areAllDone()) {
                progressionNum = 3;
            } else {
                progressionNum = 2;
            }
        }
        else {
            progressionNum = 1;
        }
    }
}
