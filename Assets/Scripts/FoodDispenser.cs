using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDispenser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool CanEat()
    {
        return true;
    }

    public void EatFood()
    {
        Debug.Log("food was eaten");
    }
}
