using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager {

    public static EventManager instance;
    public static EventManager Instance {
        get {
            if (instance == null) {
                instance = new EventManager();
            }
            return instance;
        }
    }

    #region player trigger list
    //Use EventName += FunctionName to subscribe a function to an event 
    //-= will unsubscribe a function. Not unsubscribing will cause memory leaks!
    //Simply invoke an event when it should trigger by calling it as if it were a function

    //NPC interact (triggers on any interaction either fence or NPC or other)
    public delegate void TalkToNPCAction(GameObject npc);
    public static event TalkToNPCAction OnTalk;
    public void TriggerOnTalk(GameObject npc) { OnTalk?.Invoke(npc); }

    //Picking and droping items
    public delegate void ItemPickupAction(GameObject item);
    public static event ItemPickupAction OnItemPickup;
    public void TriggerOnItemPickup(GameObject item) { OnItemPickup?.Invoke(item); }

    public delegate void ItemDropAction(GameObject item);
    public static event ItemDropAction OnItemDrop;
    public void TriggerOnItemDrop(GameObject item) { OnItemDrop?.Invoke(item); }

    //barking
    public delegate void BarkAction(GameObject player);
    public static event BarkAction OnBark;
    public void TriggerOnBark(GameObject player) { OnBark?.Invoke(player); }

    //Digging Under a fence (triggers after the dig)
    public delegate void FenceDigAction(GameObject enteringYard);
    public static event FenceDigAction OnFenceDig;
    public void TriggerOnFenceDig(GameObject enteringYard) { OnFenceDig?.Invoke(enteringYard); }
    #endregion

    #region seasonal, daynight, and weather triggers
    public delegate void SwapToNightAction();
    public static event SwapToNightAction OnNight;
    public void TriggerOnNight() { OnNight?.Invoke(); }

    public delegate void SwapToDayAction();
    public static event SwapToDayAction OnDay;
    public void TriggerOnDay() { OnDay?.Invoke(); }

    //season change call
    public delegate void SeasonChangedAction(SeasonManager.Seasons newSeason);
    public static event SeasonChangedAction OnSeasonChange;
    public void TriggerSeasonChange(SeasonManager.Seasons newSeason) { OnSeasonChange?.Invoke(newSeason); }
    #endregion
}
