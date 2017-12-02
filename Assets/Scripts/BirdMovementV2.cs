using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovementV2 : MonoBehaviour {

    // state enum stuff
    public enum BirdState { Wander, FlyAway, FlyWander, FlyDown, BathMode, AttackWander};
    public BirdState curState = BirdState.Wander;

    #region Wander Info
    // private vars
    int numWanderHops = 0;

    // public vars
    public float WanderHopForce;
    public float WanderMaxAngleChange;
    public int WanderNumHopsToTurnAround;
    [SerializeField] private PhysicMaterial moveMat;
    [SerializeField] private PhysicMaterial waitMat;
    #endregion

    #region Flight Start Info
    [Header("Flight Start Variables")]
    public float DogDistanceUntilFlight;
    public float borkDistanceUntilFlight; // TODO: make this work
    public Vector3 flightStartVelocity;
    private float startHeight;
    #endregion

    #region Flight Wander
    [Header("Flight Wander Variables")]
    public float heightToStartFlightWander = 30.0f;
    private Vector3 airPosDest;
    #endregion

    #region Flight Land
    [Header("Flight Landing Variables")]
    public Transform landingDest;
    public BirdState landingState = BirdState.Wander;
    #endregion

    #region Attack Wander
    [Header("Attack Wander Variables")]
    public Transform[] attackWanderWaypoints;
    private int attackWanderWaypointIndex = 0;
    #endregion


    // private variables
    Animator anim;
    Rigidbody rb;
    BoxCollider col;
    GameObject player;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player");

        if(curState == BirdState.AttackWander)
        {
            startAttack();
        }
    }

    private void Update()
    {
        // when wandering all we have to do is watch out for the dog, movement is controlled by animation events
        if( (curState == BirdState.Wander || curState == BirdState.FlyDown || curState == BirdState.BathMode || curState == BirdState.AttackWander) &&
            Vector3.Distance(transform.position, player.transform.position) < DogDistanceUntilFlight)
        {
            startFlight();
        }
        else if(curState == BirdState.FlyAway && transform.position.y - startHeight >= heightToStartFlightWander)
        {
            // We have reached our goal height, so now enter into a holding pattern until we are able to land
            startFlightWander();
        }
        else if (curState == BirdState.FlyWander && Vector3.Distance(transform.position, airPosDest) < 0.5f)
        {
            startFlyDown();
        }
        else if (curState == BirdState.FlyDown && Vector3.Distance(transform.position, landingDest.position) < 0.1f)
        {
            finishFlight();
        }
        //else if(curState == BirdState.AttackWander &&
        //        Vector3.Distance(transform.position, attackWanderWaypoints[attackWanderWaypointIndex].position) < 0.3f)
        else if(curState == BirdState.AttackWander)
        {
            print("dist: " + Vector3.Distance(transform.position, attackWanderWaypoints[attackWanderWaypointIndex].position));
            if(Vector3.Distance(transform.position, attackWanderWaypoints[attackWanderWaypointIndex].position) < 0.3f)
                nextAttackWaypoint();
        }
    }

    public void getBarkedAt(Vector3 bark_pos)
    {
        if( (curState == BirdState.Wander || curState == BirdState.FlyDown || curState == BirdState.AttackWander) &&
            Vector3.Distance(transform.position, player.transform.position) < borkDistanceUntilFlight)
        {
            startFlight();
        }
    }

    private void startFlight()
    {
        // set behavior state
        curState = BirdState.FlyAway;

        // set up animation
        anim.SetBool("flying", true);

        // turn bird away from player
        /// project the vector from player to bird onto bird's up vector
        Vector3 birdPlayerVec = transform.position - player.transform.position;
        birdPlayerVec = Vector3.ProjectOnPlane(birdPlayerVec, transform.up);

        /// make the bird look in this direction
        transform.right = -birdPlayerVec.normalized;

        // start flying away
        /// turn off gravity and collider so that we fly up and don't hit things
        /// maybe don't turn off collier?
        rb.useGravity = false;
        col.enabled = false;
        rb.velocity = flightStartVelocity;
        startHeight = transform.position.y;
    }

    private void startAttack()
    {
        anim.SetBool("flying", true);

        // find best box to start me out on
        float minDist = float.MaxValue;
        for(int i = 0; i < attackWanderWaypoints.Length; ++i)
        {
            float dist = Vector3.Distance(attackWanderWaypoints[i].position, transform.position);
            if (dist < minDist)
            {
                attackWanderWaypointIndex = i;
                minDist = dist;
            }
        }

        Vector3 line = (attackWanderWaypoints[attackWanderWaypointIndex].position - transform.position).normalized;
        transform.right = -line;

        // start flying
        rb.useGravity = false;
        col.enabled = false;
        rb.velocity = line * flightStartVelocity.magnitude;
    }

    private void startFlightWander()
    {
        // change state
        curState = BirdState.FlyWander;

        // Fix velocity to be in holding
        airPosDest = landingDest.position;
        airPosDest.y = transform.position.y;
        /// set up velocity to move us to airPosDest
        rb.velocity = (airPosDest - transform.position).normalized * flightStartVelocity.magnitude;

        // rotate the bird
        transform.right = -(airPosDest - transform.position).normalized;
    }

    private void startFlyDown()
    {
        curState = BirdState.FlyDown;

        rb.velocity = (landingDest.position - transform.position).normalized * flightStartVelocity.magnitude;
        transform.right = -landingDest.forward;
    }

    private void finishFlight()
    {
        transform.position = landingDest.position;

        col.enabled = true;
        rb.useGravity = true;

        curState = landingState;
        anim.SetBool("flying", false);
    }

    private void nextAttackWaypoint()
    {
        // update index
        attackWanderWaypointIndex++;
        attackWanderWaypointIndex = attackWanderWaypointIndex % attackWanderWaypoints.Length;

        // update velocity
        Vector3 line = (attackWanderWaypoints[attackWanderWaypointIndex].position - transform.position).normalized;
        //transform.right = -line;
        // try this to stop flipping?
        transform.right = -Vector3.ProjectOnPlane(line, Vector3.up);
        rb.velocity = line * flightStartVelocity.magnitude;

        
    }

    #region Animation Events
    void have_friction()
    {
        if (curState == BirdState.Wander)
            col.material = waitMat;
    }

    void small_hop_forward()
    {
        if (curState == BirdState.Wander)
            rb.AddForce(transform.right * -WanderHopForce);
    }
    
    void hop_forward()
    {
        if (curState == BirdState.Wander)
        {
            col.material = moveMat;
            if (numWanderHops >= WanderNumHopsToTurnAround)
            {
                transform.right = -transform.right;
                numWanderHops = 0;
            }
            else
            {
                transform.Rotate(transform.up, Random.Range(-WanderMaxAngleChange, WanderMaxAngleChange));
            }

            rb.AddForce(transform.right * -WanderHopForce);
            numWanderHops++;
        }

    }
#endregion
}
