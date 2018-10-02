using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewThroughObjectScript : MonoBehaviour {

    public Material mat;
    public GameObject doggo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        mat.SetVector("_CameraPosition", Camera.main.transform.position);
        mat.SetVector("_DoggoPosition", doggo.transform.position);
        mat.SetVector("_ObjectPosition", transform.position);
    }
}
