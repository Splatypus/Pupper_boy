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
            EventManager.OnNight += Activate;
            EventManager.OnDay += Deactivate;
        } else {
            EventManager.OnNight += Deactivate;
            EventManager.OnDay += Activate;
        }
        //then set if it should currently be active or not
        if(dayNight.IsDay() == isActiveAtNight){
            Deactivate();
        }
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
