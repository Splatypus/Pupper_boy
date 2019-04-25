using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RexAI : AIbase {

    public enum States {START, READYFORTOYS, HAPPY};
    public States state = States.START;

    public int totalToys = 5;
    int toysCollected = 0;

    public GameObject rewardSpawn;
    public GameObject rewardPrefab;

    new public void Start() {
        base.Start();
        EventManager.OnDay += SetProgressionNum;
        EventManager.OnNight += SetProgressionNum;
    }

    //icons to display if in range of player
    public override void OnInRange() {
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
        if (toy.HasTag(BasicToy.Tag.RexQuestItem) && state == States.READYFORTOYS) {
            toysCollected++;
            progressionNum = 2;
            if (toysCollected >= totalToys) {
                state = States.HAPPY;
                progressionNum = 3;
            }
            DestoryObjectInMouth(toy.gameObject);
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
        state = States.READYFORTOYS;
    }

    //sets progression number to 0 if its day, 1 if its night
    public void SetProgressionNum() {
        if (DayNightManager.Instance.IsDay()) {
            progressionNum = 0;
        } else if (toysCollected >= totalToys) {
            progressionNum = 3;
        } else {
            progressionNum = 1;
        }
    }
    #endregion
}