using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDig2 : MonoBehaviour {


    [Header("Dig Movement Constants")]
    public float sec1_rot_speed = 1.0f;
    public float sec1_fall_speed = 1.0f;
    public float sec1_duration = 4.0f;

    public float sec2_rot_speed = 1.0f;
    public float sec2_rise_speed = 1.0f;
    [Space(10)]

    float time_in_cur_state = 0.0f;


    [SerializeField]
    float extra_movement_for_dig;
    [SerializeField] private AudioSource dig_sound;

    Animator anim;
    DigZone curZone;
    DigZone other_side_anim;
    IconManager my_icon;
    BoxCollider col;

    bool amDigging = false;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        my_icon = GetComponentInChildren<IconManager>();
        col = GetComponent<BoxCollider>();
    }


    int cur_state = 0;
    float t = 0.0f;


    private void Update()
    {
        if(amDigging)
        {
            if (cur_state == 0)
            {
                //first, rotate the player forward and move them down
                transform.eulerAngles = transform.eulerAngles + new Vector3(sec1_rot_speed * Time.deltaTime, 0, 0);
                transform.position = transform.position + new Vector3(0, sec1_fall_speed * -Time.deltaTime, 0);
                time_in_cur_state += Time.deltaTime;
                if (time_in_cur_state >= sec1_duration)
                {
                    time_in_cur_state = 0.0f;
                    cur_state++;

                    // move along
                    move_to_next_zone(other_side_anim);

                    // rotate up
                    //transform.forward = Vector3.up;
                    Vector3 new_angle = transform.eulerAngles;
                    new_angle.x = -90;
                    transform.eulerAngles = new_angle;
                    t = 0.0f;
                }
            }
            else if (cur_state == 1)
            {
                if (Mathf.Abs(transform.eulerAngles.x) > 0.1f)
                {
                    t += Time.deltaTime;
                    float x_rot = Mathf.Lerp(-90, 0, t * sec2_rot_speed);
                    Vector3 new_angle = transform.eulerAngles;
                    new_angle.x = x_rot;
                    transform.eulerAngles = new_angle;
                }
                else
                {
                    anim.SetBool("isDigging", false);
                }
                transform.position = transform.position + new Vector3(0, sec2_rise_speed * Time.deltaTime, 0);
                //if(Physics.BoxCast(col.center, col.extents, )
                int layermask = 1 << LayerMask.NameToLayer("Ground");

                Collider[] cols = Physics.OverlapBox(col.center, col.size, transform.rotation, layermask, QueryTriggerInteraction.Ignore);

                if(!Physics.CheckBox(col.center + transform.position, col.size, transform.rotation, layermask, QueryTriggerInteraction.Ignore))
                {
                    //cur_state++;
                    amDigging = false;
                    
                    col.enabled = true;
                    GetComponent<Rigidbody>().useGravity = true;
                    FindObjectOfType<DogControllerV2>().enabled = true;
                }
                
                
            }
        }
        else
        {
            //if (Input.GetKeyDown(KeyCode.Q))
            if(Input.GetButtonDown("Dig"))
            {
                if (curZone != null)
                {
                    // stop moving
                    Rigidbody rb = GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;

                    // disable player controller
                    FindObjectOfType<DogControllerV2>().enabled = false;

                    anim.SetBool("isDigging", true);

                    amDigging = true;
                    // disable collider and gravity
                    col.enabled = false;
                    rb.useGravity = false;

                    other_side_anim = curZone;
                    cur_state = 0;

                    //dig_sound.Play(); // re-enable this once the sound effect is real
                }

            }
        }
        
    }

    private void move_to_next_zone(DigZone digZone)
    {
        DigZone zone_to_go_to = digZone.other_side;

        //print("I am at " + digZone + " going to " + zone_to_go_to);

        // eventually we will play some animation or something, this is just placeholder
        // the above comment also means that we likely will never actually change this behavior

        float dist_to_move;
        // we will move our extra 
        dist_to_move = (zone_to_go_to.transform.position - digZone.transform.position).magnitude + extra_movement_for_dig;

        transform.position += (zone_to_go_to.transform.position - digZone.transform.position).normalized * dist_to_move;
    }

    private void OnTriggerEnter(Collider other)
    {
        DigZone digZone = other.GetComponent<DigZone>();
        if (digZone != null)
        {
            curZone = digZone;
            my_icon.set_single_icon(Icons.Exclamation); // make this dig when dig is ready
            my_icon.set_single_bubble_active(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        DigZone digZone = other.GetComponent<DigZone>();
        if (digZone != null)
        {
            curZone = null;
            my_icon.set_single_bubble_active(false);
        }

    }
}
