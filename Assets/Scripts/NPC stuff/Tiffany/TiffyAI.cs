    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyAI : AIbase {

    Animator[] anim;
    public float moveDistance;
    public float walkTime;
    public float birdCount;

    public enum States { Hiding, Rescued, Happy }; //this is used instead of the standard quest number variable because why not
    public States state = States.Hiding;

    public GameObject rewardSpawn; //location to spawn reward
    public GameObject reward; //what to spawn

    public GameObject bandanaCollar;
    public GameObject defaultCollar;

    new public void Start() {
        base.Start();
        anim = GetComponentsInChildren<Animator>();
        EventManager.OnFenceDig += OnDoggoEntersYard;
    }

    //triggers the first time doggo enters tiffany's yard
    public void OnDoggoEntersYard(GameObject yard) {
        if (yard.GetComponent<DigZone>().enteringYard == DigZone.Yards.Tiffany) {
            EventManager.OnFenceDig -= OnDoggoEntersYard;
            OnInteract();
        }
    }

    public override void OnInRange() {
        if (state == States.Hiding) {
            Display(0);
        } else if (state == States.Rescued) {
            Display(1);
        } else if (state == States.Happy) {
            Display(2);
        }
    }

    //called when a toy is brought in range. If its the bandana, progress the quest
    public override void ToyInRange(Interactable toy) {
        base.ToyInRange(toy);
        //check to make sure the quest state and toy tag are correct. If so, delete the bandana and do her stuff
        if (toy.hasTag(Interactable.Tag.TiffyQuestItem) && state == States.Rescued) {
            DestoryObjectInMouth(toy.gameObject);
            state = States.Happy;
            progressionNum = 1;
            Display(2);
            if (bandanaCollar != null && defaultCollar != null) {
                bandanaCollar.SetActive(true);
                defaultCollar.SetActive(false);
            }
        }
    }

    public void SpawnReward() {
        Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
    }

    public void Saved() {
        state = States.Rescued;
        progressionNum = 1; //go to the next dialog set
        Display(1);
        StartCoroutine(WalkOut());
    }

    //moves tiffany out of her dog house
    IEnumerator WalkOut() {
        //start anims
        foreach (Animator a in anim) {
            a.SetFloat("Forward", 0.8f);
            a.SetTrigger("isSaved");
        }

        //move
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosiotion = startPosition + gameObject.transform.forward * moveDistance;
        while (startTime + walkTime > Time.time) {
            transform.position = Vector3.Lerp(startPosition, endPosiotion, (Time.time - startTime)/walkTime);
            yield return new WaitForFixedUpdate();
        }

        //end anims
        foreach (Animator a in anim) {
            a.SetFloat("Forward", 0.0f);
        }
    }
   
    //called when a bird is scared off. If shes hiding, check if all birds have been spooked away, then trigger actions if they have
    public void BirdScared() {
        if (state == States.Hiding) {
            birdCount--;
            if (birdCount == 0) {
                Saved();
            }
        }
    }

}
