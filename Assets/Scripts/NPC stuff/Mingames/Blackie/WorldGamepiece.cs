using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGamepiece : Interactable {

    Rigidbody rb;
    bool inZone;
    bool isHeld;
    public BlackieMiniGame gameSource;
    public GameObject targeter;
    float distance;
    Vector3 offsets;
    GameObject player;
    public Gamepiece boardPiece;

    // Use this for initialization
    public void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        //set up the targeter gameobject
        targeter = Instantiate(targeter);
        gameSource = FindObjectOfType<BlackieMiniGame>();
        distance = gameSource.tileDis;
        offsets = gameSource.gameObject.transform.position;
        offsets.x %= distance;
        offsets.z %= distance;
        targeter.SetActive(false);

        if (boardPiece == null)
            boardPiece = new BlackieNode(gameObject, false);
    }

    // Update is called once per frame
    void Update() {
        if (inZone && isHeld) {

            Vector3 preSnap = transform.position;
            //should targeter show?
            Vector2Int gridLocation = gameSource.WorldToGridSpace(preSnap);
            if (targeter.activeInHierarchy && !gameSource.CanPlace(boardPiece, gridLocation.x, gridLocation.y, 0)) {
                targeter.SetActive(false);
                Debug.Log("Disabling due to bad placement position");
            }
            else if (!targeter.activeInHierarchy && gameSource.CanPlace(boardPiece, gridLocation.x, gridLocation.y, 0)) {
                targeter.SetActive(true);
            }

            if (targeter.activeInHierarchy) {
                //target snapping
                preSnap.x += distance / 2.0f - offsets.x;
                preSnap.z += distance / 2.0f - offsets.z;
                targeter.transform.position = new Vector3(preSnap.x - (preSnap.x % distance) + offsets.x, 0.5f, preSnap.z - (preSnap.z % distance) + offsets.z);
            }

        }
    }

    bool checkShowTarget() {
        return isHeld && inZone;
    }

    public void OnPlace() {
        Vector2Int gridLocation = gameSource.WorldToGridSpace(transform.position);
        gameSource.PlacePiece(boardPiece, gridLocation.x, gridLocation.y, 0);
        Vector3 pos = transform.position;
        pos.x += distance / 2.0f - offsets.x;
        pos.z += distance / 2.0f - offsets.z;
        transform.position = new Vector3(pos.x - (pos.x % distance) + offsets.x, 0.5f, pos.z - (pos.z % distance) + offsets.z);
        transform.rotation = new Quaternion();
        rb.isKinematic = true;
        //if this piece is locked, stop you from ever picking it up by canging its tag and disabling this script
        if (boardPiece.isLocked) {
            gameObject.tag = "Untagged";
            this.enabled = false;
        }
    }

    public override void onPickup()
    {
        //if it is kinematic that means its been placed on the board
        if (rb.isKinematic)
        {
            Vector2Int gridLocation = gameSource.WorldToGridSpace(transform.position);
            gameSource.RemovePiece(gridLocation.x, gridLocation.y);
        }
        rb.isKinematic = false;
        base.onPickup();
        isHeld = true;
        targeter.SetActive(checkShowTarget());
        
    }

    public override void onDrop()
    {
        base.onDrop();
        //if the targeter was showing that means we can place it, so drop it on the board rather than just wherever
        if (targeter.activeInHierarchy) {
            OnPlace();
        }

        isHeld = false;
        targeter.SetActive(checkShowTarget());
        //respawn item?
        //rb.isKinematic = true;
    }

    public void FancyDestroy() {
        StartCoroutine(DeathTimer());
    }

    IEnumerator DeathTimer() {
        ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule mm = ps.main;
        mm.gravityModifier = -0.4f;
        ParticleSystem.EmissionModule em = ps.emission;
        em.rateOverTimeMultiplier = 50.0f;
        yield return new WaitForSeconds(1.0f);
        em.rateOverTimeMultiplier = 100.0f;
        yield return new WaitForSeconds(0.5f);
        foreach (MeshRenderer r in gameObject.GetComponentsInChildren<MeshRenderer>()){
            r.enabled = false;
        }
        yield return new WaitForSeconds(0.5f);
        em.rateOverTimeMultiplier = 0.0f;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    //let set inZone variable if we enter or leave the zone with this object
    public void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("GameArea")) { 
            inZone = true;
            targeter.SetActive(checkShowTarget());
        } else if (other.gameObject.GetComponent<DigZone>() != null) {
            //respawn?
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("GameArea")) { 
            inZone = false;
            targeter.SetActive(checkShowTarget());
        }
    }
}
