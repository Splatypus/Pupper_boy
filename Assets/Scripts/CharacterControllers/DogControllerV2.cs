using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; // for debug text

public class DogControllerV2 : Controller {

    #region Component Variables
    Rigidbody rigidBody;
    Animator anim;
    BoxCollider capCol;
    [SerializeField] PhysicMaterial zFriction;
    [SerializeField] PhysicMaterial mFriction;
    Transform cam;
    #endregion

    public Text debug_text;

    #region movement variables
    [SerializeField]
    float speed = 0.8f;
    [SerializeField]
    float turnspeed = 5;
    [SerializeField]
    float jumpPower = 1;
    #endregion

    Vector3 directionPos;
    Vector3 cam_right, cam_fwd;

    float horizontal;
    float vertical;
    bool jumpInput;
    bool onGround;

    float m_speed;

    #region InteractionVariables
    List<InteractableObject> inRangeOf = new List<InteractableObject>();
    #endregion

    #region Digging
    [SerializeField]
    private SphereCollider dig_look_zone;
    [SerializeField]
    private AudioSource dig_sound;
    public float rotateSpeed = 10.0f;
    public float maxRoationTime = 0.4f; 

    DigZone curZone;
    IconManager my_icon;
    TextFadeOut houseText;
    bool isDigging = false;
    #endregion

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        capCol = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<Animator>();
        my_icon = GetComponentInChildren<IconManager>();
        houseText = FindObjectOfType<TextFadeOut>();
    }
	
	// Update is called once per frame
	void Update () {
        
        HandleFriction();

        //dont do anything if digging
        if (!isDigging) {

            Move();
   
            //Handle interaction input
            if (Input.GetButtonDown("Interact")) {
                foreach (InteractableObject i in inRangeOf) {
                    i.OnInteract();
                }
            }

            //handle digging input
            if (Input.GetButtonDown("Dig")) {
                if (curZone != null) {
                    rigidBody.velocity = Vector3.zero;
                    if (curZone.isPathway) {
                        //this is to switch to a top camera when digging
                        //wait until digging animation is implemented or when digging is no longer instantanious
                        //GameObject.Find("Smart Cameras").GetComponent<cameracontrol>().isCam1 = false;

                        // for pathway
                        StartCoroutine(StartZoneDig(curZone));
                        
                    } else {
                        // for digging up object in yard
                        Instantiate(curZone.objectToDigUp, curZone.transform.position, Quaternion.identity);
                        // can give it some velocity and spin or whatever
                        curZone.gameObject.SetActive(false);
                    }
                }
            }//end dig input

        }//end isdigging check
    }//end update
    

    //void FixedUpdate()
    void Move()
    {
        // Get input from Unity's default input control system
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        // Check for sprint
        if (Input.GetAxis("Sprint") > float.Epsilon)
        {
            m_speed = speed * 1.75f;
        }
        else
        {
            m_speed = speed;
        }

        // Code for grounded movement
        if (onGround)
        {
            // Get information about the camera relative to us
            cam_right = Vector3.ProjectOnPlane(cam.right, transform.up);
            cam_fwd = Vector3.ProjectOnPlane(cam.forward, transform.up);
            
            // Compute new velocity relative to the camera and input
            Vector3 new_velocity = (((cam_right * horizontal) + (cam_fwd * vertical)) * m_speed);

            rigidBody.velocity =  new_velocity;

            if(jumpInput)
            {
                rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                onGround = false;
            }            
        }
        
        // Update animation controller with the amount that we are moving
        // There's a chance that we should be normalizing this, but I don't know well run animation blends
        float animValue = Mathf.Abs(vertical) + Mathf.Abs(horizontal);
        anim.SetFloat("Forward", animValue, 0.1f, Time.deltaTime);

        // Update player rotation
        if (horizontal != 0 || vertical != 0)
        {
            directionPos = transform.position + (cam_right * horizontal) + (cam_fwd * vertical);

            Vector3 dir = directionPos - transform.position;
            dir.y = 0;

            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));

            if (angle != 0)
                rigidBody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnspeed * Time.deltaTime);
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the thing we hit is the ground
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
            rigidBody.drag = 5;
            // currently, onAir is not used, but could be if we had an animation for jumping
            anim.SetBool("onAir", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the thing we left is the ground
        if (collision.gameObject.tag == "Ground")
        {
            onGround = false;
            rigidBody.drag = 0;
            // currently, onAir is not used, but could be if we had an animation for jumping
            anim.SetBool("onAir", true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        //if we enter a dig zone, set that up
        DigZone digZone = other.GetComponent<DigZone>();
        if (digZone != null) {
            print("digger entered into a trigger named " + other.name);
            curZone = digZone;
            my_icon.set_single_icon(Icons.Dig); // make this dig when dig is ready
            my_icon.set_single_bubble_active(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        //exiting dig zone
        DigZone digZone = other.GetComponent<DigZone>();
        if (digZone != null) {
            print("digger LEFT trigger " + other.name);
            curZone = null;
            my_icon.set_single_bubble_active(false);
        }

    }

    void HandleFriction()
    {
        // If there is no input, we set our physics material to have max friction
        if(horizontal == 0 && vertical == 0)
        {
            capCol.material = mFriction;
        }
        else
        {
            // If the player is moving, don't apply any friction
            capCol.material = zFriction;
        }
        
    }

    public override void OnDeactivated() {
        anim.SetFloat("Forward", 0.0f); //disable animations
        rigidBody.velocity = Vector3.zero; //and stop it from moving
    }

    //add object to things we can interact with
    public void addObject(InteractableObject i) {
        inRangeOf.Add(i);
    }


    //remove object from list
    public void removeObject(InteractableObject i) {
        inRangeOf.Remove(i);
    }

    //starts dig animation
    IEnumerator StartZoneDig(DigZone digZone) {
        isDigging = true;
        //rotate towards the fence
        float timeTaken = 0.0f;
        while ( transform.rotation != Quaternion.LookRotation(digZone.other_side.transform.position - digZone.transform.position) && timeTaken < maxRoationTime){
            transform.rotation = Quaternion.RotateTowards(  transform.rotation, 
                                                            Quaternion.LookRotation(digZone.other_side.transform.position - digZone.transform.position), 
                                                            rotateSpeed * Time.fixedDeltaTime);
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        //dig under it
        anim.SetTrigger("Dig");
        dig_sound.Play();
        yield return new WaitForSeconds(0.8f);
        move_to_next_zone(digZone);
        anim.SetTrigger("Dig2");
        houseText.setText(curZone.other_side.enteringYardName);
        //after the animation, restore movement
        yield return new WaitForSeconds(0.2f);
        isDigging = false;
    }

    //moves the character to the next dig zone when digging
    private void move_to_next_zone(DigZone digZone) {
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

}
