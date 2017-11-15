using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentPickupObject : MonoBehaviour {

    ScentObject cur_scent;
    public float time_to_smell;
    public float smell_pickup_range = 1.0f;

    float m_smell_time = 0.0f;


    // Update is called once per frame
    void Update () {
        int layer_mask = 1 << 8; // this is the layermask for the smelling things
        Collider[] cols = Physics.OverlapSphere(transform.position, smell_pickup_range, layer_mask);

        // for now assume that there will never be more than 1 scent thing in an area
        if (cols.Length > 0)
        {
            ScentObject scent;
            scent = cols[0].gameObject.GetComponent<ScentHolder>().get_scent();
            if(cur_scent != scent)
            {
                m_smell_time += Time.deltaTime;
                if (m_smell_time > time_to_smell)
                {
                    cur_scent = scent;
                    print("I have a new smell now. it is named " + cur_scent.smell_name);
                    m_smell_time = 0.0f;
                }
            }
            
        }
        else
        {
            m_smell_time = 0.0f;
        }

	}
}
