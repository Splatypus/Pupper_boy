using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppyController : MonoBehaviour {

    public float moveSpeed = 3.0f;
    public float rotSpeed = 5.0f;
    public float cameraRotSpeed = 2.0f;
    public Camera m_camera;
    Rigidbody rb;

    public FlightMode flightMode;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        /*
        Cursor.lockState = CursorLockMode.Locked;
        */
    }

    void swapCameraMode()
    {
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!flightMode.IsFlying())
        {
            // move the dog
            float vertical = Input.GetAxis("Vertical");
            float horiztonal = Input.GetAxis("Horizontal");

            // rotate the doggo
            rb.angularVelocity = new Vector3(0, horiztonal * rotSpeed, 0);


            rb.velocity = transform.forward * vertical * moveSpeed;
        }
    }
}
