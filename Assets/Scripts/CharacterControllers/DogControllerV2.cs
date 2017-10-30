using System.Collections;
using UnityEngine;

public class DogControllerV2 : MonoBehaviour {

    #region Component Variables
    Rigidbody rigidBody;
    Animator anim;
    CapsuleCollider capCol;
    [SerializeField] PhysicMaterial zFriction;
    [SerializeField] PhysicMaterial mFriction;
    Transform cam;
    #endregion

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
    

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        capCol = GetComponent<CapsuleCollider>();
        SetupAnimatior();
    }
	
	// Update is called once per frame
	void Update () {
        HandleFriction();
	}

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        

        if (onGround)
        {
            
            //cam_right = cam.right;
            cam_right = Vector3.ProjectOnPlane(cam.right, transform.up);
            //cam_fwd = cam.forward;
            cam_fwd = Vector3.ProjectOnPlane(cam.forward, transform.up);
            rigidBody.AddForce(((cam_right * horizontal) + (cam_fwd * vertical)) * speed / Time.deltaTime);

            // TODO: make jump work (oops)
            // I think after I set up the jump animations I will work on this
            /*
            if(jumpInput)
            {
                anim.SetTrigger("Jump");
                rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
            */
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
    }

    private void OnCollisionExit(Collision collision)
    {
        //if(collision.gameObject.tag == "Floor")
        onGround = false;
        rigidBody.drag = 0;
        anim.SetBool("onAir", true);
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
        anim = GetComponent<Animator>();

        // allow us to get the avatar from our child is what I think this is doing
        // something about making swaping models from children easier?
        foreach(var childAnimator in GetComponentsInChildren<Animator>())
        {
            if(childAnimator != anim)
            {
                anim.avatar = childAnimator.avatar;
                Destroy(childAnimator); // why?
                break;
            }
        }
    }
}
