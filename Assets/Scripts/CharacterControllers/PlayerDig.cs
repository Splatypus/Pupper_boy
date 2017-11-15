using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDig : MonoBehaviour {

    [SerializeField]
    private SphereCollider dig_look_zone;
    [SerializeField]
    float extra_movement_for_dig;
    [SerializeField] private AudioSource dig_sound;

    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            //print("got q!");
            

            if(dig_look_zone)
            {
                //print("have a place to look!");

                Collider[] inGrabber = Physics.OverlapSphere(dig_look_zone.transform.position, dig_look_zone.radius * 0.5f);

                foreach( Collider c in inGrabber)
                {
                    DigZone digZone = c.GetComponent<DigZone>();
                    if(digZone)
                    {
                        //print("got me some diggle zoner " + digZone);
                        anim.SetTrigger("Dig");
                        move_to_next_zone(digZone);
                        dig_sound.Play();
                        return; // probably should not do this. xd
                    }
                    
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

        transform.position += (zone_to_go_to.transform.position - digZone.transform.position).normalized * dist_to_move;
    }
}