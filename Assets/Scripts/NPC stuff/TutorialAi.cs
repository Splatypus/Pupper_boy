using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAi : Dialog {

	// Use this for initialization
	new void Start () {
        base.Start();
        StartCoroutine(afterStart());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator afterStart() {
        yield return new WaitForEndOfFrame();
        OnInteract();
    }
}
