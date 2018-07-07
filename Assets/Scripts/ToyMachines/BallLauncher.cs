using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour {

    [SerializeField] private Vector3 launchDirection;           //direction of ball launch
    [SerializeField] private float launchForce;                 //force of launch, applied in direction specified above

    [SerializeField] private Vector3 storePosition;           //position within ball launcher to store it until launch

    private GameObject ballForLaunch = null;                    //ball about to be launched

    [SerializeField] private float waitTime;                    //time between ball being loaded and ball being launched
    private float timer = 0f;                                   //current time from load to launch

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(ballForLaunch != null)
		{
            ballForLaunch.transform.rotation = this.transform.rotation;
            ballForLaunch.transform.localPosition = storePosition;

            timer += Time.deltaTime;
            if (timer >= waitTime)
                LaunchBall();
        }
	}


    public void LoadBall( GameObject ball)
    {
        ballForLaunch = ball;
        ballForLaunch.transform.parent = this.transform;
        ballForLaunch.transform.localPosition = storePosition;
        ballForLaunch.GetComponent<Rigidbody>().useGravity = false;
        timer = 0f;
    }

    void LaunchBall()
    {
        ballForLaunch.transform.parent = null;
        ballForLaunch.GetComponent<Rigidbody>().useGravity = true;
        ballForLaunch.GetComponent<Rigidbody>().AddForce(launchDirection * launchForce, ForceMode.Impulse);
        ballForLaunch = null;
    }
}
