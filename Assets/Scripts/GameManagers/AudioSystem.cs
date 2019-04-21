using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem
{
    public static AudioManager audioManager;
    public static AudioMixer masterMixer;
    public static AudioMixerGroup masterGroup;
    public static AudioMixerGroup musicGroup;
    public static AudioMixerGroup sfxGroup;
    public static AudioMixerGroup ambientGroup;

    public static void PlayAudioClip(AudioClip clip, AudioMixerGroup groupToPlayOn)
    {
        AudioSource sourceToPlayOn = AudioManager.GetAudioSource();
        sourceToPlayOn.outputAudioMixerGroup = groupToPlayOn;
        sourceToPlayOn.clip = clip;

        sourceToPlayOn.Play();
    }

    public static void ChangeMixerVolume(string mixKey, float value)
    {
        audioManager.masterMixer.SetFloat(mixKey, Mathf.Log10(value) * 33);
    }

}