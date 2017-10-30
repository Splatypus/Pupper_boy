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

    [SerializeField] private AudioSource launchSound;
    [SerializeField] private AudioSource flightSound;
    [SerializeField] private AudioSource flightMusic;

    [SerializeField] private float fadeTime;
    private Coroutine fadeRoutine = null;
    private float musicStartVolume;

    public bool can_fly = false;
    private PuppyPickup pickup;
    private Vector3 euler_rotation;
    RigidbodyConstraints initial_rb_constratins;

    Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        tr = this.GetComponent<TrailRenderer>();
        ps = this.GetComponent<ParticleSystem>();

        tr.enabled = false;

        musicStartVolume = flightMusic.volume;

        pickup = FindObjectOfType<PuppyPickup>();

        initial_rb_constratins = rb.constraints;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!can_fly)
            return;
		if(isFlying)
        {
            float vertical = Input.GetAxis("Vertical") * tiltSensitivity;
            float horiztonal = Input.GetAxis("Horizontal") * tiltSensitivity;

            this.transform.Rotate(new Vector3(-1f * vertical, horiztonal, 0f));
            
            rb.velocity = transform.forward * speedOfFlight;

            if (!launchSound.isPlaying && !flightSound.isPlaying)
                flightSound.Play();

            // poop while flying
            if(Input.GetKeyDown(KeyCode.R))
            {
                // poop
                pickup.poop();
            }

            if(anim)
            {
                anim.SetFloat("Forward", 1.0f, 0.0f, Time.deltaTime); // maybe should be FixedDeltaTime?
            }
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
        euler_rotation = transform.eulerAngles;
        this.GetComponent<Rigidbody>().useGravity = false;
        if (this.GetComponent<DogControllerV2>())
            this.GetComponent<DogControllerV2>().enabled = false;
        else if (this.GetComponent<ThirdPersonUserControl>())
            this.GetComponent<ThirdPersonUserControl>().enabled = false;
        rb.constraints = RigidbodyConstraints.None;
        tr.enabled = true;
        ps.Play();
        this.transform.Rotate(new Vector3(-1f * takeoffAngle, 0f, 0f));
        isFlying = true;

        launchSound.Play();
        if(flightMusic.isPlaying)
            flightMusic.Stop();
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
        flightMusic.volume = musicStartVolume;
        flightMusic.Play();
    }

    public void DeactivateFlightMode()
    {
        //float cur_y = transform.eulerAngles.y;
        euler_rotation.y = transform.eulerAngles.y;
        transform.eulerAngles = euler_rotation;
        this.GetComponent<Rigidbody>().useGravity = true;
        //this.GetComponent<DogControllerV2>().enabled = true;
        if (this.GetComponent<DogControllerV2>())
            this.GetComponent<DogControllerV2>().enabled = true;
        else if (this.GetComponent<ThirdPersonUserControl>())
            this.GetComponent<ThirdPersonUserControl>().enabled = true;
        rb.constraints = initial_rb_constratins;
        tr.enabled = false;
        ps.Stop();
        isFlying = false;

        if(launchSound.isPlaying)
            launchSound.Stop();
        if(flightSound.isPlaying)
            flightSound.Stop();
        fadeRoutine = StartCoroutine(FadeMusic());
    }

    public bool IsFlying()
    {
        return isFlying;
    }

    private IEnumerator FadeMusic()
    {
        float startVolume = flightMusic.volume;
        float startTime = Time.time;
        while (flightMusic.volume > 0.01f)
        {
            float lerpAmount = 1f - ((Time.time - startTime) / fadeTime);
            flightMusic.volume = startVolume * lerpAmount;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        flightMusic.Stop();
        fadeRoutine = null;
    }
}
