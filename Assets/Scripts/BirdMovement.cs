using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour {

    GameObject Dog;
    enum BirdState {Wander, FlyAway, BathMode};

    // bird will wander then if you get close will fly away, then later come back
    // and chill on the bird bath

    [Header("Wander Stats")]
    public float time_per_hop;
    public float hop_force;
    public float wander_circle_center_dist;
    public float wander_circle_radius;

    public float dist_start_flight;

    BirdState m_state;
    float time_in_state = 0.0f;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        Dog = GameObject.FindGameObjectWithTag("Player");
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

        m_state = BirdState.Wander;

        float dist_to_dog = (Dog.transform.position - transform.position).magnitude;

        if (dist_to_dog < dist_start_flight)
            m_state = BirdState.FlyAway;

        // worry about going back to the bath later
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
        }
    }

    void WanderMove()
    {
        if(time_in_state > time_per_hop)
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

    }
}
