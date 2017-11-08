using System.Collections;
using UnityEngine;

using UnityEngine.UI; // for debug text

public class DogControllerV2 : MonoBehaviour {

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
    

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        capCol = GetComponent<BoxCollider>();
        SetupAnimatior();
    }
	
	// Update is called once per frame
	void Update () {
        HandleFriction();

        move();
	}

    //void FixedUpdate()
    void move()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_speed = speed * 1.75f;
        }
        else
        {
            m_speed = speed;
        }


        //if (onGround)
        // need to do stuff in here otherwise it doesn't work??
        //if (true)
        if (onGround)
        {
            cam_right = Vector3.ProjectOnPlane(cam.right, transform.up);
            cam_fwd = Vector3.ProjectOnPlane(cam.forward, transform.up);
            
            //rigidBody.AddForce(((cam_right * horizontal) + (cam_fwd * vertical)) * m_speed * Time.deltaTime);
            Vector3 new_velocity = (((cam_right * horizontal) + (cam_fwd * vertical)) * m_speed * Time.deltaTime);

            rigidBody.velocity =  new_velocity; //(((cam_right * horizontal) + (cam_fwd * vertical)) * m_speed * Time.deltaTime);
            
        }
        
        float animValue = Mathf.Abs(vertical) + Mathf.Abs(horizontal);
        anim.SetFloat("Forward", animValue, 0.1f, Time.deltaTime); // maybe should be FixedDeltaTime?
        
        if(horizontal != 0 || vertical != 0)
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
        // TODO: eventually make sure that you are actually on the ground and not just hitting anything. This only works on flatworld
        // just need to mark the floor as the floor and I think that we are good!
        //if(collision.gameObject.tag == "Floor")
        onGround = true;
        rigidBody.drag = 5;
        anim.SetBool("onAir", false);
        //debug_text.text = "grounded";
    }

    private void OnCollisionExit(Collision collision)
    {
        //if(collision.gameObject.tag == "Floor")
        onGround = false;
        rigidBody.drag = 0;
        anim.SetBool("onAir", true);
        //debug_text.text = "flying";
    }
    

    
    void HandleFriction()
    {
        
        // if there's no input
        if(horizontal == 0 && vertical == 0)
        {
            capCol.material = mFriction;
        }
        else
        {
            capCol.material = zFriction;
        }
        
    }
    

    void SetupAnimatior()
    {
        anim = GetComponentInChildren<Animator>();
    }
}
