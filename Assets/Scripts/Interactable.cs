using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public bool play_sound = false;
    AudioSource m_source;
    
    public void onPickup()
    {
        if(play_sound)
        {
            if (!m_source)
                m_source = GetComponent<AudioSource>();

            if(m_source)
            {
                m_source.Play();
            }
            
        }
    }

    public void onDrop()
    {

    }
}
