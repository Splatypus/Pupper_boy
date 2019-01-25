using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public bool play_sound = false;

    public AudioClip[] pickup_sounds;
    public AudioClip[] drop_sounds;

    public enum Tag {Ball, Smell, TiffyQuestItem, Soap, RexQuestItem, SocksQuestItem};
    public List<Tag> tagList = new List<Tag>();

    [HideInInspector]
    public bool isCurrentlyHeld = false;

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


    //called by the player character when it picks up this item. Plays a sound and notifies the event manager
    public virtual void onPickup()
    {
        EventManager.Instance.TriggerOnItemPickup(gameObject); //tells the event manager that this item has been picked up to trigger any effects
        isCurrentlyHeld = true;
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

    //called by the player when it drops this item. Plays a sound and notifies the event manager
    public virtual void onDrop()
    {
        EventManager.Instance.TriggerOnItemDrop(gameObject);
        isCurrentlyHeld = false;
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
