using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RexAI : AIbase {

    public enum States {START, READYFORTOYS, HAPPY};
    public States state = States.START;

    public int totalToys = 5;
    int toysCollected = 0;

    new public void Start() {
        base.Start();
        DayNightManager dnMan = DayNightManager.Instance;
        dnMan.AddTrigger(DayNightManager.Times.DAY, SetProgressionNum);
        dnMan.AddTrigger(DayNightManager.Times.NIGHT, SetProgressionNum);
    }

    //icons to display if in range of player
    public override void OnInRange() {
        //happy when all toys are collected. Grumpy during the day, sad at night
        if (toysCollected >= totalToys) {
            Display(2);
        } else if (DayNightManager.Instance.currentTime == DayNightManager.Times.DAY) {
            Display(0);
        } else {
            Display(1);
        }
    }

    //if you bring rex his toys, incriment his toy count and delete the toy
    public override void ToyInRange(Interactable toy) {
        base.ToyInRange(toy);
        if (toy.hasTag(Interactable.Tag.RexQuestItem) && state == States.READYFORTOYS) {
            toysCollected++;
            if (toysCollected >= totalToys)
                state = States.HAPPY;
            DestoryObjectInMouth(toy.gameObject);
        }
    }

    #region dialogFunctions
    //unllocks scent mode
    public void UnlockScentMode() {
        ScentManager.Instance.isUnlocked = true;
        state = States.READYFORTOYS;
    }

    //sets progression number to 0 if its day, 1 if its night
    public void SetProgressionNum() {
        if (DayNightManager.Instance.currentTime == DayNightManager.Times.DAY) {
            progressionNum = 0;
        } else if (toysCollected >= totalToys) {
            progressionNum = 3;
        } else {
            progressionNum = 1;
        }
    }
    #endregion
}