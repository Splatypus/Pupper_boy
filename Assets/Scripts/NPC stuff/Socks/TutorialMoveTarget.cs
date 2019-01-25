using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoveTarget : MonoBehaviour {

    public SocksTutorial owner;
    public float deathSequenceGravityMod = -4.0f;
    public bool requiresToy = false;
    public Interactable.Tag requiredTag = Interactable.Tag.SocksQuestItem;

    public List<GameObject> objectsInside;

    // Use this for initialization
    void Start () {
        objectsInside = new List<GameObject>();
	}

    //starts the death of this objective (just a fade out for the particle system) and notifies socks that this point was reached
    public void OnTriggerEnter(Collider other) {
        Interactable toy = other.GetComponent<Interactable>();
        //first check is just for the player reaching it
        if (other.CompareTag("Player")) {
            //if they need to bring a toy, then notify the socks to either tell them to bring the toy, or to drop it inside
            if (requiresToy) {
                //if theyre carrying the toy, tell them to drop it, if not, tell them to go get it
                GameObject carriedItem = other.gameObject.GetComponentInChildren<PuppyPickup>().itemInMouth;
                owner.PlayerEnteredItemZone(carriedItem != null && carriedItem.GetComponent<Interactable>().tagList.Contains(requiredTag));

            //if they don't needa toy, then just inform socks that they've won
            } else {
                StartCoroutine(DeathSequence());
                owner.ObjectiveComplete();
            }
        //otherwise, if the item is a required toy, start watching for the player to drop this item, unless they threw or rolled it in, then just win
        } else if (requiresToy && toy != null && toy.tagList.Contains(requiredTag)) {
            if (toy.isCurrentlyHeld) {
                EventManager.OnItemDrop += ItemWasDropped;
                objectsInside.Add(other.gameObject);
            } else {
                StartCoroutine(DeathSequence());
                owner.ObjectiveComplete();
            }
        }
    }

    public void OnTriggerExit(Collider other) {
        Interactable toy = other.GetComponent<Interactable>();
        //if this is a required toy, then stop watching for the player to drop it
        if (requiresToy && toy != null && toy.tagList.Contains(requiredTag)) {
            EventManager.OnItemDrop -= ItemWasDropped;
            objectsInside.Remove(other.gameObject);
        }
    }

    //disables emission, waits for the last to despawn, then destroys the gameobject
    IEnumerator DeathSequence() {
        this.enabled = false;
        ParticleSystem.EmissionModule em = gameObject.GetComponent<ParticleSystem>().emission;
        em.rateOverTime = 0;
        ParticleSystem.MainModule mm = gameObject.GetComponent<ParticleSystem>().main;
        mm.gravityModifier = deathSequenceGravityMod;
        yield return new WaitForSeconds(mm.startLifetime.constant);
        Destroy(gameObject);
    }

    //called when an item is dropped. Then check to see if it was one within the collider
    public void ItemWasDropped(GameObject item) {
        //Check if this was an item in our collider, then if it was, stop checking for it to be dropped, and consider this a victory
        if (objectsInside.Contains(item)) {
            EventManager.OnItemDrop -= ItemWasDropped;
            StartCoroutine(DeathSequence());
            owner.ObjectiveComplete();
        }
    }

}
