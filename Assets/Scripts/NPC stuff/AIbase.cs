using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbase : Dialog {

    public GameObject[] Icons;
    private GameObject Player;
    public Interactable.Tag ToyTag;

    public int questNumber = 0;

    private bool inRange = false;
    private GameObject activeIcon;
    

    //easy way to incriment quest numebr and conversation number at once
    public void NextQuest() {
        questNumber++;
        SetConversationNumber();
    }

    //Triggers when the player enters the range
    public virtual void OnInRange() {
        
    }
    //Trigger when the player leaves range
    public virtual void OnExitRange() {
        EndDisplay();
    }

    public virtual void ToyInRange(GameObject toy) {
        EndDisplay();
        PuppyPickup inMouth = Player.GetComponent<DogControllerV2>().ppickup;
        //if the player is holding the object drop it first so everything doesnt break. 
        if (inMouth.itemInMouth != null && inMouth.itemInMouth == toy) {
            inMouth.DropItem();
            inMouth.objectsInRange.Remove(toy);
        }
        Destroy(toy);
    }

    //Calling OnInteract will trigger if the player is in range and barks. This is defined in the parent class.

    //Displays the thought bubble
    public void Display(GameObject icon) {
        EndDisplay();
        icon.SetActive(true);
        activeIcon = icon;
    }
    //hides the icon
    public void EndDisplay() {
        if (activeIcon != null) {
            activeIcon.SetActive(false);
        }
    }


    new void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);
        if (col.gameObject.CompareTag("Player")) {
            inRange = true;
            OnInRange();
        } else if (col.gameObject.GetComponent<Interactable>() != null && col.gameObject.GetComponent<Interactable>().hasTag(ToyTag)) {
            ToyInRange(col.gameObject);
            
        }
    }

    new void OnTriggerExit(Collider col) {
        base.OnTriggerExit(col);
        if (col.gameObject.CompareTag("Player")) {
            inRange = false;
            OnExitRange();
        }
    }

    // Use this for initialization
    new public void Start() {
        base.Start();
        foreach (GameObject i in Icons) {
            i.SetActive(false);
        }
        Player = GameObject.FindGameObjectWithTag("Player");
    }

}
