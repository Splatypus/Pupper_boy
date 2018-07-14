using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//add this to an object to have it be enabled and diabled when scent mode hits it
public class EnableWhenScent : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ScentManager.Instance.scentObjects.Add(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
