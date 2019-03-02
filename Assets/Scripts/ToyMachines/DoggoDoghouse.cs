using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoDoghouse : AIbase {

    public override void OnInRange() {
        base.OnInRange();
        Display(0);
    }

    public void ChangeTime() {
        //swap times between day and night
        /* Old version
        DayNightManager manager = DayNightManager.Instance;
        if (manager.currentTime == DayNightManager.Times.DAY) {
            manager.SetTime(DayNightManager.Times.NIGHT);
        } else {
            manager.SetTime(DayNightManager.Times.DAY);
        }*/

        //New version using global triggers
        DayNightManager.Instance.SwapTime();
    }
}
