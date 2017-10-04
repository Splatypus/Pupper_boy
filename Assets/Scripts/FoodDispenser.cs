using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDispenser : MonoBehaviour {

    public bool is_full = true;
    [SerializeField] private float refill_time = 10.0f;
    [SerializeField] private GameObject food_holder;

    [SerializeField] private AudioClip[] eating_clips;
    AudioSource m_source;
    

	// Use this for initialization
	void Start () {
        m_source = GetComponent<AudioSource>();
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
        int i = Random.Range(0, eating_clips.Length);
        m_source.clip = eating_clips[i];
        m_source.Play();
        food_holder.SetActive(false);
        Invoke("activeate_food", refill_time);
    }

    private void activeate_food()
    {
        food_holder.SetActive(true);
    }
}
