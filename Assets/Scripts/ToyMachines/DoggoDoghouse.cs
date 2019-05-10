using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoDoghouse : AIbase {

    public override void OnInRange() {
        base.OnInRange();
        Display(0);
    }

    public void ChangeTime() {
        DayNightManager.Instance.SwapTime();
    }
}
