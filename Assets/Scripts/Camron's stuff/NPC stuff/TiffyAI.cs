    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyAI : AIbase {

    Animator[] anim;
    public float moveDistance;
    public float speed;

    public enum States { Hiding, Rescued, Happy }; //this is used instead of the standard quest number variable because why not
    public States state = States.Hiding;

    public GameObject rewardSpawn;
    public GameObject reward;

    public GameObject[] birds;

    public GameObject bandanaObject;
    public GameObject noBandanaObject;


    public override void OnInRange() {
        if (state == States.Hiding) {
            Display(Icons[0]);
        } else if (state == States.Rescued) {
            Display(Icons[1]);
        } else if (state == States.Happy) {
            Display(Icons[2]);
        }
    }

    public override void ToyInRange() {
        if (state == States.Rescued) {
            print("TOYTHING");
            base.ToyInRange();
            state = States.Happy;
            SetConversationNumber();
            Display(Icons[2]);
            Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
            if (bandanaObject != null && noBandanaObject != null) {
                bandanaObject.SetActive(true);
                noBandanaObject.SetActive(false);
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
        SetConversationNumber(); //go to the next dialog set
        Display(Icons[1]);
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
        if (state == States.Rescued && moveDistance > 0) {
            transform.position += Vector3.back * Time.deltaTime * speed;
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
