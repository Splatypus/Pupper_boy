using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelController : MonoBehaviour {

    // Code for states
    private enum SquirrelState { Idle, Wander, EscapeToTree, EscapeFromCorner, ClimbTree};
    SquirrelState state = SquirrelState.Idle;

    // Issue with model causes there to be discrepency between positions of things
    public Transform actual_positon;

    [Header("Idle Stats")]
    [SerializeField] private int minNumIdleLoops = 2;
    [SerializeField] private int maxNumIdleLoops = 2;
    [SerializeField] private int numMovementsBeforeSwap = 2;

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
    private GameObject treeTarget = null;

    [Header("Climb Stats")]
    [SerializeField] private float climbMoveSpeed;
    public AnimationCurve treeShapeCurve;

    private int num_loops_remaining;
    private int num_jumps_until_swap;
    private Rigidbody rb;
    private Animator anim;
    private bool onGround = false;
    private BoxCollider col;
    private float start_climb_time = 0.0f;
    private Vector3 start_tree_climb_pos = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        num_loops_remaining = Random.Range(minNumIdleLoops, maxNumIdleLoops);
        num_jumps_until_swap = numMovementsBeforeSwap;

        state = SquirrelState.Idle;
        col = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If we are in escape mode, act differently
        if(state == SquirrelState.EscapeToTree || state == SquirrelState.EscapeFromCorner)
        {
            // If we hit the tree, start climbing
            if (collision.gameObject.tag == "Tree")
            {
                StartClimb();

                // Testing ignoring tree collision after hitting it
                Physics.IgnoreCollision(collision.collider, col);
            }
            else if(collision.gameObject.tag != "Ground")
            {
                // Right now the squirrel wants to just ignore things that are not the tree and the ground
                // This lets us not have to worry about bushes and whatnot. This could potentially cause issues
                // with certain lawn setups, if there is something big near a tree
                Physics.IgnoreCollision(collision.collider, col);
            }
        }

        // Set up grounded state
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
        // Look to see if the player gets close enough to trigger fleeing
        if(other.tag == "Player" && (state == SquirrelState.Idle || state == SquirrelState.Wander))
        {
            StartEscape(other.gameObject);
        }

        // Checks when the squirrel hits upper part of the tree, then destroy it after a short delay
        if(state == SquirrelState.ClimbTree && other.tag == "Tree")
        {
            float death_timer = 0.01f * other.gameObject.transform.localScale.y;
            Destroy(this.gameObject, death_timer);

            //print("hit it at dt = " + (Time.time - start_climb_time) + " named " + other.name + " I am " + this.name);
        }
    }

    void StartEscape(GameObject player)
    {
        // Locate the nearest tree in our yard
        float distToNearestTree = float.MaxValue;
        GameObject closest_tree = null;
        Vector3 DogToSquirrelVec = actual_positon.position - player.transform.position;
        foreach (GameObject tree in treesInMyYard)
        {
            // Create a vector from the player to the tree
            Vector3 meToTreeVec = tree.transform.position - actual_positon.position;

            // Make sure that the player isn't blocking the tree
            if(Vector3.Dot(DogToSquirrelVec, meToTreeVec) > 0)
            {
                if(meToTreeVec.magnitude < distToNearestTree)
                {
                    closest_tree = tree;
                    distToNearestTree = meToTreeVec.magnitude;
                }
            }
        }

        // Our target for escape will be the closest tree
        treeTarget = closest_tree;

        // Check if we were able to find viable tree
        if (closest_tree == null)
        {
            // change to escape state
            state = SquirrelState.EscapeFromCorner;

            // Player has trapped the squirrel in a corner. I don't know the best behavior to do here
            // For now, run directly away from the player and just book it through the fence
            float angle_diff = Vector3.SignedAngle(actual_positon.forward, player.transform.position - actual_positon.position, transform.up) + 180f;
            transform.RotateAround(actual_positon.position, transform.up, angle_diff);
        }
        else
        {
            // change to escape state
            state = SquirrelState.EscapeToTree;

            // rotate towards the tree I want to go to
            // This doesn't really work, since the angle afterwards isn't zero
            float max_rot = 2f;
            float angle_diff = Vector3.SignedAngle(actual_positon.forward, treeTarget.transform.position - actual_positon.position, Vector3.up);
            angle_diff = Mathf.Clamp(-max_rot * Time.deltaTime, max_rot * Time.deltaTime, angle_diff);
            transform.RotateAround(actual_positon.position, Vector3.up, angle_diff);
        }

        anim.SetBool("isWander", true);
    }

    private void FixedUpdate()
    {
        // If our squirrel's movement uses constant moving rather than hopping, update movement
        if(constantMovement && onGround && state == SquirrelState.Wander)
        {
            if(rb.velocity.magnitude < wanderMovementSpeed)
                rb.velocity = wanderMovementSpeed * transform.forward;
        }

        // If we are running away not to a tree, just keep booking it
        if(state == SquirrelState.EscapeFromCorner && onGround)
        {
            if(rb.velocity.magnitude < escapeMoveSpeed)
                rb.velocity = transform.forward * escapeMoveSpeed;
        }

        // If we are attempting to make it to a tree, keep trying to look at that tree
        if (state == SquirrelState.EscapeToTree && onGround)
        {
            float angle_diff = Vector3.SignedAngle(actual_positon.forward, treeTarget.transform.position - actual_positon.position, Vector3.up);
            transform.RotateAround(actual_positon.position, Vector3.up, angle_diff);
            if (rb.velocity.magnitude < escapeMoveSpeed)
                rb.velocity = transform.forward * escapeMoveSpeed;
        }

        // If we are climbing, attempt to stick to the curve of the tree
        if(state == SquirrelState.ClimbTree)
        {
            float time_to_climb_tree = 1.7f;
            float tree_width_scale_const = 0.05f;
            Vector3 me_to_tree = actual_positon.position - treeTarget.transform.position;
            me_to_tree = Vector3.ProjectOnPlane(me_to_tree, Vector3.up);
            Vector3 new_pos = transform.position;

            float dist_from_start = treeShapeCurve.Evaluate((Time.time - start_climb_time) / time_to_climb_tree);

            me_to_tree = me_to_tree.normalized * dist_from_start * tree_width_scale_const;
            me_to_tree.y = 0;
            //print(me_to_tree);
            new_pos -= me_to_tree;
            transform.position = new_pos;
        }
    }

    // TODO: make this not shitty
    void StartClimb()
    {
        state = SquirrelState.ClimbTree;

        // turn off collider

        // turn off gravity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        // rotate up to climb
        transform.RotateAround(actual_positon.position, transform.right, -90);

        // set up velocity
        rb.velocity = Vector3.up * climbMoveSpeed;

        start_climb_time = Time.time;
        start_tree_climb_pos = transform.position;
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
            
        }
        
    }

    void wanderApplyForce()
    {
        if (state == SquirrelState.Wander)
        {
            rb.AddForce(transform.forward * wanderJumpForce1);
        }
        if(state == SquirrelState.EscapeToTree || state == SquirrelState.EscapeFromCorner)
        {
            rb.AddForce(transform.forward * escapeJumpForce);
        }
    }
    #endregion
}
