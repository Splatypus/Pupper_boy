using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//############  NO LONGER IN USE. NOTHING WILL INTERACT WITH THIS SCRIPT. EVERYTHING HERE HAS BEEN MOVED INTO DOGCONTROLLERV2


public class PlayerInteractions : MonoBehaviour {


    
    List<InteractableObject> inRangeOf = new List<InteractableObject>();
    public string InputAxis = "Interact";

    //add object to things we can interact with
    public void addObject(InteractableObject i) {
        inRangeOf.Add(i);
    }

    //remove object from list
    public void removeObject(InteractableObject i) {
        inRangeOf.Remove(i);
    }

    //when interact button is pressed, interact with everything were in range of (maybe change this to only the closest object?)
    public void Update() {
        if (Input.GetButtonDown(InputAxis)) {
            foreach (InteractableObject i in inRangeOf) {
                i.OnInteract();
            }
        }
    }
}
