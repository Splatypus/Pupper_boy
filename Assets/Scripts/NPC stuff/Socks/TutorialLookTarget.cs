using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLookTarget : MonoBehaviour {

    public SocksTutorial owner;
    public float dotCutoff;

    public float lookTimeNeeded;
    float totalLookTime = 0.0f;

    public Color startColor;
    public Color endColor;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //if the player is looking in the right direction
        if (Vector3.Dot((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) > dotCutoff) {
            //add time looking at the object
            totalLookTime += Time.deltaTime;
            //then change its color based on how long you've looked at it
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, totalLookTime / lookTimeNeeded);

            //then complete this objective if its been looked at for long enough
            if (totalLookTime > lookTimeNeeded) {
                owner.ObjectiveComplete();
                Destroy(gameObject);
            }
        }
	}
}
