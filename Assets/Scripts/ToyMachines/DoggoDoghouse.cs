using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoDoghouse : AIbase {

    public override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        OnInteract();
    }

    public override void OnInRange(GameObject player) {
        base.OnInRange(player);
        Display(0);
    }

    public void ChangeTime() {
        DayNightManager.Instance.SwapTime();
    }
}
