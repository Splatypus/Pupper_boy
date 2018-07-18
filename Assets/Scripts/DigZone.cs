using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigZone : InteractableObject {

    public DogControllerV2 playerController;

    public DigZone other_side;

    public string enteringYardName = "";

    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<DogControllerV2>();
    }

    public override void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.GetComponent<DogControllerV2>().addObject(this);
            playerController.DigZoneEnter();
        }
        
    }

    public override void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.GetComponent<DogControllerV2>().removeObject(this);
            playerController.DigZoneExit();
        }
    }

    public override void OnInteract() {
        base.OnInteract();
        playerController.Dig(this);
    }

}
