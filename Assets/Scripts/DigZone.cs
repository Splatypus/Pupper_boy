using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigZone : InteractableObject {

    public DogController playerController;

    public DigZone other_side;

    //public string enteringYardName = ""; //The name of the yard you enter when you dig INTO this zone

    
    public enum Yards { Seabiscuit, Bubbles, Tiffany, Home, Rex, Creek, Blackie }
    public static string[] yardNames = { "Seabiscuit's Yard", "Bubbles' Yard", "Tiffany's Yard", "Home", "Rex's Domain", "Cherry Creek", "Chip's Workshop" };
    public Yards enteringYard;

    private void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<DogController>();
    }

    //used to easily get the yard name from the enum
    public static string GetYardName(Yards y) {
        return yardNames[(int)y];
    }

    //and one to get the name of a instance of this object specifically
    public string GetYardName() {
        return yardNames[(int)enteringYard];
    }

    public override void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.GetComponent<DogController>().AddObject(this);
            playerController.DigZoneEnter();
        }
        
    }

    public override void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.GetComponent<DogController>().RemoveObject(this);
            playerController.DigZoneExit();
        }
    }

    public override void OnInteract() {
        base.OnInteract();
        playerController.Dig(this);
    }

}
