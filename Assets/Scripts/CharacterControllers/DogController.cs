
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; // for debug text

public class DogController : Controller {

    #region Component Variables
    CharacterController controller;
    Animator anim;
    Transform cam;
    PuppyPickup mouth;
    #endregion

    #region new world order movement variables
    public Vector3 v; //velocity
    [Header("Movement")]
    public float maxSpeed;
    public float sprintMultiplier;
    public float acceleration;
    public float decceleration;
    public float inAirMult;
    public float gravity;
    public float maxFallSpeed;
    public float hillSmoothingFactor;
    public float jumpForce;
    public float turnspeed = 5;
    public float freezeMovementAngle = 90.0f;
    public float SpeedMultScentMode = 0.5f;
    #endregion

    Vector3 cam_right, cam_fwd;

    public bool hasFlight = false;

    #region InteractionVariables
    [Header("Interaction")]
    public PuppyPickup ppickup; //reference to the mouth script cuz sometimes u need that in ur life

    List<InteractableObject> inRangeOf = new List<InteractableObject>();
    #endregion

    #region Digging
    [Header("Digging")]
    [SerializeField] AudioSource dig_sound;
    public float rotateSpeed = 10.0f;
    public float maxRotaionTime = 0.4f;

    IconManager my_icon;
    TextFadeOut houseText;
    bool isDigging = false;
    int digZoneCount = 0; //how many dig zones the player is currently in
    #endregion

    [HideInInspector]
    public EscMenuManager escMenu;


    void Start() {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        mouth = GetComponentInChildren<PuppyPickup>();
        anim = GetComponentInChildren<Animator>();
        my_icon = GetComponentInChildren<IconManager>();
        houseText = FindObjectOfType<TextFadeOut>();
        ppickup = GetComponentInChildren<PuppyPickup>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        v = Vector3.zero;
    }

    //move in fixed update since it changes velocity. In normal update it would sometimes feel like it had small delays.
    void FixedUpdate() {
        if (!isDigging) {
            Move();
        }
    }

    // Update is called once per frame
    void Update() {

        //Opening Esc Menu should always be available
        if (Input.GetButtonDown("Cancel")) {
            escMenu.Show();
            gameObject.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Dialog);
        }

        //dont do anything if digging, or in Esc Menu
        if (!isDigging) {

            //Move();

            //Handle interaction input
            if (Input.GetButtonDown("Dig") && inRangeOf.Count > 0) {

                //interact with the closest object
                InteractableObject closest = inRangeOf[0];
                float shortDis = Vector3.Distance(transform.position, closest.transform.position);
                foreach (InteractableObject i in inRangeOf) {
                    float dis = Vector3.Distance(transform.position, i.transform.position);
                    if (dis < shortDis) {
                        shortDis = dis;
                        closest = i;
                    }
                }
                closest.OnInteract();
            }

            //flight mode
            if (hasFlight && Input.GetButtonDown("Fly")) {
            //    gameObject.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Flight);
            }

            //scent mode toggle
            if (Input.GetButtonDown("Scent")) {
                ScentManager.Instance.InputEnable();
            } else if (Input.GetButtonUp("Scent")) {
                ScentManager.Instance.InputDisable();
            }

            if (Input.GetButtonDown("Interact")) {
                mouth.DoInputAction();
            }

        }//end isdigging check
    }

    void Move() {
        //grounded check before any movement
        bool isGrounded = controller.isGrounded;
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetButton("Jump");

        // Get information about the camera relative to us
        cam_right = Vector3.ProjectOnPlane(cam.right, transform.up) * horizontal;
        cam_fwd = Vector3.ProjectOnPlane(cam.forward, transform.up) * vertical;
        Vector3 moveDirection = (cam_right + cam_fwd).normalized; //new move direction. 
        //^On PC builds this should be normalized, since axies cannot be between 0 and 1, but can both be 1. Meaning you need to prevent faster diagonal movement
        //on console, this should be non-normalized, since the stick gives values between 0 and 1, but cannot put both at 1, and you may want <1 values, such as moving the stick half way left

        //find accelerations and max speeds
        float a = acceleration; //acceleration for this frame
        if (!isGrounded) {
            a *= inAirMult;
        }
        float newMaxSpeed = maxSpeed;
        if (Input.GetAxis("Sprint") > 0.1f) {
            newMaxSpeed = maxSpeed * sprintMultiplier;
        }
        if (ScentManager.Instance.isEnabled) {
            newMaxSpeed *= SpeedMultScentMode;
        }

        //find the direction doggo is supposed to move
        Vector3 dir = transform.position + (cam_right) + (cam_fwd);
        dir -= transform.position;
        dir.y = 0;
        //the angle of rotation between doggo and the desired movement 
        float angle = 0;
        if (dir.sqrMagnitude > Mathf.Epsilon) //angle is only nonzero if the movement direction exists
            angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));

        //reduce the speed if your angle is too far off
        if (angle > freezeMovementAngle && isGrounded)
            newMaxSpeed = 0;

        //store vertical movement speed before messing with horizontal, since the entire vector is changed
        float verticalSpeed = isGrounded? -hillSmoothingFactor/Time.deltaTime : v.y;

        v.y = 0;
        //find horizontal movement based on input
        if (Mathf.Abs(horizontal) > Mathf.Epsilon || Mathf.Abs(vertical) > Mathf.Epsilon) {
            //in air
            if (!isGrounded) {
                v += moveDirection * a * Time.fixedDeltaTime;
                //if this acceleration pushes it over the max speed, cap it there instead
                if (v.sqrMagnitude > newMaxSpeed * newMaxSpeed) {
                    v = v.normalized * maxSpeed;
                }
            }

            //acceleration only affects speed while on the ground. 
            else {
                v = moveDirection * Mathf.Min(v.magnitude + a * Time.fixedDeltaTime, newMaxSpeed);
            }
        }
        //no input
        else {
            //in air
            if (!isGrounded) {
                v = v.normalized * (v.magnitude - decceleration * Time.fixedDeltaTime);
            }
            //grouned
            else {
                v = Vector3.zero;
            }
        }


        v.y = verticalSpeed;//reset v.y after changes from horizontal movement

        //then calculate verticle speed
        //if grounded, vert speed is 0 unless jumping, if in air, calc gravity
        if (isGrounded && jumpInput) {
            v.y = jumpForce;
        } 
        //gravity
        if (v.y - (gravity * Time.fixedDeltaTime) < -maxFallSpeed) {
            v.y = -maxFallSpeed;
        } else {
            v.y -= gravity * Time.fixedDeltaTime;
        }

        //set movement
        controller.Move(v * Time.fixedDeltaTime);

        // Update animation controller with the amount that we are moving
        float animValue = Mathf.Sqrt(vertical * vertical + horizontal * horizontal);
        anim.SetFloat("Forward", animValue, 0.1f, Time.fixedDeltaTime);
        anim.SetBool("onAir", !isGrounded);

        //set turnspeed
        float ts = turnspeed;
        //if doggo is in the air, he turns slower
        if (!isGrounded) {
            ts *= inAirMult;
        }

        // Update player rotation if there is movement in any direction
        if (horizontal != 0 || vertical != 0) {
            if (angle > Mathf.Epsilon)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), (angle / 180.0f + 1.0f) * ts / angle * Time.fixedDeltaTime);
            //If the cameralock button is pressed, then look towards the camera vector
        } else if (Input.GetButton("CameraLock")) {
            //look vector directly through the doggo
            Quaternion newLookDirection = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.position - cam.transform.position, transform.up).normalized);
            angle = Quaternion.Angle(transform.rotation, newLookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newLookDirection, (angle / 180.0f + 1.0f) * ts / angle * Time.fixedDeltaTime);
        }
    }

    #region digging
    //digs through the given zone
    public void Dig(DigZone zone) {
        if (controller.isGrounded) {
            //Run digging coroutine. This does the animation and movement and eveything
            StartCoroutine(StartZoneDig(zone));
        }
    }

    //when entering a dig zone, enable sprite and digzone count
    public void DigZoneEnter() {
        if (digZoneCount == 0) {
            my_icon.set_single_icon(Icons.Dig);
            my_icon.set_single_bubble_active(true);
        }
        digZoneCount++;
    }

    //when leaving a dig zone, disable sprite if doggo is now in no zones
    public void DigZoneExit() {
        digZoneCount--;
        if (digZoneCount == 0) {
            my_icon.set_single_bubble_active(false);
        }
    }

    //moves the character to the next dig zone when digging
    private void Move_to_next_zone(DigZone digZone) {
        DigZone zone_to_go_to = digZone.other_side;


        //find out how far to move. This is done by assuming that one dig zone is a plane with a normal pointing towards the other zone
        //this makes it easy to find the distance from the character to that plane and move the player that far
        float dist_to_move;
        Vector3 plane = (digZone.transform.position - zone_to_go_to.transform.position).normalized;
        Vector3 toPlayer = transform.position - zone_to_go_to.transform.position;
        dist_to_move = Vector3.Dot(plane, toPlayer);
        //check terrain to that we dont teleport a dog into the ground. 
        Vector3 targetLocation = transform.position + ((zone_to_go_to.transform.position - digZone.transform.position).normalized * dist_to_move);
        int groundMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(targetLocation + new Vector3(0, 5.0f, 0), Vector3.down, out hit, 10.0f, groundMask)) {
            targetLocation += new Vector3(0, 5.0f - hit.distance, 0);
        }

        transform.position = targetLocation;
    }

    //starts dig animation
    IEnumerator StartZoneDig(DigZone digZone) {
        isDigging = true;
        if (ppickup.itemInMouth != null) {
            ppickup.itemInMouth.SetActive(false);
        }
        //rotate towards the fence
        float timeTaken = 0.0f;
        while (transform.rotation != Quaternion.LookRotation(digZone.other_side.transform.position - digZone.transform.position) && timeTaken < maxRotaionTime) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                            Quaternion.LookRotation(digZone.other_side.transform.position - digZone.transform.position),
                                                            rotateSpeed * Time.fixedDeltaTime);
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        //dig under it
        anim.SetTrigger("Dig");
        yield return new WaitForSeconds(0.6f);
        dig_sound.Play();
        Move_to_next_zone(digZone);
        anim.SetTrigger("Dig2");
        houseText.setText(digZone.other_side.GetYardName());
        //after the animation, restore movement
        yield return new WaitForSeconds(0.6f);
        isDigging = false;
        if (ppickup.itemInMouth != null) {
            ppickup.itemInMouth.SetActive(true);
        }

        //and then run the event trigger letting things know we have reached the other side
        EventManager.Instance.TriggerOnFenceDig(digZone.other_side.gameObject);
    }

    #endregion


    public override void OnDeactivated() {
        anim.SetFloat("Forward", 0.0f); //disable animations
        //rigidBody.velocity = Vector3.zero; //and stop it from moving
        v = Vector3.zero;
    }

    public override void OnActivated() {
        
    }

    //add object to things we can interact with
    public void AddObject(InteractableObject i) {
        inRangeOf.Add(i);
    }

    //remove object from list
    public void RemoveObject(InteractableObject i) {
        inRangeOf.Remove(i);
    }

}
