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

    [SerializeField]
    float speed = 0.8f;
    [SerializeField]
    float turnspeed = 5;
    [SerializeField]
    float jumpPower = 1;

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

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        capCol = GetComponent<BoxCollider>();
        SetupAnimatior();
    }
	
	// Update is called once per frame
	void Update () {
        HandleFriction();

        // If it is on the screen, print to the debug text if we are grounded
        if(debug_text)
        {
            debug_text.text = "onGround: " + onGround;
        }

        Move();

        //Handle interaction input
        if (Input.GetButtonDown("Interact")) {
            foreach (InteractableObject i in inRangeOf) {
                i.OnInteract();
            }
        }

    }
    

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

    void SetupAnimatior()
    {
        anim = GetComponentInChildren<Animator>();
    }

    //add object to things we can interact with
    public void addObject(InteractableObject i) {
        inRangeOf.Add(i);
    }


    //remove object from list
    public void removeObject(InteractableObject i) {
        inRangeOf.Remove(i);
    }
}
