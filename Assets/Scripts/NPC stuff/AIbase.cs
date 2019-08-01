﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIbase : Dialog2 {
    
    public Sprite[] Icons;
    public GameObject Player;
    public Image iconRenderer;
    public GameObject iconCanvas;

    // Use this for initialization
    new public void Start(){
        base.Start();
        Player = GameObject.FindGameObjectWithTag("Player");
        EndDisplay();
    }

    public override void OnEnd() {
        base.OnEnd();
        //since the player is in range on ending dialog, trigger this again
        OnInRange(Player);
    }

    //Triggers when the player enters the range
    public virtual void OnInRange(GameObject player) {
        BasicToy toy = player.GetComponent<DogController>().mouth.itemInMouth?.GetComponent<BasicToy>();
        Display(0);
        if (toy != null) {
            ToyInRange(toy);
        }
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

    //called when something with the toy interaction script on it is brought within range
    public virtual void ToyInRange(BasicToy toy) {
        
    }

    public GameObject GetCarriedItem() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return GetCarriedItem(player);
    }
    public GameObject GetCarriedItem(GameObject player) {
        if (player != null) {
            return player.GetComponentInChildren<PuppyPickup>().itemInMouth;
        }
        return null;
    }
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
