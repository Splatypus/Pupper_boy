using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RexFall : AIbase
{
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "RexFallProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "RexFallPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "RexFallState"; } }
    const int START = 0;
    const int FIRST_ITEM_FOUND = 1;
    const int SECOND_ITEM_FOUND = 2;
    const int FINISHED = 3; //Finished when third item is found

    public FallManager manager;
    public GameObject[] items;

    public override void Start()
    {
        base.Start();
        foreach (GameObject i in items) {
            i.SetActive(false);
        }
    }

    //if you bring tiffany items, change progression num
    public override void ReactToItem(BasicToy toy)
    {
        base.ReactToItem(toy);
        if (toy != null && toy.HasTag(BasicToy.Tag.RexQuestItem))
        {
            progressionNum = 1;
            characterState++;
        }
    }

    //called after we remove a toy from the player's possession
    public void AfterToyTaken()
    {
       
    }

    //spawns the next unfound item based on character state
    public void SpawnNextItem() {
        items[characterState].SetActive(true);
    }

}
