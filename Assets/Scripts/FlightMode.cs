using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class FlightMode : MonoBehaviour {

    [SerializeField] private bool isFlying;
    [SerializeField] private float speedOfFlight;
    [SerializeField] private float takeoffAngle;
    [SerializeField] private float tiltSensitivity;

    private Rigidbody rb;
    private TrailRenderer tr;
    private ParticleSystem ps;


	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody>();
        tr = this.GetComponent<TrailRenderer>();
        ps = this.GetComponent<ParticleSystem>();

        tr.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isFlying)
        {
            float vertical = Input.GetAxis("Vertical") * tiltSensitivity;
            float horiztonal = Input.GetAxis("Horizontal") * tiltSensitivity;

            this.transform.Rotate(new Vector3(-1f * vertical, horiztonal, 0f));
            rb.velocity = transform.forward * speedOfFlight;
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if (isFlying)
                DeactivateFlightMode();
            else
                ActivateFlightMode();
        }
	}

    public void ActivateFlightMode()
    {
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<ThirdPersonUserControl>().enabled = false;
        tr.enabled = true;
        ps.Play();
        this.transform.Rotate(new Vector3(-1f * takeoffAngle, 0f, 0f));
        isFlying = true;
    }

    public void DeactivateFlightMode()
    {
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<ThirdPersonUserControl>().enabled = true;
        tr.enabled = false;
        ps.Stop();
        isFlying = false;
    }

    public bool IsFlying()
    {
        return isFlying;
    }
}
