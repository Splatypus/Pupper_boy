using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbase : Dialog2 {

    public Sprite[] Icons;
    public GameObject Player;
    public SpriteRenderer iconRenderer;
    public GameObject iconCanvas;

    // Use this for initialization
    new public void Start(){
        base.Start();
        Player = GameObject.FindGameObjectWithTag("Player");
        EndDisplay();
    }

    //Triggers when the player enters the range
    public virtual void OnInRange() {
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
    public virtual void ToyInRange(Interactable toy) {
        
    }

    //destroys a gameobject and removes it from the player's mouth if its being held. Note that even if the object is not help, this function deltes it anyway
    public void DestoryObjectInMouth(GameObject toDestroy) {
        PuppyPickup inMouth = Player.GetComponent<DogController>().ppickup;
        if (inMouth.itemInMouth != null && inMouth.itemInMouth == toDestroy) {
            inMouth.DropItem();
            inMouth.objectsInRange.Remove(toDestroy);
        }
        Destroy(toDestroy);
    }

    public override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);
        Interactable toyScript = col.GetComponent<Interactable>();
        //if this is the player, call OnInRange, if this is a toy, call ToyInRange
        if (col.gameObject.CompareTag("Player")) {
            OnInRange();
        } else if (toyScript != null) {
            ToyInRange(toyScript);
        }
    }

    public override void OnTriggerExit(Collider col){
        base.OnTriggerExit(col);
        if (col.gameObject.CompareTag("Player")) {
            OnExitRange();
        }
    }
}
