using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbase : Dialog2 {

    public Sprite[] Icons;
    public GameObject Player;
    public SpriteRenderer iconRenderer;
    public GameObject iconCanvas;

    private bool inRange = false;

    // Use this for initialization
    new public void Start(){
        base.Start();
        Player = GameObject.FindGameObjectWithTag("Player");
        EndDisplay();
    }

    //Triggers when the player enters the range
    public virtual void OnInRange() {
        
    }
    //Trigger when the player leaves range
    public virtual void OnExitRange() {
        EndDisplay();
    }

    //Displays the thought bubble
    public void Display(int iconIndex) {
        if (Icons.Length != 0) {
            iconCanvas.SetActive(true);
            iconRenderer.sprite = Icons[iconIndex];
        }
    }
    //hides the icon
    public void EndDisplay() {
        if (iconCanvas != null)
            iconCanvas.SetActive(false);
    }


    public override void OnTriggerEnter(Collider col) {
        base.OnTriggerEnter(col);
        if (col.gameObject.CompareTag("Player")) {
            inRange = true;
            OnInRange();
        }
    }

    public override void OnTriggerExit(Collider col){
        base.OnTriggerExit(col);
        if (col.gameObject.CompareTag("Player")) {
            inRange = false;
            OnExitRange();
        }
    }
}
