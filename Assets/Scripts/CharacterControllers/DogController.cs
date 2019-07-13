
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; // for debug text

public class DogController : Controller {

    #region Component Variables
    CharacterController controller;
    PlayerControllerManager manager;
    Animator anim;
    Transform cam;
    FreeCameraLook cameraScript;
    [HideInInspector] public PuppyPickup mouth;
    #endregion

    #region new world order movement variables
    [Header("Movement")]
    public float maxSpeed;
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
    [Header("Sprinting")]
    public float sprintMultiplier;
    public float sprintDelay;
    public float sprintRampTime;
    private float moveStartTime;

    #endregion

    Vector3 cam_right, cam_fwd;

    //public bool hasFlight = false;

    List<InteractableObject> inRangeOf = new List<InteractableObject>();

    #region Digging
    [Header("Digging")]
    [SerializeField] AudioSource dig_sound;
    public float rotateSpeed = 10.0f;
    public float maxRotaionTime = 0.4f;
    public float distanceAddedToDig = 1.5f;

    IconManager my_icon;
    TextFadeOut houseText;
    bool isDigging = false;
    DigZone currentDiggingZone;
    int digZoneCount = 0; //how many dig zones the player is currently in
    #endregion

    [HideInInspector]
    public EscMenuManager escMenu;


    void Start() {
        manager = GetComponent<PlayerControllerManager>();
        controller = GetComponent<CharacterController>();
        cameraScript = Camera.main.GetComponent<FreeCameraLook>();
        cam = cameraScript.phantomCamera.transform; //Camera.main.transform;
        mouth = GetComponentInChildren<PuppyPickup>();
        anim = GetComponentInChildren<Animator>();
        my_icon = GetComponentInChildren<IconManager>();
        houseText = FindObjectOfType<TextFadeOut>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentDiggingZone = null;

        manager.v = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {
        
        //Opening Esc Menu should always be available
        if (Input.GetButtonDown("Cancel")) {
            escMenu.Show();
            gameObject.GetComponent<PlayerControllerManager>().ChangeMode(PlayerControllerManager.Modes.Pause);
        }

        //dont do anything if digging, or in Esc Menu
        if (!isDigging) {

            Move();

            //Handle interaction input
            if (Input.GetButtonDown("Dig")) {
                //if something is close by to interact with, do that
                if (inRangeOf.Count > 0) {
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

                    //if nothing is close by to interact with, check the mouth instead
                } else if (mouth.itemInMouth != null) {
                    ItemDialog dialog = mouth.itemInMouth.GetComponent<ItemDialog>();
                    if (dialog != null) {
                        dialog.OnInteract();
                    }
                }
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

        //move camera
        float speed = Vector3.Dot(Vector3.ProjectOnPlane(manager.v, Vector3.up), cameraScript.transform.forward);
        cameraScript.trueMaxDistance = Mathf.Lerp(cameraScript.trueMaxDistance, cameraScript.maxDistance + speed / 5.0f, 8.0f * Time.deltaTime);

    }

    void Move() {
        //grounded check before any movement
        bool isGrounded = controller.isGrounded;
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jumpInput = Input.GetButtonDown("Jump");

        // Get information about the camera relative to us
        cam_right = Vector3.ProjectOnPlane(cam.right, transform.up) * horizontal;
        cam_fwd = Vector3.ProjectOnPlane(cam.forward, transform.up) * vertical;
        Vector3 moveDirection = (cam_right + cam_fwd).normalized; //new move direction. 
        //^On PC builds this should be normalized, since axies cannot be between 0 and 1, but can both be 1. Meaning you need to prevent faster diagonal movement
        //on console, this should be non-normalized, since the stick gives values between 0 and 1, but cannot put both at 1, and you may want <1 values, such as moving the stick half way left

        //Handle Sprinting
        if (horizontal == 0 && vertical == 0) {
            moveStartTime = Time.time;
        }
        if (Input.GetAxis("Sprint") > 0.1f) { //if sprinting key is held, jump to max sprint
            moveStartTime = -(sprintDelay + sprintRampTime);
        }

        //find accelerations and max speeds
        float a = acceleration; //acceleration for this frame
        if (!isGrounded) {
            a *= inAirMult;
        }
        float newMaxSpeed = maxSpeed;
        if (Time.time - moveStartTime > sprintDelay) {
            newMaxSpeed *= Mathf.Lerp(1, sprintMultiplier, (Time.time - (moveStartTime + sprintDelay)) / sprintRampTime); //adjust max speed to account for sprinting
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
        if (angle > freezeMovementAngle && isGrounded) {
            moveStartTime = Time.time - sprintDelay;
            newMaxSpeed = 0;
        }

        //store vertical movement speed before messing with horizontal, since the entire vector is changed
        float verticalSpeed = isGrounded ? -hillSmoothingFactor / Time.deltaTime : manager.v.y;

        manager.v.y = 0;
        //find horizontal movement based on input
        if (Mathf.Abs(horizontal) > Mathf.Epsilon || Mathf.Abs(vertical) > Mathf.Epsilon) {
            //in air
            if (!isGrounded) {
                manager.v += moveDirection * a * Time.deltaTime;
                //if this acceleration pushes it over the max speed, cap it there instead
                if (manager.v.sqrMagnitude > newMaxSpeed * newMaxSpeed) {
                    manager.v = manager.v.normalized * maxSpeed;
                }
            }

            //acceleration only affects speed while on the ground. 
            else {
                manager.v = moveDirection * Mathf.Min(manager.v.magnitude + a * Time.deltaTime, newMaxSpeed);
            }
        }
        //no input
        else {
            //in air
            if (!isGrounded) {
                manager.v = manager.v.normalized * (manager.v.magnitude - decceleration * Time.deltaTime);
            }
            //grouned
            else {
                manager.v = Vector3.zero;
            }
        }


        manager.v.y = verticalSpeed;//reset v.y after changes from horizontal movement

        //then calculate verticle speed
        //if grounded, vert speed is 0 unless jumping, if in air, calc gravity
        if (isGrounded && jumpInput) {
            manager.v.y = jumpForce;
        }
        //gravity
        if (manager.v.y - (gravity * Time.deltaTime) < -maxFallSpeed) {
            manager.v.y = -maxFallSpeed;
        } else {
            manager.v.y -= gravity * Time.deltaTime;
        }

        //set movement
        controller.Move(manager.v * Time.deltaTime);

        // Update animation controller with the amount that we are moving
        float animValue = new Vector2(manager.v.x, manager.v.z).magnitude / maxSpeed;
        anim.SetFloat("Forward", animValue);
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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), (angle / 180.0f + 1.0f) * ts / angle * Time.deltaTime);
            //If the cameralock button is pressed, then look towards the camera vector
        } else if (Input.GetButton("CameraLock")) {
            //look vector directly through the doggo
            Quaternion newLookDirection = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.position - cam.transform.position, transform.up).normalized);
            angle = Quaternion.Angle(transform.rotation, newLookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newLookDirection, (angle / 180.0f + 1.0f) * ts / angle * Time.deltaTime);
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
        dist_to_move = Vector3.Dot(plane, toPlayer) + distanceAddedToDig;
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
        currentDiggingZone = digZone;
        isDigging = true;

        //hide item in mouth to prevent weird collisions
        if (mouth.itemInMouth != null)
            mouth.itemInMouth?.SetActive(false);

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
        anim.SetTrigger("digIsPressed");
        anim.SetBool("diggingUnder", true);
        
    }

    public void PlayDigSound() {
        dig_sound.Play();
    }

    //called after the animation to dig under the fence is finished
    public void AfterDugDown() {

        Move_to_next_zone(currentDiggingZone);
        houseText.setText(currentDiggingZone.other_side.GetYardName());
        
    }

    //called by the animation when the dig finishes
    public void FinishDig() {
        isDigging = false;
        anim.SetBool("diggingUnder", false);
        if (mouth.itemInMouth != null)
            mouth.itemInMouth.SetActive(true);
        
        //and then run the event trigger letting things know we have reached the other side
        EventManager.Instance.TriggerOnFenceDig(currentDiggingZone.other_side.gameObject);
        currentDiggingZone = null;
    }

    #endregion
    

    public override void OnDeactivated() {
        anim.SetFloat("Forward", 0.0f); //disable animations
        //rigidBody.velocity = Vector3.zero; //and stop it from moving
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
