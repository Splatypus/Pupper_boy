using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightManager : MonoBehaviour {

    public static DayNightManager Instance; //Static reference to the current instance of this manager
    public enum Times { DAY, NIGHT };
    Times currentTime = Times.DAY;

    //gets
    public Times GetTime() {
        return currentTime;
    }
    public bool IsDay() {
        return currentTime == Times.DAY;
    }

    //singleton pattern but for gameobjects.
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    //turns time to the given time if it isnt already. 
    public void SetTime(Times t) {
        if (currentTime == t) {
            return;
        }
        currentTime = t;
        //then run triggers if we changed the time
        if (t == Times.DAY) {
            EventManager.Instance.TriggerOnDay();
        } else if (t == Times.NIGHT) {
            EventManager.Instance.TriggerOnNight();
        }
    }

    //changes from night to day or day to night
    public void SwapTime() {
        if (IsDay()) {
            SetTime(Times.NIGHT);
        } else {
            SetTime(Times.DAY);
        }
    }
}
