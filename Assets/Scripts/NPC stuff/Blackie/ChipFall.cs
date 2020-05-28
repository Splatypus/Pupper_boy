using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipFall : AIbase
{
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "ChipFallProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "ChipFallPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "ChipFallState"; } }
    const int START = 0;
    const int FINISHED = 1;

    public BlackieGameViewController game;
    public FallManager manager;

    bool isGameActive = false;

    public override void Start()
    {
        base.Start();
        if (characterState == FINISHED) {
            manager.chipDone = true;
        }
    }

    public void Finish()
    {
        //tell fall manager that chip's section is finished
        manager.chipDone = true;
        //Set progression num, 2 if chip was the last dog, 1 otherwise
        progressionNum = manager.areAllDone() ? 2 : 1;
        //set character state to save progress
        characterState = FINISHED;
    }

    //load a puzzle
    public void StartGame(int index) {
        //prevent reloading a board if its already up
        if (isGameActive)
            return;

        //load board with callback to start dialog
        isGameActive = true;
        game.LoadFile(index, ()=> {
            isGameActive = false;
            progressionNum = 1;
            OnInteract();
        });
    }
}
