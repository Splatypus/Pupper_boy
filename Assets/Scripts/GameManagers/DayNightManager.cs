using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightManager : MonoBehaviour {

    public static DayNightManager Instance; //Static reference to the current instance of this manager
    public enum Times { DAY, NIGHT };
    int numberOfTimes = 2;
    public Times currentTime = Times.DAY;
    public List<Trigger> triggers;

    void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
            SetUp();
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    //holds daynight actions and the number of times to repeat them
    public class ActionContainter {
        public UnityAction action;
        public bool limitedTriggers;
        public int triggersRemaining;

        //performs this specific action. Reduces the trigger count if its used.
        //Returns true if this should remain active, false if it should be deleted.
        public bool DoTrigger() {
            action.Invoke();
            if (limitedTriggers) {
                triggersRemaining--;
                return triggersRemaining <= 0;
            }
            return true;
        }

    }

    //Each trigger instance is a set event point, such as night or day starting.
    public class Trigger {
        List<ActionContainter> ActionList;

        //adds an event to the list of actions at this trigger
        public ActionContainter AddEvent(UnityAction function) {
            if (ActionList == null)
                ActionList = new List<ActionContainter>();
            ActionContainter ac = new ActionContainter { action = function, limitedTriggers = false, triggersRemaining = 0 };
            ActionList.Add(ac);
            return ac;
        }
        public ActionContainter AddEvent(UnityAction function, int triggers) {
            if (ActionList == null)
                ActionList = new List<ActionContainter>();
            ActionContainter ac = new ActionContainter { action = function, limitedTriggers = true, triggersRemaining = triggers };
            ActionList.Add(ac);
            return ac;
        }

        //removes an event form the list of triggers
        public void RemoveEvent(ActionContainter a) {
            ActionList.Remove(a);
        }

        //call this when this should trigger
        public void DoTrigger() {
            //resolve each action, remove it from the list if it runs out of triggers.
            List<ActionContainter> expired = new List<ActionContainter>();
            for (int i = 0; i < ActionList.Count; i++) {
                if (ActionList[i].DoTrigger()) {
                    expired.Add(ActionList[i]);
                }
            }
            //then remove ones that have expired (ya this isnt fast, but its very rare to be removing a lot of stuff at once.
            foreach (ActionContainter a in expired) {
                ActionList.Remove(a);
            }
        }

    }

    //when this is first made, do all the data setup thats needed
    void SetUp() {
        triggers = new List<Trigger>();
        for (int i = 0; i < numberOfTimes; i++) {
            triggers.Add(new Trigger());
        }
    }

    //turns time to the given time if it isnt already. 
    public void SetTime(Times t) {
        if (currentTime == t) {
            return;
        }
        currentTime = t;
        //then do the corrisponding trigger. 
        triggers[(int)t].DoTrigger();

    }

    //adds an event at the designated time
    public void AddTrigger(Times t, UnityAction action) {
        triggers[(int)t].AddEvent(action);
    }
    public void AddTrigger(Times t, UnityAction a, int repititions) {
        triggers[(int)t].AddEvent(a, repititions);
    }
}
