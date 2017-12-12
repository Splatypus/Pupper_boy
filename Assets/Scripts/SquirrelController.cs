using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelController : MonoBehaviour {

    private enum SquirrelState { Idle, Wander, Escape};
    SquirrelState state = SquirrelState.Idle;

    public Transform actual_positon;

    [Header("Idle Stats")]
    [SerializeField] private int minNumIdleLoops = 2;
    [SerializeField] private int maxNumIdleLoops = 2;
    [SerializeField] private int numMovementsBeforeSwap = 2;
    public PhysicMaterial idleMaterial;

    [Header("Movement Stats")]
    [SerializeField] private int minNumMovementLoops = 2;
    [SerializeField] private int maxNumMovementLoops = 5;
    [SerializeField] private float WanderMaxAngleChange = 10.0f;
    [SerializeField] private float wanderMovementSpeed = 2.0f;
    [SerializeField] private float wanderJumpForce1 = 150.0f;
    [SerializeField] private bool constantMovement = false;

    [Header("Escape Stats")]
    public GameObject[] treesInMyYard;
    [SerializeField] private float escapeMoveSpeed;
    [SerializeField] private float escapeJumpForce;

    private int num_loops_remaining;
    private int num_jumps_until_swap;
    private Rigidbody rb;
    private Animator anim;
    private bool onGround = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        num_loops_remaining = Random.Range(minNumIdleLoops, maxNumIdleLoops);
        num_jumps_until_swap = numMovementsBeforeSwap;

        state = SquirrelState.Idle;
    }


    // grounded stuff
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            print("RUN AWAY DOGGO IS NEAR");
            startEscape(other.gameObject);
        }
    }
    

    void startEscape(GameObject player)
    {
        // change state
        state = SquirrelState.Escape;

        float distToNearestTree = float.MaxValue;
        GameObject closest_tree = null;
        Vector3 DogToSquirrelVec = actual_positon.position - player.transform.position;
        foreach (GameObject tree in treesInMyYard)
        {
            Vector3 meToTreeVec = tree.transform.position - actual_positon.position;
            //print("Dot for tree named " + tree.name + " is: " + Vector3.Dot(DogToSquirrelVec, meToTreeVec));
            if(Vector3.Dot(DogToSquirrelVec, meToTreeVec) > 0)
            {
                if(meToTreeVec.magnitude < distToNearestTree)
                {
                    closest_tree = tree;
                    distToNearestTree = meToTreeVec.magnitude;
                }
            }
        }

        if(closest_tree == null)
        {
            // dog has trapped the squirrel in a corner
            // death is the only solution?
            
            // I think that they should run for the fence, but I really don't know the best thing to do at this point
        }
        else
        {
            // rotate towards the tree I want to go to
            //float angle_diff = Vector3.SignedAngle(DogToSquirrelVec, closest_tree.transform.position - actual_positon.position, transform.up);
            float angle_diff = Vector3.SignedAngle(transform.forward, closest_tree.transform.position - actual_positon.position, transform.up);
            print("angle diff: " + angle_diff);
            transform.RotateAround(actual_positon.position, transform.up, angle_diff);

            // run that way real speedylike
            anim.SetBool("isWander", true);
        }
    }

    private void FixedUpdate()
    {
        if(constantMovement && onGround && state == SquirrelState.Wander)
        {
            if(rb.velocity.magnitude < wanderMovementSpeed)
                rb.velocity = wanderMovementSpeed * transform.forward;
        }

        if(state == SquirrelState.Escape && onGround)
        {
            if(rb.velocity.magnitude < escapeMoveSpeed)
                rb.velocity = transform.forward * escapeMoveSpeed;
        }
    }

    #region Animation Events
    void idleLoopUpdate()
    {
        if (state != SquirrelState.Idle)
            return;
        
        num_loops_remaining--;

        if(num_loops_remaining < 0)
        {
            // set up state
            state = SquirrelState.Wander;

            // set up rotation
            if(num_jumps_until_swap <= 0)
            {
                transform.RotateAround(actual_positon.position, transform.up, 45);
                num_jumps_until_swap = numMovementsBeforeSwap;
            }
            num_jumps_until_swap--;

            //transform.RotateAround(actual_positon.position, transform.up, Random.Range(-WanderMaxAngleChange, WanderMaxAngleChange));
            transform.RotateAround(actual_positon.position, transform.up, 15f);

            anim.SetBool("isWander", true);
            num_loops_remaining = Random.Range(minNumMovementLoops, maxNumMovementLoops);
        }
        
    }

    void wanderLoopUpdate()
    {
        if (state != SquirrelState.Wander)
            return;

        num_loops_remaining--;

        if(num_loops_remaining < 0)
        {
            // set up state
            state = SquirrelState.Idle;

            // ste up loops
            num_loops_remaining = Random.Range(minNumIdleLoops, maxNumIdleLoops);

            // stop moving
            rb.velocity = Vector3.zero;

            // set up animator
            anim.SetBool("isWander", false);

            BoxCollider mcol = GetComponent<BoxCollider>();
        }
        
    }

    void wanderApplyForce()
    {
        if (state == SquirrelState.Wander)
        {
            rb.AddForce(transform.forward * wanderJumpForce1);
        }
        if(state == SquirrelState.Escape)
        {
            rb.AddForce(transform.forward * escapeJumpForce);
        }
    }
    #endregion
}
