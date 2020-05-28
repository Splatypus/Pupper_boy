using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyFall : AIbase
{
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "TiffanyFallProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "TiffanyFallPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "TiffanyFallState"; } }
    const int START = 0;
    const int FINISHED = 1;

    public FallManager manager;
    int totalItems;
    public GameObject[] items;
    int itemsCollected = 0;

    public override void Start()
    {
        base.Start();
        totalItems = items.Length;
        if (characterState == FINISHED)
        {

            //set initial manager state to reflect it
            manager.tiffanyDone = true;
            //remove collectable items
            foreach (GameObject i in items)
            {
                i.SetActive(false);
            }
        }
    }

    //if you bring tiffany items, change progression num
    public override void ReactToItem(BasicToy toy)
    {
        base.ReactToItem(toy);
        if (toy != null && toy.HasTag(BasicToy.Tag.TiffyQuestItem))
        {
            progressionNum = 1;
        }
    }

    public void AfterToyTaken()
    {
        itemsCollected++;
        if (itemsCollected >= totalItems)
        {
            characterState = FINISHED;
            progressionNum = 1;
            manager.tiffanyDone = true;

            //if last dog, set to 2 instead
            if (manager.areAllDone()) {
                progressionNum = 2;
            }
        }
        else
        {
            progressionNum = 0;
        }
    }
}
