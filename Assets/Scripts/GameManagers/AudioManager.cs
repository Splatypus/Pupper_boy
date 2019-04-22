using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    //Main AudioMixer
    public AudioMixer masterMixer;
    static AudioManager me;
    //public AudioClip testClip;

    public static List<AudioSource> alllAudioSources = new List<AudioSource>();

    private void Awake()
    {
        if (me == null)
        {
            me = this;
            DontDestroyOnLoad(me);
        }
        else
        {
            if(me != this)
            {
                Destroy(this);
            }
        }
    }

    private void Start()
    {
        AudioSystem.audioManager = this;

        AudioSystem.masterMixer = masterMixer;
        AudioSystem.masterGroup = masterMixer.FindMatchingGroups("Master")[0];
        AudioSystem.musicGroup = masterMixer.FindMatchingGroups("Music")[0];
        AudioSystem.sfxGroup = masterMixer.FindMatchingGroups("SFX")[0];
        AudioSystem.ambientGroup = masterMixer.FindMatchingGroups("Ambient")[0];

        //Invoke("TestPlay", 1);
        //Invoke("TestPlay", 1.1f);
        //Invoke("TestPlay", 1.62f);
        //Invoke("TestPlay", 1.3f);
        //Invoke("TestPlay", 1.5f);
        //Invoke("TestPlay", 2.1f);
        //Invoke("TestPlay", 2);
        //Invoke("TestPlay", 4);
        //Invoke("TestPlay", 3.5f);
        //        TestPlay
        //Invoke("TestPlay", 6);
        //Invoke("TestPlay", 6.1f);
        //Invoke("TestPlay", 6.62f);
        //Invoke("TestPlay", 6.3f);
        //Invoke("TestPlay", 6.5f);
        //Invoke("TestPlay", 6.1f);
        //Invoke("TestPlay", 6);
        //Invoke("TestPlay", 6);
        //Invoke("TestPlay", 6.5f);
    }

    void TestPlay()
    {
        //AudioSystem.PlayAudioClip(testClip, AudioSystem.sfxGroup);
    }

    public static AudioSource GetAudioSource()
    {
        foreach (AudioSource aud in alllAudioSources)
        {
            if (!aud.isPlaying)
                return aud;
        }
        print("asdhjr");

        GameObject newObj = Instantiate(new GameObject("Audio System Managed - Audio Source"));
        newObj.transform.SetParent(me.gameObject.transform);

        AudioSource newSrc = newObj.AddComponent<AudioSource>();
        newSrc.playOnAwake = false;

        alllAudioSources.Add(newSrc);

        return newSrc;
    }
}