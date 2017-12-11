using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelController : MonoBehaviour {

    private enum SquirrelState { Idle, Wander, Escape};
    SquirrelState state = SquirrelState.Idle;

    [Header("Idle Stats")]
    [SerializeField] private int minNumIdleLoops = 2;
    [SerializeField] private int maxNumIdleLoops = 2;
    [SerializeField] private int numMovementsBeforeSwap = 2;

    [Header("Movement Stats")]
    [SerializeField] private int minNumMovementLoops = 2;
    [SerializeField] private int maxNumMovementLoops = 5;
    [SerializeField] private float WanderMaxAngleChange = 10.0f;
    [SerializeField] private float wanderMovementSpeed = 2.0f;

    private int num_loops_remaining;
    private int num_jumps_until_swap;
    private Rigidbody rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        num_loops_remaining = Random.Range(minNumIdleLoops, maxNumIdleLoops);
        num_jumps_until_swap = numMovementsBeforeSwap;

        rb.velocity = transform.forward * wanderMovementSpeed;
    }

    #region Animation Events
    void idleLoopUpdate()
    {
        /*
        num_loops_remaining--;

        if(num_loops_remaining <= 0)
        {
            // set up state
            state = SquirrelState.Wander;

            // set up rotation
            if(num_jumps_until_swap <= 0)
            {
                //transform.forward = -transform.forward;
                num_jumps_until_swap = numMovementsBeforeSwap;
            }
            num_jumps_until_swap--;
            //transform.Rotate(transform.up, Random.Range(-WanderMaxAngleChange, WanderMaxAngleChange));

            // set up RigidBody
            rb.velocity = wanderMovementSpeed * transform.forward;

            anim.SetBool("isWander", true);
            num_loops_remaining = Random.Range(minNumMovementLoops, maxNumMovementLoops);
        }
        */
    }

    void wanderLoopUpdate()
    {
        /*
        num_loops_remaining--;

        if(num_loops_remaining <= 0)
        {
            rb.velocity = Vector3.zero;
            anim.SetBool("isWander", false);
        }
        */
    }

    void wanderApplyForce()
    {
        print("applying force");
        rb.AddForce(transform.forward * wanderMovementSpeed);
    }
    #endregion
}
