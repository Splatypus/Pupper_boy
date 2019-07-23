using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoAnimationReceiver : MonoBehaviour
{

    public DogController movementController;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AfterDugObject() {
        movementController.AfterDugObject();
    }

    public void AfterDugDown() {
        movementController.AfterDugDown();
    }

    public void FinishDig() {
        movementController.FinishDig();
    }

    public void PlayDigSound() {
        movementController.PlayDigSound();
    }

    public void SpewDirt() {
        movementController.SpewDirt();
    }

    public void ShakeDirt() {
        movementController.ShakeDirt();
    }

    public void ExpandHole(float percentDug) {
        movementController.ExpandHole(percentDug);
    }

}
