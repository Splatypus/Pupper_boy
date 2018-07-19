using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDigZone : InteractableObject {

    public GameObject holePrefab;
    public GameObject reward;
    public GameObject IconCanvas;
    public GameObject ParticleObject;
    public int totalDigCount = 3; //how many times this needs to be dug
    public AudioClip digSound;
    public AudioClip toySpawnSound;

    AudioSource audioSource;
    bool inRange = false;
    bool scentActive = false;
    bool isAnimating = false;
    int digCount = 0;
    Vector3 originalScale;
    DogControllerV2 playerController;

    private void Start() {
        audioSource = gameObject.GetComponent<AudioSource>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<DogControllerV2>();
    }

    public override void OnInteract() {
        if ((scentActive || digCount > 0) && !isAnimating) {//only run OnInteract if visible
            digCount++;
            audioSource.clip = digSound;
            audioSource.Play();
            if (digCount == 1) {
                //instantiate the hole and then grow its size
                holePrefab = Instantiate(holePrefab, transform);
                holePrefab.layer = 9;
                originalScale = holePrefab.transform.localScale;
                holePrefab.transform.localScale = Vector3.zero;
                StartCoroutine(GrowHole(1.0f));
            }
            //increase hole size
            StartCoroutine(GrowHole(1.0f));
        }
    }

    //checks to see if Icon can be enabled and does so
    public void EnableIcon() {
        if (inRange && (scentActive || digCount > 0) && !isAnimating) {
            IconCanvas.SetActive(true);
        }
    }

    //enable icons if in range when scent hits
    public void OnScent() {
        scentActive = true;
        if (inRange) {
            EnableIcon();
        }
    }

    //disable icons if not dug yet when scent ends
    public void OnScentEnd() {
        scentActive = false;
        if (digCount == 0) {
            IconCanvas.SetActive(false);
        }
    }

    //trigger OnInRange when player is close
    public override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);
        if (col.gameObject.CompareTag("Player")) {
            //only run of this object is visible
            inRange = true;
            EnableIcon();
        }
    }

    public override void OnTriggerExit(Collider col) {
        base.OnTriggerExit(col);
        if (col.gameObject.CompareTag("Player")) {
            inRange = false;
            IconCanvas.SetActive(false);
        }
    }

    //called when size animations are started
    void StartAnim() {
        isAnimating = true;
        IconCanvas.SetActive(false);
    }

    //called when size animations finish
    void StopAnim() {
        isAnimating = false;
        EnableIcon();
    }

    IEnumerator GrowHole(float duration) {
        StartAnim();
        float startTime = Time.time;
        while (startTime + duration > Time.time) {
            holePrefab.transform.localScale = Vector3.Lerp(originalScale * (digCount - 1) / totalDigCount, originalScale * digCount / totalDigCount, (Time.time - startTime) / duration);
            yield return new WaitForEndOfFrame();
        }
        StopAnim();
        //then check to see if the hole reached its full size
        if (digCount >= totalDigCount) {
            reward = Instantiate(reward, transform.position, transform.rotation);
            reward.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-5.0f, 5.0f), 10.0f, Random.Range(-5.0f, 5.0f));
            reward.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
            playerController.removeObject(this);
            StartCoroutine(EnableCollider(0.5f));
            StartCoroutine(ShrinkHoleOverTime(10.0f));
            //play spawn sound
            audioSource.clip = toySpawnSound;
            audioSource.Play();
        }
    }

    IEnumerator EnableCollider(float delay) {
        yield return new WaitForSeconds(delay);
        reward.GetComponent<Collider>().enabled = true;
    }

    //changes the size of the hole over a given duration to zero, then deletes it
    IEnumerator ShrinkHoleOverTime(float duration) {
        //start animation, disable particles
        StartAnim();
        ParticleObject.GetComponent<ParticleSystem>().Stop();
        //shrink hole over time
        float startTime = Time.time;
        while (startTime + duration > Time.time) {
            holePrefab.transform.localScale = originalScale * (1 - ((Time.time - startTime) / duration));
            yield return new WaitForEndOfFrame();
        }
        ScentManager.Instance.scentObjects.Remove(ParticleObject.GetComponent<ScentObject>());
        //wait for particle system to finish before destroying this
        yield return new WaitForSeconds(Mathf.Max(ParticleObject.GetComponent<ParticleSystem>().main.startLifetime.constant - duration, 0.0f));
        Destroy(gameObject);
    }
}
