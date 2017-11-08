using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentHolder : MonoBehaviour {

    // want to have something that holds what scent this object has
    //  this thing needs to be able to be copied over to other smelly objects
    ParticleSystem m_particle_system;
    ParticleSystem.MainModule m_module;

	// Use this for initialization
	void Start () {
        m_particle_system = GetComponentInChildren<ParticleSystem>();
        m_module = m_particle_system.main;
        //m_particle_system.col
        m_module.startColor = Color.cyan;
        //m_particle_system.main = m_module;
        //m_particle_system.startColor = Color.cyan;

        //m_particle_system.main.startColor = Color.cyan;
        //m_particle_system.emit
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
