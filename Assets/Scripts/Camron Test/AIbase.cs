using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIbase : MonoBehaviour {

    public GameObject[] Dialog;
    private GameObject Player;
    public float range;
    public GameObject Toy;

    private bool inRange = false;
    private GameObject activeIcon;
    

    //Triggers when the player enters the range
    public virtual void OnInRange() {
        Display(Dialog[0]);
    }
    //Trigger when the player leaves range
    public virtual void OnExitRange() {
        EndDisplay();
    }

    public virtual void ToyInRange() {
        Destroy(Toy);
        EndDisplay();
        Display(Dialog[1]);
    }

    //Trigger when the player barks while in rage
    public virtual void OnBark() {

    }
    //Displays the thought bubble
    public void Display(GameObject icon) {
        EndDisplay();
        icon.SetActive(true);
        activeIcon = icon;
    }
    //hides the icon
    public void EndDisplay() {
        if (activeIcon != null) {
            activeIcon.SetActive(false);
        }
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
        if (Toy != null && Vector3.Distance(gameObject.transform.position, Toy.transform.position) < range) {
            ToyInRange();
        }
    }


    // Use this for initialization
    void Start() {
        foreach (GameObject i in Dialog) {
            i.SetActive(false);
        }
        Player = GameObject.FindGameObjectWithTag("Player");
    }

}
