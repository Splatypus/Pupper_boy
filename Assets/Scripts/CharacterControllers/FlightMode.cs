using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class FlightMode : Controller {
    /*
    [SerializeField] private float speedOfFlight;
    [SerializeField] private float takeoffAngle;
    [SerializeField] private float tiltSensitivity;

    private Rigidbody rb;
    private TrailRenderer tr;
    private ParticleSystem ps;
    
    public AudioSource launchSound;
    public AudioSource flightSound;
    public AudioClip flightMusic;

    BoxCollider col;
    [SerializeField] private PhysicMaterial flightMaterial;

    
    private PuppyPickup pickup;
    private Vector3 euler_rotation;
    RigidbodyConstraints initial_rb_constratins;

    Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponentInChildren<Animator>();
        rb = this.GetComponent<Rigidbody>();
        tr = this.GetComponentInChildren<TrailRenderer>();
        ps = this.GetComponent<ParticleSystem>();
        col = GetComponent<BoxCollider>();

        tr.enabled = false;

        pickup = FindObjectOfType<PuppyPickup>();

        initial_rb_constratins = rb.constraints;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        if (Input.GetButtonDown("Fly"))
        {
            gameObject.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Walking);
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
            float vertical = Input.GetAxis("Vertical") * tiltSensitivity;
            float horiztonal = Input.GetAxis("Horizontal") * tiltSensitivity;

            this.transform.Rotate(new Vector3(-1f * vertical, horiztonal, 0f));
            
            rb.velocity = transform.forward * speedOfFlight;

            if (!launchSound.isPlaying && !flightSound.isPlaying)
                flightSound.Play();

            // poop while flying
            //if(Input.GetKeyDown(KeyCode.R))
            if(Input.GetButtonDown("PoopDivebomb"))
            {
                // poop
                pickup.poop();
            }

            if(anim)
            {
                anim.SetFloat("Forward", 1.0f, 0.0f, Time.fixedDeltaTime); // maybe should be FixedDeltaTime? yes it should. Fixed
            }

        
	}

    public override void OnActivated()
    {
        // set up rigid body and controller to be good for flight
        euler_rotation = transform.eulerAngles;
        if (this.GetComponent<DogControllerV2>())
            this.GetComponent<DogControllerV2>().enabled = false;
        else if (this.GetComponent<ThirdPersonUserControl>())
            this.GetComponent<ThirdPersonUserControl>().enabled = false;
        rb.constraints = RigidbodyConstraints.None;
        this.transform.Rotate(new Vector3(-1f * takeoffAngle, 0f, 0f));
        col.material = flightMaterial;

        // set up trail and particle effect
        tr.enabled = true;
        ps.Play();

        // set up sounds for flight
        launchSound.Play();
        //swap music to flight music
        MusicManager.Instance.ChangeSong(1.0f, flightMusic);

        anim.SetBool("isFlying", true);
    }

    public override void OnDeactivated()
    {
        //float cur_y = transform.eulerAngles.y;
        euler_rotation.y = transform.eulerAngles.y;
        transform.eulerAngles = euler_rotation;
        this.GetComponent<Rigidbody>().velocity *= 0.5f;
        //this.GetComponent<DogControllerV2>().enabled = true;
        if (this.GetComponent<DogControllerV2>())
            this.GetComponent<DogControllerV2>().enabled = true;
        else if (this.GetComponent<ThirdPersonUserControl>())
            this.GetComponent<ThirdPersonUserControl>().enabled = true;
        rb.constraints = initial_rb_constratins;
        tr.enabled = false;
        ps.Stop();

        if(launchSound.isPlaying)
            launchSound.Stop();
        if(flightSound.isPlaying)
            flightSound.Stop();

        //return music back to default
        MusicManager.Instance.ChangeSong(1.0f, MusicManager.Instance.defaultTheme);

        anim.SetBool("isFlying", false);
    }
    */
}
