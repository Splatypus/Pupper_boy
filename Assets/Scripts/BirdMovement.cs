using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour {

    GameObject Dog;
    enum BirdState {Wander, FlyAway, FlyDown, BathMode};

    // bird will wander then if you get close will fly away, then later come back
    // and chill on the bird bath

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

    BirdState m_state;
    float time_in_state = 0.0f;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        Dog = GameObject.FindGameObjectWithTag("Player");
        flight_vector.Normalize();
	}

	// Update is called once per frame
	void Update () {
        UpdateState();
        MoveBird();
    }

    void UpdateState()
    {
        // if there is no dog, just wander
        if (!Dog)
        {
            Dog = GameObject.FindGameObjectWithTag("Player");
            m_state = BirdState.Wander;
        }
        

        float dist_to_dog = (Dog.transform.position - transform.position).magnitude;

        if (m_state == BirdState.Wander && dist_to_dog < dist_start_flight)
        {
            if (m_state != BirdState.FlyAway)
                time_in_state = 0.0f;
            m_state = BirdState.FlyAway;
        }
        if ((m_state == BirdState.BathMode || m_state == BirdState.FlyDown) && dist_to_dog < dist_for_bath_runaway)
        {
            m_state = BirdState.FlyAway;
        }
        
    }

    void MoveBird()
    {
        switch(m_state)
        {
            case BirdState.Wander:
                WanderMove();
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


    void WanderMove()
    {
        UpdateState();
        if (time_in_state > time_per_hop)
        {
            time_in_state = 0.0f;

            // find a direction to hop in
            Vector3 wander_circle_center = transform.forward * wander_circle_center_dist;
            Vector2 circle_pos = ((Vector3)Random.insideUnitCircle.normalized * wander_circle_radius);
            Vector3 wander_dest = wander_circle_center + new Vector3(circle_pos.x, 0, circle_pos.y);

            transform.forward = wander_dest.normalized;
            rb.AddForce(wander_dest.normalized * hop_force);
        }
        else
        {
            time_in_state += Time.deltaTime;
        }
    }

    void FlyAway()
    {
        if(transform.position.y > flight_height_target)
        {
            // wait then fly down
            Invoke("StartFlyDown", flight_wait_delay);
        }

        rb.velocity = flight_vector * flight_speed;
    }

    void FlyDown()
    {
        rb.velocity = (bird_bath_pos.position - transform.position).normalized * flight_speed;
        if((bird_bath_pos.position - transform.position).magnitude < 0.1f)
        {
            m_state = BirdState.BathMode;
            rb.velocity = Vector3.zero;
        }
    }

    void StartFlyDown()
    {
        m_state = BirdState.FlyDown;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, flight_height_target, transform.position.z);
    }
}
