using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDigZone : InteractableObject {

    [Header("Hole Properties")]
    public GameObject holePrefab;
    public GameObject IconCanvas;
    public GameObject ParticleObject;
    public Vector3 finalSize = new Vector3(1.3f, 1.0f, 1.3f);
    public float digAmountNeeded = 1.0f;

    [Header("Reward Properties")]
    public GameObject reward;
    public AudioClip toySpawnSound;
    public Vector3 lowerSpawnVelocityBounds = new Vector3(-5.0f, 8.0f, -5.0f);
    public Vector3 upperSpawnVelocityBounds = new Vector3(5.0f, 10.0f, 5.0f);

    AudioSource audioSource;
    bool scentActive = false;
    bool isAnimating = false;
    HoleSizeChange holeInstance;
    DogController playerController;

    private void Start() {
        audioSource = gameObject.GetComponent<AudioSource>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<DogController>();
    }

    public override void OnInteract() {
        if ((scentActive || holeInstance != null) && !isAnimating) {//only run OnInteract if visible
            if (holeInstance == null) {
                //if this is the first time attempting to dig this hole, spawn a model for it
                holeInstance = playerController.SpawnHole(playerController.holeForwardDistance, holePrefab);
                holeInstance.maxAmountDug = digAmountNeeded;
                holeInstance.maxSize = finalSize;
            }
            //tell the controller what to do
            playerController.activeHole = holeInstance;
            playerController.StartItemDig();

            playerController.digCallback = StopDig;
        }
    }

    //checks to see if Icon can be enabled and does so
    public void EnableIcon() {
        if (isInRange && (scentActive || holeInstance != null) && !isAnimating) {
            IconCanvas.SetActive(true);
        }
    }
    public void DisableIcon() {
        IconCanvas.SetActive(false);
    }

    //enable icons if in range when scent hits
    public void OnScent() {
        scentActive = true;
        EnableIcon();
    }

    //disable icons if not dug yet when scent ends
    public void OnScentEnd() {
        scentActive = false;
        if (holeInstance == null) {
            DisableIcon();
        }
    }

    //trigger OnInRange when player is close
    public override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);
        if (col.gameObject.CompareTag("Player")) {
            EnableIcon();
        }
    }

    public override void OnTriggerExit(Collider col) {
        base.OnTriggerExit(col);
        if (col.gameObject.CompareTag("Player")) {
            DisableIcon();
        }
    }

    //called when size animations are started
    void StartDig() {
        isAnimating = true;
        IconCanvas.SetActive(false);
    }

    //called when size animations finish
    void StopDig() {
        //reset callback
        playerController.digCallback = null;

        //let doggo move again
        playerController.FinishItemDig();

        isAnimating = false;
        EnableIcon();

        if (holeInstance.getPercentDug() >= 1.0f) {
            FinishHole();
        }
    }

    //called when the hole is fully dug. Deletes it and spawns anything that was dug up
    void FinishHole() {
        //pop reward out of the ground
        reward = Instantiate(reward, transform.position, transform.rotation);
        reward.GetComponent<Rigidbody>().velocity = new Vector3(
                                                            Random.Range(lowerSpawnVelocityBounds.x, upperSpawnVelocityBounds.y), 
                                                            Random.Range(lowerSpawnVelocityBounds.y, upperSpawnVelocityBounds.y),
                                                            Random.Range(lowerSpawnVelocityBounds.z, upperSpawnVelocityBounds.z));
        //reward.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        DisableIcon();
        //remove hole
        playerController.RemoveObject(this);
        StartCoroutine(EnableCollider(0.5f));
        holeInstance.Decay();
        //play spawn sound
        audioSource.clip = toySpawnSound;
        audioSource.Play();
        //stop scent stuff
        ParticleObject.GetComponent<ParticleSystem>().Stop();
        ScentManager.Instance.scentObjects.Remove(ParticleObject.GetComponentInChildren<HoleDigZoneScent>());
    }

    //enables the collider on the item that was spawned, then deletes self
    IEnumerator EnableCollider(float delay) {
        yield return new WaitForSeconds(delay);
        reward.GetComponent<Collider>().enabled = true;
        Destroy(gameObject);
    }
}
