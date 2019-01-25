using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMovementController : Controller {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnActivated() {
        base.OnActivated();
        gameObject.GetComponent<Rigidbody>().drag = 100;
    }
}
