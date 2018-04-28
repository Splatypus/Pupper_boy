using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public bool play_sound = false;

    public AudioClip[] pickup_sounds;
    public AudioClip[] drop_sounds;

    public enum Tag {Ball, Smell, TiffyQuestItem, Soap};
    public List<Tag> tagList = new List<Tag>();

    AudioSource m_source;

    //checks if this item has tag t, returns true if it does
    public bool hasTag(Tag t) {
        return tagList.Contains(t);
    }

    //adds a tag to this items list of tags
    public void addTag(Tag t) {
        if (!hasTag(t)) {
            tagList.Add(t);
        }
    }

    //removes tag
    public void removeTag(Tag t) {
        tagList.Remove(t);
    }



    public virtual void onPickup()
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

    public virtual void onDrop()
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
