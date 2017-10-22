using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbase : MonoBehaviour {

    public Sprite[] Dialog;
    public GameObject Player;
    public float range;

    private bool inRange = false;
    

    //Triggers when the player enters the range
    public virtual void OnInRange() {

    }
    //Trigger when the player leaves range
    public virtual void OnExitRange() {

    }
    //Trigger when the player barks while in rage
    public virtual void OnBark() {

    }
    //Displays the thought bubble
    public void Display(Sprite icon) {

    }
    //hides the icon
    public void EndDisplay() {

    }
	
	// Update is called once per frame
	void Update () {
        if (!inRange && Vector3.Distance(gameObject.transform.position, Player.transform.position) < range) {
            inRange = true;
            OnInRange();
        } else if (inRange && Vector3.Distance(gameObject.transform.position, Player.transform.position) > range) {
            inRange = false;
            OnExitRange();
        }
	}


    // Use this for initialization
    void Start() {

    }

}
