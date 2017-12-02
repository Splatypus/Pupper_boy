using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyAI : AIbase {

    Animator anim;
    public float moveDistance;
    public float speed;

    public enum States { Hiding, Rescued, Happy };
    public States state = States.Hiding;

    public GameObject rewardSpawn;
    public GameObject reward;

    public GameObject[] birds;


    public override void ToyInRange() {
        if (state == States.Rescued) {
            base.ToyInRange();
            state = States.Happy;
            Display(Dialog[2]);
            Instantiate(reward, rewardSpawn.transform.position, rewardSpawn.transform.rotation);
        }
    }

    public void Saved() {
        //anim.SetFloat("Forward", 1.0f, 0.1f, Time.deltaTime);
        anim.SetFloat("Forward", 0.8f);
        state = States.Rescued;
    }

    public override void Start() {
        base.Start();
        anim = GetComponentInChildren<Animator>();
    }

    public override void Update() {
        base.Update();
        //check if tiffy is safe now and if so run saved
        if (state == States.Hiding) {
            bool isSafe = true;
            for (int i = 0; i < birds.Length && isSafe; i++) {
                if (birds[i].GetComponent<BirdMovementV2>().curState == BirdMovementV2.BirdState.AttackWander) {
                    isSafe = false;
                }
            }
            if (isSafe) {
                Saved();
            }
        }
        if (state == States.Rescued && moveDistance > 0) {
            transform.position += Vector3.back * Time.deltaTime * speed;
            moveDistance -= Time.deltaTime * speed;
            if (moveDistance < 0) {
                anim.SetFloat("Forward", 0.0f); //, 0.1f, Time.deltaTime);
            }
        }
    }

}
