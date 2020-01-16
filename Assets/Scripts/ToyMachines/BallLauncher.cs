using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallLauncher : MonoBehaviour {

    [Header("Variables")]
    public List<BasicToy.Tag> acceptedTags;
    public float launchTimer;
    public float rejectTimer;
    public AnimationCurve suckCurve;
    public float launchVelocity;
    public float rejectVelocity;
    public float squishAmount;
    [Header("Related Components")]
    public Transform input;
    public Transform output;
    public Transform center;
    public GameObject rejectParticleSystem;
    public Material mat;
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip suckSound;
    public AudioClip shakeSound;
    public AudioClip launchSound;
    public AudioClip rejectSound;

    [HideInInspector]public GameObject objectLoaded = null;
    [HideInInspector] public GameObject deniedInput = null; //used so that the intake doesnt immediatly suck in a rejected item

    //Launches the item placed inside
    public void LaunchBall() {
        //play sounds
        PlaySound(launchSound, false);
        BasicToy toy = objectLoaded.GetComponent<BasicToy>();
        if (toy) {
            toy.PlayPickupSound();
        }

        //move object to output location
        objectLoaded.transform.position = output.transform.position;
        objectLoaded.transform.rotation = output.transform.rotation;
        //enable colliders/gravity on the item
        foreach (Collider c in objectLoaded.GetComponents<Collider>()) {
            c.enabled = true;
        }
        Rigidbody rb = objectLoaded.GetComponent<Rigidbody>();
        if (rb == null) {
            return;
        }
        rb.useGravity = true;

        //then launch it
        rb.velocity = output.transform.right * launchVelocity;
        objectLoaded = null;
    }

    //Rejects the item placed inside. Smoke effect
    public void RejectBall() {
        //play sounds
        PlaySound(rejectSound, false);
        BasicToy toy = objectLoaded.GetComponent<BasicToy>();
        if (toy) {
            toy.PlayPickupSound();
        }
        //move object to output location
        objectLoaded.transform.position = input.transform.position;
        objectLoaded.transform.rotation = input.transform.rotation;
        //enable colliders/gravity on the item
        foreach (Collider c in objectLoaded.GetComponents<Collider>()) {
            c.enabled = true;
        }
        Rigidbody rb = objectLoaded.GetComponent<Rigidbody>();
        if (rb == null) {
            return;
        }
        rb.useGravity = true;

        //then launch it
        rb.velocity = input.transform.right * rejectVelocity;
        Destroy(Instantiate(rejectParticleSystem, input.transform.position, Quaternion.LookRotation(input.transform.forward, input.transform.right) ), 3.0f);

        //stop the same item from being loaded for the next 2 seconds (so the machine doesnt grab it as it moves thru the input zone)
        deniedInput = objectLoaded;
        objectLoaded = null;
        StartCoroutine(DoActionAfterDelay(2.0f, () => { deniedInput = null; }));
    }

    public void LoadBall(GameObject ball) {
        //if the machine cant take the input item, do nothing with it
        if (objectLoaded != null || ball == deniedInput) {
            return;
        }
        objectLoaded = ball;
        //animate the object being pulled into the input
        StartCoroutine(SuckItemIn(1.0f, ball));
    }

    //Once a ball has been sucked into the machine, decide what to do with it
    public void BallEntered() {
        //check to see if its flagged as a toy
        BasicToy toyComponent = objectLoaded.GetComponent<BasicToy>();
        if (!toyComponent) {
            StartCoroutine(ShakeMachine(rejectTimer, RejectBall));
        }
        //check if any of the objects tags overlap, if they do launch it, if not reject it
        bool hasTagMatch = false;
        foreach (BasicToy.Tag tag in acceptedTags) {
            if (toyComponent.HasTag(tag)) {
                hasTagMatch = true;
                break;
            }
        }
        if (!hasTagMatch) {
            StartCoroutine(ShakeMachine(rejectTimer, RejectBall));
        } else {
            StartCoroutine(ShakeMachine(launchTimer, LaunchBall));
        }
    }

    //sets the audio source to play a specific sound, flag for if it should loop
    void PlaySound(AudioClip sound, bool doesLoop) {
        audioSource.clip = sound;
        audioSource.loop = doesLoop;
        audioSource.Play();
    }

    //applies a squash and stretch shader to the machine
    IEnumerator ShakeMachine(float duration, Action endAction) {
        //play sounds
        PlaySound(shakeSound, true);

        mat.SetFloat("_Amount", squishAmount);

        yield return new WaitForSeconds(duration);
        mat.SetFloat("_Amount", 0);
        endAction.Invoke();
    }

    //Pulls and item from its current location into the machine
    IEnumerator SuckItemIn(float duration, GameObject item) {
        //play sounds
        PlaySound(suckSound, false);
        //disable colliders/gravity on the item
        foreach (Collider c in item.GetComponents<Collider>()) {
            c.enabled = false;
        }
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }

        //initial values
        Vector3 startPosition = item.transform.position;
        float startTime = Time.time;

        //lerp item in
        while (startTime + duration > Time.time) {
            item.transform.position = Vector3.Lerp(startPosition, center.transform.position, suckCurve.Evaluate((Time.time - startTime) / duration));
            yield return new WaitForFixedUpdate();
        }
        item.transform.position = center.transform.position;

        BallEntered();
    }

    IEnumerator DoActionAfterDelay(float duration, Action endAction) {
        yield return new WaitForSeconds(duration);
        endAction.Invoke();
    }
}
