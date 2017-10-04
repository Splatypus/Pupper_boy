using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDispenser : MonoBehaviour {

    public bool is_full = true;
    [SerializeField] private float refill_time = 10.0f;
    [SerializeField] private GameObject food_holder;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool CanEat()
    {
        return food_holder.activeSelf;
    }

    public void EatFood()
    {
        //Debug.Log("food was eaten");
        food_holder.SetActive(false);
        Invoke("activeate_food", refill_time);
    }

    private void activeate_food()
    {
        food_holder.SetActive(true);
    }
}
