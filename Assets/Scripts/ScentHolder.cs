using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentHolder : MonoBehaviour {

    // current scent
    public ScentObject cur_scent;

    ParticleSystem m_particle_system;
    // Use this for initialization
    void Start () {
        update_particle_system();
    }
	
    void update_particle_system()
    {
        if (cur_scent == null)
        {
            print("error: scent is currently null");
            return;
        }

        if(m_particle_system == null)
            m_particle_system = GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule m_module = m_particle_system.main;
        
        Color c = cur_scent.particle_color;
        m_module.startColor = c;
    }

    public ScentObject get_scent()
    {
        return cur_scent;
    }
}
