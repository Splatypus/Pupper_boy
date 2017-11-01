using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class daynight : MonoBehaviour {

    private Light thisLight;

    private float transition = 0.0f;
    public float transitionSpeed = 0.25f;

    private bool sunrise = true;

	// Use this for initialization
	void Start () {
        thisLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        transition += (sunrise) ? transitionSpeed * Time.deltaTime : -transitionSpeed * Time.deltaTime;
        if (transition < 0.0f || transition > 1.0f)
        {
            sunrise = !sunrise;
        }

        thisLight.intensity = transition;
        thisLight.color = Color.Lerp(Color.blue, Color.white, transition);
	}
}
