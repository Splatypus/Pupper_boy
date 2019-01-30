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
    public float randomStartupTimeMax = 1.0f;
    #endregion

    #region Flight Start Info
    [Header("Flight Start Variables")]
    public float DogDistanceUntilFlight;
    public float borkDistanceUntilFlight;
    public float flightStartVelocity; //upwards velocity
    public float acceleration;
    public float accelerationRange;
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
    Vector3 desiredRight;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player");

        if(curState == BirdState.AttackWander)
        {
            StartAtack();
        }

        if(curState == BirdState.Wander)
        {
            //anim.StopPlayback();
            anim.enabled = false;
            Invoke("StartAnimating", Random.Range(0.0f, randomStartupTimeMax));
        }

        EventManager.OnBark += GetBarkedAt;
    }

    private void StartAnimating()
    {
        anim.enabled = true;
    }

    float t = 0.0f;
    private void Update()
    {
        // PLACEHOLDER
        //transform.right = desiredRight;
        if(curState == BirdState.FlyWander || curState == BirdState.AttackWander)
        {
            if (Vector3.Distance(transform.right, desiredRight) > float.Epsilon)
            {
                //print("doing lerp t = " + t);
                t += Time.deltaTime;
                transform.right = Vector3.Lerp(transform.right, desiredRight, t / 50);
            }
            else
            {
                t = 0.0f;
            }
        }

        // when wandering all we have to do is watch out for the dog, movement is controlled by animation events
        if( (curState == BirdState.Wander || curState == BirdState.FlyDown || curState == BirdState.BathMode || curState == BirdState.AttackWander) &&
            Vector3.Distance(transform.position, player.transform.position) < DogDistanceUntilFlight)
        {
            StartFlight();
        }
        else if(curState == BirdState.FlyAway && transform.position.y - startHeight >= heightToStartFlightWander)
        {
            // We have reached our goal height, so now enter into a holding pattern until we are able to land
            StartFlightWander();
        }
        else if (curState == BirdState.FlyWander && Vector3.Distance(transform.position, airPosDest) < 0.5f)
        {
            StartFlyDown();
        }
        else if (curState == BirdState.FlyDown && Vector3.Distance(transform.position, landingDest.position) < 0.1f)
        {
            FinishFlight();
        }
        //else if(curState == BirdState.AttackWander &&
        //        Vector3.Distance(transform.position, attackWanderWaypoints[attackWanderWaypointIndex].position) < 0.3f)
        else if(curState == BirdState.AttackWander)
        {
            if(Vector3.Distance(transform.position, attackWanderWaypoints[attackWanderWaypointIndex].position) < 0.3f)
                NextAttackWaypoint();

            if(Vector3.Distance(transform.position, attackWanderWaypoints[attackWanderWaypointIndex].position) > 50f)
            {
                transform.position = attackWanderWaypoints[attackWanderWaypointIndex].position;
                NextAttackWaypoint();
            }
        }
    }

    public void GetBarkedAt(GameObject p){
        Vector3 bark_pos = p.transform.position;
        if( (curState == BirdState.Wander || curState == BirdState.FlyDown || curState == BirdState.AttackWander) &&
            Vector3.Distance(transform.position, player.transform.position) < borkDistanceUntilFlight){
            StartFlight();
        }
    }

    public virtual void StartFlight()
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
        //transform.right = -birdPlayerVec.normalized;
        

        // start flying away
        /// turn off gravity and collider so that we fly up and don't hit things
        /// maybe don't turn off collier?
        rb.useGravity = false;
        col.enabled = false;
        
        //Vector3 actualFlightVelocity = (Vector3.up + birdPlayerVec.normalized * 0.5f).normalized * flightStartVelocity;
        rb.velocity = birdPlayerVec * 0.5f;
        StartCoroutine(RampSpeedOverTime(flightStartVelocity, acceleration, accelerationRange));

        transform.right = -Vector3.ProjectOnPlane(birdPlayerVec * 0.5f, transform.up).normalized;

        startHeight = transform.position.y;
    }

    private void StartAtack()
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

        transform.position = attackWanderWaypoints[attackWanderWaypointIndex].position;
        /*
        attackWanderWaypointIndex++;

        Vector3 line = (attackWanderWaypoints[attackWanderWaypointIndex].position - transform.position).normalized;
        transform.right = -line;
        */

        NextAttackWaypoint();
        // start flying
        rb.useGravity = false;
        col.enabled = false;
        //rb.velocity = line * flightStartVelocity.magnitude;
    }

    private void StartFlightWander() {
        // change state
        curState = BirdState.FlyWander;

        if (landingDest != null) {
            // Fix velocity to be in holding
            airPosDest = landingDest.position;
            airPosDest.y = transform.position.y;
            /// set up velocity to move us to airPosDest
            rb.velocity = (airPosDest - transform.position).normalized * flightStartVelocity;

            // rotate the bird
            //transform.right = -(airPosDest - transform.position).normalized;
            desiredRight = -(airPosDest - transform.position).normalized;
        } else {
            //if we have no landing dest, just go straight and set up a fade out routine
            rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
            StartCoroutine(DeathSequence(5.0f, 5.0f));
        }
    }

    private void StartFlyDown()
    {
        curState = BirdState.FlyDown;

        rb.velocity = (landingDest.position - transform.position).normalized * flightStartVelocity;
        //transform.right = -landingDest.forward;
        desiredRight = -landingDest.forward;
    }

    private void FinishFlight()
    {
        transform.position = landingDest.position;

        col.enabled = true;
        rb.useGravity = true;

        curState = landingState;
        anim.SetBool("flying", false);
    }

    private void NextAttackWaypoint()
    {
        // update index
        attackWanderWaypointIndex++;
        attackWanderWaypointIndex = attackWanderWaypointIndex % attackWanderWaypoints.Length;

        // update velocity
        Vector3 line = (attackWanderWaypoints[attackWanderWaypointIndex].position - transform.position).normalized;
        //transform.right = -line;
        // try this to stop flipping?
        //transform.right = -Vector3.ProjectOnPlane(line, Vector3.up);
        desiredRight = -Vector3.ProjectOnPlane(line, Vector3.up);
        rb.velocity = line * flightStartVelocity;

        
    }

    //Ramps up speed to maxSpeed at the acceleration rate
    IEnumerator RampSpeedOverTime(float maxSpeed, float acc, float accRange) {
        float newAcc = Random.Range(acc - accRange, acc + accRange);
        while (rb.velocity.y < maxSpeed) {
            rb.velocity += new Vector3(0.0f, newAcc * Time.fixedDeltaTime, 0.0f);
            yield return new WaitForFixedUpdate();
        }
    }

    //Sets a delay and then fades the bird out of existance afterward
    IEnumerator DeathSequence(float delay, float fadeTime) {
        yield return new WaitForSeconds(delay);
        Vector3 startScale = gameObject.transform.localScale;
        float startTime = Time.time;
        while (startTime + fadeTime > Time.time) {
            gameObject.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, (Time.time - startTime) / fadeTime);
            yield return new WaitForFixedUpdate();
        }
        EventManager.OnBark -= GetBarkedAt;
        Destroy(gameObject);
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
