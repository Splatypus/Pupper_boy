using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public bool play_sound = false;

    public AudioClip[] pickup_sounds;
    public AudioClip[] drop_sounds;

    AudioSource m_source;
    
    public void onPickup()
    {
        if(play_sound)
        {
            if (!m_source)
                m_source = GetComponent<AudioSource>();

            if(m_source)
            {
                int sound_index = Random.Range(0, pickup_sounds.Length);
                m_source.clip = pickup_sounds[sound_index];
                m_source.Play();
            }
            
        }
    }

    public void onDrop()
    {
        if (play_sound)
        {
            if (!m_source)
                m_source = GetComponent<AudioSource>();

            if (m_source)
            {
                int sound_index = Random.Range(0, drop_sounds.Length);
                m_source.clip = drop_sounds[sound_index];
                m_source.Play();
                m_source.Play();
            }

        }
    }
}
