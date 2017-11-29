using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDig2 : MonoBehaviour {


    [Header("Dig Movement Constants")]
    public float sec1_rot_speed = 1.0f;
    public float sec1_fall_speed = 1.0f;
    public float sec1_duration = 4.0f;
    [Space(10)]

    float time_in_cur_state = 0.0f;

    [SerializeField]
    private SphereCollider dig_look_zone;
    [SerializeField]
    float extra_movement_for_dig;
    [SerializeField]
    float extra_movement_for_dig_y = 0.25f;
    [SerializeField] private AudioSource dig_sound;

    Animator anim;
    DigZone curZone;
    IconManager my_icon;

    bool amDigging = false;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        my_icon = GetComponentInChildren<IconManager>();
    }
    int cur_state = 0;
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
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
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
                    GetComponent<BoxCollider>().enabled = false;
                    GetComponent<Rigidbody>().useGravity = false;

                    //dig_sound.Play(); // re-enable this once the sound effect is real
                }

            }
        }
        
    }

    private void move_to_next_zone(DigZone digZone)
    {
        DigZone zone_to_go_to = digZone.other_side;

        print("I am at " + digZone + " going to " + zone_to_go_to);

        // eventually we will play some animation or something, this is just placeholder
        // the above comment also means that we likely will never actually change this behavior

        float dist_to_move;
        // we will move our extra 
        dist_to_move = (zone_to_go_to.transform.position - digZone.transform.position).magnitude + extra_movement_for_dig;

        transform.position += (zone_to_go_to.transform.position - digZone.transform.position).normalized * dist_to_move + new Vector3(0, extra_movement_for_dig_y, 0);
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
