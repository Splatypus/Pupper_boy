using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbase : Dialog2 {
    
    public Sprite[] Icons;
    [HideInInspector]public GameObject Player;
    public SpriteRenderer iconRenderer;
    public GameObject iconCanvas;

    // Use this for initialization
    public override void Start(){
        base.Start();
        Player = GameObject.FindGameObjectWithTag("Player");
        EndDisplay();
    }

    //override on interact to hide the currently displayed icon
    public override void OnInteract() {
        EndDisplay();
        base.OnInteract();
    }

    public override void OnEnd() {
        base.OnEnd();
        //since the player is in range on ending dialog, trigger this again
        OnInRange(Player);
    }

    //Triggers when the player enters the range
    public virtual void OnInRange(GameObject player) {
        Display(0);
    }
    //Trigger when the player leaves range
    public virtual void OnExitRange() {
        EndDisplay();
    }

    //Displays the thought bubble
    public void Display(int iconIndex) {
        if (Icons.Length != 0 && iconIndex < Icons.Length) {
            iconCanvas.SetActive(true);
            iconRenderer.sprite = Icons[iconIndex];
        }
    }
    //hides the icon
    public void EndDisplay() {
        if (iconCanvas != null)
            iconCanvas.SetActive(false);
    }

    public GameObject GetCarriedItem() {
        return GetCarriedItem(Player);
    }
    public GameObject GetCarriedItem(GameObject player) {
        if (player != null) {
            //return it
            return player.GetComponentInChildren<PuppyPickup>().itemInMouth;
        }
        return null;
    }
    //checks the item in the player's mouth and then calls react to item on it
    public void InspectPlayerItem() {
        //find the item in the player's mouth
        GameObject toyItem = Player.GetComponentInChildren<PuppyPickup>().itemInMouth;
        //if its a toy, give the AI a chance to do something with it
        BasicToy toy = toyItem?.GetComponent<BasicToy>();
        ReactToItem(toy);
    }
    //called when a check for a carried item is called
    public virtual void ReactToItem(BasicToy toy) {}
    //destroys a gameobject and removes it from the player's mouth if its being held. Note that even if the object is not held, this function deltes it anyway
    public void DestoryObjectInMouth(GameObject toDestroy) {
        PuppyPickup inMouth = Player.GetComponent<DogController>().mouth;
        if (inMouth.itemInMouth != null && inMouth.itemInMouth == toDestroy) {
            inMouth.DropItem();
        }
        Destroy(toDestroy);
    }
    //destroys the item currently being carried by doggo
    public void ConsumeCarriedItem() {
        DestoryObjectInMouth(GetCarriedItem());
    }

    //generates a random reply by setting progression num
    public void SetRandomProgressionNum(int max) {
        progressionNum = Random.Range(0, max);
    }

    public override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);
        //if this is the player, call OnInRange, if this is a toy, call ToyInRange
        if (col.gameObject.CompareTag("Player")) {
            OnInRange(col.gameObject);
        }
    }

    public override void OnTriggerExit(Collider col){
        base.OnTriggerExit(col);
        if (col.gameObject.CompareTag("Player")) {
            OnExitRange();
        }
    }
}
