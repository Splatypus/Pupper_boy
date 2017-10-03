using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyBoxTrigger : MonoBehaviour {

    [SerializeField] private ToyBox toyBox;          //toybox that this trigger is connected to

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Pickup")
        {
            toyBox.AddToy(other.gameObject);
        }
    }
}
