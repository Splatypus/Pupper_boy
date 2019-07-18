using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RexAI : AIbase {
    protected override string DIALOG_PROGRESS_SAVE_KEY { get { return "RexSummerProgression"; } }
    protected override string PROGRESSION_NUM_SAVE_KEY { get { return "RexSummerPN"; } }
    protected override string CHARACTER_STATE_SAVE_KEY { get { return "RexSummerState"; } }
    const int START = 0;
    const int READYFORTOYS = 1;
    const int HAPPY = 2;
    

    public int totalToys = 5;
    int toysCollected = 0;

    public GameObject rewardSpawn;
    public GameObject rewardPrefab;

    new public void Start() {
        base.Start();
        EventManager.OnDay += SetProgressionNum;
        EventManager.OnNight += SetProgressionNum;

        //if we should have spawned the soap, and bubbles has not yet received it.
        if (characterState == HAPPY && SaveManager.getInstance().GetInt("BubblesSummerState", 0) >= BubblesAI.BUBBLES) {
            SpawnReward();
        }

    }

    new public void OnDestroy() {
        base.OnDestroy();
        EventManager.OnDay -= SetProgressionNum;
        EventManager.OnNight -= SetProgressionNum;
    }

    //icons to display if in range of player
    public override void OnInRange(GameObject player) {
        base.OnInRange(player);
        //happy when all toys are collected. Grumpy during the day, sad at night
        if (toysCollected >= totalToys) {
            Display(2);
        } else if (DayNightManager.Instance.IsDay()) {
            Display(0);
        } else {
            Display(1);
        }
    }

    //if you bring rex his toys, incriment his toy count and delete the toy
    public override void ToyInRange(BasicToy toy) {
        base.ToyInRange(toy);
        if (toy.HasTag(BasicToy.Tag.RexQuestItem) && characterState == READYFORTOYS) {
            progressionNum = 2;
        }
    }
    public void AfterToyTaken() {
        toysCollected++;
        if (toysCollected >= totalToys) {
            characterState = HAPPY;
            progressionNum = 3;
        } else {
            progressionNum = 4;
        }
    }

    #region dialogFunctions
    //spawns food bowl at designated location
    public void SpawnReward() {
        Instantiate(rewardPrefab, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }

    //unllocks scent mode
    public void UnlockScentMode() {
        ScentManager.Instance.enabled = true;
        SaveManager.getInstance().PutInt("HasScentMode", 1);
        SaveManager.getInstance().SaveFile();
        characterState = READYFORTOYS;
    }

    //sets progression number to 0 if its day, 1 if its night
    public void SetProgressionNum() {
        //if something else has set this, then we let that take precidence.
        if (progressionNum != 0) {
            return;
        }
        if (DayNightManager.Instance.IsDay()) {
            progressionNum = 0;
        } else if (toysCollected >= totalToys) {
            progressionNum = 3;
        } else if (toysCollected > 0) {
            progressionNum = 4;
        } else {
            progressionNum = 1;
        }
    }
    #endregion
}