using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDive : MonoBehaviour {

    GameObject Dog;
    enum BirdState { Dive, Wander, FlyAway, FlyDown, BathMode };

    private bool isDiving = true;

    // bird will wander then if you get close will fly away, then later come back
    // and chill on the bird bath
    public bool wander_on_land = false;

    public birdTracker birdTracker;

    [Header("Dive Points")]
    public GameObject objectA;
    public GameObject objectB;
    public float stagger = 1;

    [Header("Wander Stats")]
    public float time_per_hop;
    public float hop_force;
    public float wander_circle_center_dist;
    public float wander_circle_radius;
    public float dist_start_flight;

    [Header("Flight Stats")]
    public float flight_height_target;
    public float flight_speed;
    public Vector3 flight_vector;
    public float flight_wait_delay;

    [Header("Fly Down")]
    public Transform bird_bath_pos;

    [Header("Bath stats")]
    public float dist_for_bath_runaway;

    private Animator m_animator;

    BirdState m_state;
    float time_in_state = 0.0f;
    Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Dog = GameObject.FindGameObjectWithTag("Player");
        flight_vector.Normalize();

        m_animator = GetComponent<Animator>();

        // animation state is wander/hop
        AnimationClip clip;

        AnimationEvent evt = new AnimationEvent();

        evt.time = 0.0f;
        evt.functionName = "hop_forward";
        clip = m_animator.runtimeAnimatorController.animationClips[0];
        clip.AddEvent(evt);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        MoveBird();
    }

    void UpdateState()
    {
        // if there is no dog, just dive or wander
        if (!Dog)
        {
            Dog = GameObject.FindGameObjectWithTag("Player");
            if (isDiving)
            {
                m_state = BirdState.Dive;
                m_animator.SetBool("flying", true);
            }
            else
            {
                m_state = BirdState.Wander;
            }
            
        }


        float dist_to_dog = (Dog.transform.position - transform.position).magnitude;

        if (m_state == BirdState.Wander && dist_to_dog < dist_start_flight)
        {
            if (m_state != BirdState.FlyAway)
                time_in_state = 0.0f;
            m_state = BirdState.FlyAway;
            // animation state is flying
            m_animator.SetBool("flying", true);
        }
        if ((m_state == BirdState.BathMode || m_state == BirdState.FlyDown) && dist_to_dog < dist_for_bath_runaway)
        {
            m_state = BirdState.FlyAway;
            // animation state is flying (is there an issue if we go from fly->fly?)
            m_animator.SetBool("flying", true);
        }
        if (m_state == BirdState.Dive && dist_to_dog < dist_start_flight)
        {
            isDiving = false;
            birdTracker.birdsDiving--;
            m_state = BirdState.FlyAway;
            m_animator.SetBool("flying", true);
        }

    }

    void MoveBird()
    {
        switch (m_state)
        {
            case BirdState.Dive:
                diveBomb();
                break;
            case BirdState.Wander:
                //WanderMove();
                break;
            case BirdState.FlyAway:
                FlyAway();
                break;
            case BirdState.FlyDown:
                FlyDown();
                break;
            case BirdState.BathMode:
                UpdateState();
                break;
        }
    }

    void hop_forward()
    {
        if (m_state == BirdState.Wander)
        {
            Vector3 wander_circle_center = transform.right * -1 * wander_circle_center_dist;
            Vector2 circle_pos = ((Vector3)Random.insideUnitCircle.normalized * wander_circle_radius);
            Vector3 wander_dest = wander_circle_center + new Vector3(circle_pos.x, 0, circle_pos.y);

            transform.right = -wander_dest.normalized;
            rb.AddForce(wander_dest.normalized * hop_force);
        }
    }


    void WanderMove()
    {
        UpdateState();
        if (time_in_state > time_per_hop)
        {
            time_in_state = 0.0f;

            // find a direction to hop in
            Vector3 wander_circle_center = transform.right * -1 * wander_circle_center_dist;
            Vector2 circle_pos = ((Vector3)Random.insideUnitCircle.normalized * wander_circle_radius);
            Vector3 wander_dest = wander_circle_center + new Vector3(circle_pos.x, 0, circle_pos.y);

            transform.right = -wander_dest.normalized;
            rb.AddForce(wander_dest.normalized * hop_force);
        }
        else
        {
            time_in_state += Time.deltaTime;
        }
    }

    void FlyAway()
    {
        if (transform.position.y > flight_height_target)
        {
            // wait then fly down
            Invoke("StartFlyDown", flight_wait_delay);
        }

        rb.velocity = flight_vector * flight_speed;
        transform.right = -flight_vector;
    }

    void FlyDown()
    {
        rb.velocity = (bird_bath_pos.position - transform.position).normalized * flight_speed;
        transform.right = -rb.velocity.normalized;
        if ((bird_bath_pos.position - transform.position).magnitude < 0.1f)
        {
            if (wander_on_land)
            {
                rb.useGravity = true;
                m_state = BirdState.Wander;
            }
            else
                m_state = BirdState.BathMode;

            rb.velocity = Vector3.zero;
            transform.position = bird_bath_pos.position;
            transform.rotation = bird_bath_pos.rotation;
            rb.angularVelocity = Vector3.zero;
            // set animation state to sitting on birdbath
            // can also do some stuff with getting him to play in the bath
            m_animator.SetBool("flying", false);
        }
    }

    void StartFlyDown()
    {
        m_state = BirdState.FlyDown;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, flight_height_target, transform.position.z);
    }

    void diveBomb()
    {
        transform.position = Vector3.Lerp(objectA.transform.position, objectB.transform.position, Mathf.PingPong(Time.time, stagger));
    }

}
