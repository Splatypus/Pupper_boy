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

    public void AfterDugDown() {
        movementController.AfterDugDown();
    }

    public void PlayDigSound() {
        movementController.PlayDigSound();
    }

    public void FinishDig() {
        movementController.FinishDig();
    }

    public void SpewDirt() {
        movementController.SpewDirt();
    }

    public void ExpandHole(float percentDug) {
        movementController.ExpandHole(percentDug);
    }

}
