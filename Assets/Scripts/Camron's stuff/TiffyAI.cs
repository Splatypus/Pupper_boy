using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiffyAI : AIbase {

    Animator anim;
    public float moveDistance;
    public float speed;

    public enum States { Hiding, Rescued, Happy };
    public States state = States.Hiding;


    public override void ToyInRange() {
        if (state == States.Rescued) {
            base.ToyInRange();
            state = States.Happy;
            Display(Dialog[2]);
        }
    }

    public void Saved() {
        //anim.SetFloat("Forward", 1.0f, 0.1f, Time.deltaTime);
        anim.SetFloat("Forward", 1.0f);
    }

    public override void Start() {
        base.Start();
        anim = GetComponentInChildren<Animator>();
    }

    public override void Update() {
        base.Update();
        if (Input.GetKeyDown(KeyCode.B)) {
            Saved();
        }
        if (state == States.Rescued && moveDistance > 0) {
            transform.position += Vector3.back * Time.deltaTime * speed;
            moveDistance -= Time.deltaTime * speed;
            if (moveDistance < 0) {
                anim.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                state = States.Rescued;
            }
        }
    }

}
