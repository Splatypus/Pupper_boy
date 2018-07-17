    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyAI : AIbase {

    Animator[] anim;
    public float moveDistance;
    public float speed;

    public enum States { Hiding, Rescued, Happy }; //this is used instead of the standard quest number variable because why not
    public States state = States.Hiding;

    public GameObject rewardSpawn; //location to spawn reward
    public GameObject reward; //what to spawn

    public GameObject[] birds; //array of birds attacking her

    public GameObject bandanaCollar;
    public GameObject defaultCollar;



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
            Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
            if (bandanaCollar != null && defaultCollar != null) {
                bandanaCollar.SetActive(true);
                defaultCollar.SetActive(false);
            }
        }
    }

    public void Saved() {
        //anim.SetFloat("Forward", 1.0f, 0.1f, Time.deltaTime);

        //anim.SetFloat("Forward", 0.8f);
        foreach(Animator a in anim)
        {
            a.SetFloat("Forward", 0.8f);
            a.SetTrigger("isSaved");
        }
        state = States.Rescued;
        progressionNum = 1; //go to the next dialog set
        Display(1);
    }

    new public void Start() {
        base.Start();
        anim = GetComponentsInChildren<Animator>();
    }

    public void Update() {
        //check if tiffy is safe now and if so run saved
        if (state == States.Hiding) {
            bool isSafe = true;
            for (int i = 0; i < birds.Length && isSafe; i++) {
                if (birds[i].GetComponent<BirdMovementV2>().curState == BirdMovementV2.BirdState.AttackWander) {
                    isSafe = false;
                }
            }
            //if there are no more birds, she saved
            if (isSafe) {
                Saved();
            }
        }
        //move forward if needed. This should probably be moved to a coroutine
        if (state == States.Rescued && moveDistance > 0) {
            transform.position += gameObject.transform.forward * Time.deltaTime * speed;
            moveDistance -= Time.deltaTime * speed;
            if (moveDistance < 0) {
                //anim.SetFloat("Forward", 0.0f); //, 0.1f, Time.deltaTime);

                foreach (Animator a in anim)
                {
                    a.SetFloat("Forward", 0.0f);
                }
            }
        }
    }

}
