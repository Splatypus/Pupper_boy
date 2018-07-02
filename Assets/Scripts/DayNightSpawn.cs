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
            dayNight.nightTrigger.AddEvent(new UnityAction(Activate));
            dayNight.dayTrigger.AddEvent(new UnityAction(Deactivate));
        } else {
            dayNight.dayTrigger.AddEvent(new UnityAction(Activate));
            dayNight.nightTrigger.AddEvent(new UnityAction(Deactivate));
        }
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
