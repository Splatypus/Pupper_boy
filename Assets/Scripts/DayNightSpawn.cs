using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//spawns an object when it hits day or night. Removes it during the other one
public class DayNightSpawn : MonoBehaviour {

    public bool isActiveAtNight = false;

    // Use this for initialization
    void Start() {
        DayNightManager dayNight = DayNightManager.Instance;
        //if its active at night, add activate to the night triggers and deactivate to day triggers. Otherwise do the reverse.
        if (isActiveAtNight) {
            dayNight.AddTrigger(DayNightManager.Times.NIGHT, new UnityAction(Activate));
            dayNight.AddTrigger(DayNightManager.Times.DAY, new UnityAction(Deactivate));
        } else {
            dayNight.AddTrigger(DayNightManager.Times.DAY, new UnityAction(Activate));
            dayNight.AddTrigger(DayNightManager.Times.NIGHT, new UnityAction(Deactivate));
        }
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
