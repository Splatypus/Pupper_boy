using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour {

    GameObject Dog;
    enum BirdState {Wander, Afraid, FlyAway};

    [Header("Wander Stats")]
    public float wander_movespeed;

    public float dist_to_be_afraid;
    public float dist_to_be_fly_away;

    BirdState m_state;

	// Use this for initialization
	void Start () {
        //Dog = FindObjectOfType<>
        Dog = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update () {
        UpdateState();
        MoveBird();
    }

    void UpdateState()
    {
        if (!Dog)
        {
            m_state = BirdState.Wander;
        }

        float dist_to_dog = (Dog.transform.position - transform.position).magnitude;

        m_state = BirdState.Wander;

        if (dist_to_dog < dist_to_be_afraid)
            m_state = BirdState.Afraid;

        if (dist_to_dog < dist_to_be_fly_away)
            m_state = BirdState.FlyAway;
    }

    void MoveBird()
    {
        switch(m_state)
        {
            case BirdState.Wander:
                WanderMove();
                break;
            case BirdState.Afraid:
                AfraidMove();
                break;
            case BirdState.FlyAway:
                FlyAway();
                break;
        }
    }

    void WanderMove()
    {

    }

    void AfraidMove()
    {

    }

    void FlyAway()
    {

    }
}
