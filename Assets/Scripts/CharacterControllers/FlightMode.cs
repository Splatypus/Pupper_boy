using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class FlightMode : Controller {
    
    [SerializeField] private float speedOfFlight;
    [SerializeField] private float takeoffAngle;
    [SerializeField] private float tiltSensitivity;

    private Rigidbody rb;
    private TrailRenderer tr;
    private ParticleSystem ps;

    [SerializeField] private AudioSource BackgroundMusic;
    [SerializeField] private AudioSource launchSound;
    [SerializeField] private AudioSource flightSound;
    [SerializeField] private AudioSource flightMusic;

    BoxCollider col;
    [SerializeField] private PhysicMaterial flightMaterial;

    [SerializeField] private float fadeTime;
    [SerializeField] private float fadeBGTime;

    float BGMusicNormalVolume;

    private Coroutine fadeRoutine = null;
    private Coroutine BGFadeOutRoutine = null;
    private Coroutine BGFadeInRoutine = null;
    private float musicStartVolume;
    
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
        if(BackgroundMusic != null)
            BGMusicNormalVolume = BackgroundMusic.volume;

        tr.enabled = false;

        musicStartVolume = flightMusic.volume;

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
        if(flightMusic.isPlaying)
            flightMusic.Stop();
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
        flightMusic.volume = musicStartVolume;
        flightMusic.Play();

        // if bg music is currently fading in, stop fading it in
        if(BGFadeInRoutine != null)
        {
            StopCoroutine(BGFadeInRoutine);
            BGFadeInRoutine = null;
        }
        BGFadeOutRoutine = StartCoroutine(FadeBGOut());

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
        fadeRoutine = StartCoroutine(FadeMusic());
        BGFadeInRoutine = StartCoroutine(FadeBGIn());

        // if bg music is currently fading in, stop fading it out
        if (BGFadeOutRoutine != null)
        {
            StopCoroutine(BGFadeOutRoutine);
            BGFadeOutRoutine = null;
        }

        anim.SetBool("isFlying", false);
    }


    private IEnumerator FadeBGOut()
    {
        float startVolume = BackgroundMusic.volume;
        float startTime = Time.time;
        while (BackgroundMusic.volume > 0.01f)
        {
            float lerpAmount = 1f - ((Time.time - startTime) / fadeBGTime);
            BackgroundMusic.volume = startVolume * lerpAmount;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        BackgroundMusic.Pause();
        BGFadeOutRoutine = null;
    }

    private IEnumerator FadeBGIn()
    {
        float startVolume = BGMusicNormalVolume;
        float startTime = Time.time;
        BackgroundMusic.UnPause();
        while (BackgroundMusic.volume < 1.0f)
        {
            float lerpAmount = ((Time.time - startTime) / fadeBGTime);
            BackgroundMusic.volume = startVolume * lerpAmount;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BackgroundMusic.volume = 1.0f;
        BGFadeInRoutine = null;
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
