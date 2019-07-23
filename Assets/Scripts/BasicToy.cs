using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicToy : MonoBehaviour, PuppyPickup.IPickupItem {

    [Header("Audio")]
    public bool play_sound = false;
    public AudioClip[] pickup_sounds;
    public AudioClip[] drop_sounds;
    AudioSource m_source;

    [Header("Visual")]
    public MeshRenderer[] meshes;

    public enum Tag {Ball, Smell, TiffyQuestItem, Soap, RexQuestItem, SocksQuestItem, Shiny};
    [Header("Misc")]
    public List<Tag> tagList = new List<Tag>();

    [HideInInspector]
    public bool isCurrentlyHeld = false;

    //checks if this item has tag t, returns true if it does
    public bool HasTag(Tag t) {
        return tagList.Contains(t);
    }

    //adds a tag to this items list of tags
    public void AddTag(Tag t) {
        if (!HasTag(t)) {
            tagList.Add(t);
        }
    }

    //removes tag
    public void RemoveTag(Tag t) {
        tagList.Remove(t);
    }

    #region interface methods
    public void OnFocus() {
        foreach (MeshRenderer r in meshes) {
            r.material.SetFloat("_IsFocused", 1);
        }
    }

    public void OnDefocus() {
        foreach (MeshRenderer r in meshes) {
            r.material.SetFloat("_IsFocused", 0);
        }

    }

    public void OnPickup(PuppyPickup source) {
        //trigger events
        EventManager.Instance.TriggerOnItemPickup(gameObject);
        //run sounds
        isCurrentlyHeld = true;
        if (play_sound) {
            if (!m_source)
                m_source = GetComponent<AudioSource>();

            if (m_source) {
                int sound_index = Random.Range(0, pickup_sounds.Length);
                m_source.clip = pickup_sounds[sound_index];
                m_source.Play();
            }
        }
        //attach to mouth
        source.itemInMouth = gameObject;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.detectCollisions = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.parent = source.mouth;
        transform.localPosition = Vector3.zero;
        transform.rotation = source.mouth.rotation;

    }

    public void OnDrop(Vector3 currentVelocity) {
        //trigger events
        EventManager.Instance.TriggerOnItemDrop(gameObject);

        //initial velocity
        GetComponent<Rigidbody>().velocity = currentVelocity;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().detectCollisions = true;

        isCurrentlyHeld = false;
        if (play_sound) {
            if (!m_source)
                m_source = GetComponent<AudioSource>();

            if (m_source) {
                int sound_index = Random.Range(0, drop_sounds.Length);
                m_source.clip = drop_sounds[sound_index];
                m_source.Play();
                m_source.Play();
            }
        }
    }
    #endregion
}
