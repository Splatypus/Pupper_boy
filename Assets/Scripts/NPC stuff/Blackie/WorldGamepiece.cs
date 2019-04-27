using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[deprecated] - Change from interactable to IPickupItem/Basic toy broke some things...
//Since we have a new blackieminigame, this script will probably never be used

public class WorldGamepiece : BasicToy {
    
    Rigidbody rb;
    bool inZone;
    bool isHeld;
    public BlackieMiniGame2 gameSource;
    public GameObject targeter;
    new public List<MeshRenderer> meshes;
    public MeshRenderer fixedPieceMesh; //this mesh changes color when this piece is a fixed piece.
    public MeshRenderer defaultColorMesh; //this mesh is used to display a set initial color, such as for goal nodes
    float distance;
    Vector3 offsets;
    GameObject player;
    public BlackieMiniGame2.Gamepiece boardPiece;

    // Use this for initialization
    public void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        //set up the targeter gameobject
        targeter = Instantiate(targeter);
        gameSource = FindObjectOfType<BlackieMiniGame2>();
        distance = gameSource.tileDis;
        offsets = gameSource.gameObject.transform.position;
        offsets.x %= distance;
        offsets.z %= distance;
        targeter.SetActive(false);

    }

    // Update is called once per frame
    void Update() {
        if (inZone && isHeld) {

            Vector3 preSnap = transform.position;
            //should targeter show?
            Vector2Int gridLocation = gameSource.WorldToGridSpace(preSnap);
            if (targeter.activeInHierarchy && !gameSource.CanPlace(boardPiece, gridLocation.x, gridLocation.y, 0)) {
                targeter.SetActive(false);
                //Debug.Log("Disabling due to bad placement position: " + gridLocation.x + ", " + gridLocation.y);
            }
            else if (!targeter.activeInHierarchy && gameSource.CanPlace(boardPiece, gridLocation.x, gridLocation.y, 0)) {
                targeter.SetActive(true);
            }

            if (targeter.activeInHierarchy) {
                //target snapping
               // preSnap.x += distance / 2.0f - offsets.x;
               // preSnap.z += distance / 2.0f - offsets.z;
               // targeter.transform.position = new Vector3(preSnap.x - (preSnap.x % distance) + offsets.x, 0.5f, preSnap.z - (preSnap.z % distance) + offsets.z);
                targeter.transform.position = gameSource.GridToWorldSpace(gridLocation);
            }

        }
    }

    bool checkShowTarget() {
        return isHeld && inZone;
    }

    public void OnPlace() {
        rb.isKinematic = true;
        //find where it goes on the grid
        Vector2Int gridLocation = gameSource.WorldToGridSpace(transform.position);
        //calculate it's rotation
        float rotation = Mathf.Repeat(transform.rotation.eulerAngles.y - gameSource.transform.rotation.eulerAngles.y + 720.0f, 360.0f);
        //place piece
        gameSource.PlacePiece(boardPiece, gridLocation.x, gridLocation.y, (int)(rotation/90.0f + 0.5f));
        transform.position = gameSource.GridToWorldSpace(gridLocation);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        //if this piece is locked, stop you from ever picking it up by canging its tag and disabling this script
        if (boardPiece.isLocked) {
            gameObject.tag = "Untagged";
            this.enabled = false;
        }
    }

    /*public override void OnPickup()
    {
        //if it is kinematic that means its been placed on the board
        if (rb.isKinematic)
        {
            Vector2Int gridLocation = gameSource.WorldToGridSpace(transform.position);
            gameSource.RemovePiece(gridLocation.x, gridLocation.y);
        }
        rb.isKinematic = false;
        //rb.detectCollisions = false;
        base.OnPickup();
        isHeld = true;
        targeter.SetActive(checkShowTarget());
        
    }*/

    //for when a short is caused and this piece needs to be yote into the air
    public void DoForcedRemove() {
        rb.isKinematic = false;
        rb.velocity = (new Vector3(Random.Range(-2.5f, 2.5f), 10.0f, Random.Range(-2.5f, 2.5f)));
    }

    /*public override void OnDrop()
    {
        base.OnDrop();
        //if the targeter was showing that means we can place it, so drop it on the board rather than just wherever
        if (targeter.activeInHierarchy) {
            OnPlace();
        }

        isHeld = false;
        targeter.SetActive(checkShowTarget());
        //respawn item?
        //rb.isKinematic = true;
    }*/

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
