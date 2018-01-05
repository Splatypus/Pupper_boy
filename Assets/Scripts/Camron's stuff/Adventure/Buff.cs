using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {

    public int duration;

    //TURN START TICK
    public void DoTick() {
        OnTick();
        duration--;
        if (duration < 0) {
            RemoveBuff();
        }
    }

    public virtual void OnTick() {

    }


    //ON REMOVAL (called when buff expires)
    public void RemoveBuff() {
        OnRemove();
    }

    public virtual void OnRemove() {

    }

}
