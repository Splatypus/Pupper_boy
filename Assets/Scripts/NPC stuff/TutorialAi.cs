using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAi : Dialog2 {

	// Use this for initialization
	new void Start () {
        base.Start();
        StartCoroutine(AfterStart());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator AfterStart() {
        yield return new WaitForSeconds(0.1f);
        OnInteract();
    }
}
