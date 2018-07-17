using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLaunchV2 : MonoBehaviour {

    // Launching variables
    [SerializeField] private float launch_force;
    [SerializeField] private Vector3 launch_direction;
    [SerializeField] private Transform launch_start_pos;

    [SerializeField] private Transform storePosition;
    [SerializeField] private float launch_delay;

    private AudioSource audioSource;

    private GameObject ballToLaunch = null;
    private Rigidbody ball_rb;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool can_launch()
    {
        return ballToLaunch == null;
    }

    public void LoadBall(GameObject ball)
    {
        ballToLaunch = ball;
        ballToLaunch.transform.position = storePosition.position;

        ball_rb = ballToLaunch.GetComponent<Rigidbody>();
        if(ball_rb != null)
        {
            ball_rb.useGravity = false;
            ball_rb.velocity = Vector3.zero;
        }

        SphereCollider sc = ballToLaunch.GetComponent<SphereCollider>();
        if(sc != null)
        {
            sc.enabled = false;
        }

        Invoke("LaunchBall", launch_delay);
    }

    void LaunchBall()
    {
        audioSource.Play();
        ballToLaunch.transform.position = launch_start_pos.position;

        SphereCollider sc = ballToLaunch.GetComponent<SphereCollider>();
        if (sc != null)
        {
            sc.enabled = true;
        }

        if (ball_rb != null)
        {
            ball_rb.useGravity = true;
            //ball_rb.velocity = launch_direction.normalized * launch_force;
            ball_rb.velocity = (transform.forward+ launch_direction).normalized * launch_force;
        }

        ballToLaunch = null;
        ball_rb = null;
    }
}
