using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {


    public static EventManager Instance;

    #region player trigger list
    //Use EventName += FunctionName to subscribe a function to an event (-= to remove. please dont cause memeory leaks)
    //Simply invoke an event when it should trigger by calling it as if it were a function

    //NPC interact (triggers on any interaction either fence or NPC or other)
    public delegate void TalkToNPCAction(GameObject npc);
    public static event TalkToNPCAction OnTalk;
    public void TriggerOnTalk(GameObject npc) { if (OnTalk != null) OnTalk(npc); }

    //Picking and droping items
    public delegate void ItemPickupAction(GameObject item);
    public static event ItemPickupAction OnItemPickup;
    public void TriggerOnItemPickup(GameObject item) { if (OnItemDrop != null) OnItemPickup(item); }

    public delegate void ItemDropAction(GameObject item);
    public static event ItemDropAction OnItemDrop;
    public void TriggerOnItemDrop(GameObject item) {if (OnItemDrop != null) OnItemDrop(item);  }

    //barking
    public delegate void BarkAction(GameObject player);
    public static event BarkAction OnBark;
    public void TriggerOnBark(GameObject player) { if (OnBark != null) OnBark(player); }

    //Digging Under a fence (triggers after the dig)
    public delegate void FenceDigAction(GameObject enteringYard);
    public static event FenceDigAction OnFenceDig;
    public void TriggerOnFenceDig(GameObject enteringYard) { if (OnFenceDig != null) OnFenceDig(enteringYard); }
    #endregion

    #region seasonal, daynight, and weather triggers
    
    #endregion

    void Awake() {
        //singleton pattern but for gameobjects.
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }



}
