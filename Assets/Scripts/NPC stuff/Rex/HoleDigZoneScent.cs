using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDigZoneScent : ScentObject {

    public HoleDigZone digzone;

    public override void StartScent() {
        base.StartScent();
        digzone.OnScent();
    }

    public override void EndScent() {
        base.EndScent();
        digzone.OnScentEnd();
    }
}
