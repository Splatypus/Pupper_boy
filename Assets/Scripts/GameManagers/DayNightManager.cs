using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightManager {
    public static string DAY_SAVE_KEY = "dayNight";

    public static DayNightManager instance;
    public static DayNightManager Instance {
        get {
            if (instance == null) {
                instance = new DayNightManager();
            }
            return instance;
        }
    }
    private DayNightManager() {}

    public enum Times { DAY, NIGHT };
    Times currentTime = Times.DAY;

    //loads in data
    public void LoadData() {
        //loading daytime
        int loadedTime = SaveManager.getInstance().GetInt(DAY_SAVE_KEY, 0);
        currentTime = (Times)loadedTime;
        if (loadedTime == 0) {
            EventManager.Instance.TriggerOnDay();
        }
        //loading nighttime
        else {
            EventManager.Instance.TriggerOnNight();
        }
    }

    public static void Reset() {
        instance = null;
    }

    //gets
    public Times GetTime() {
        return currentTime;
    }
    public bool IsDay() {
        return currentTime == Times.DAY;
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
        //set the time change to be saved
        SaveManager.getInstance().PutInt(DAY_SAVE_KEY, (int)t);
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
