using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyBox : MonoBehaviour {

    private List<GameObject> toysInBox = new List<GameObject>();             //list of all toys in the box
    [SerializeField] private float tossForce;                                //force applied to ball when tossed



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TossInBox(GameObject toy)
    {
        toy.transform.parent = this.transform;
        Vector3 distance = transform.position - toy.transform.position;
        distance.Normalize();
        toy.GetComponent<Rigidbody>().AddForce(new Vector3(distance.x, tossForce, distance.z), ForceMode.Impulse);
    }

    public void AddToy(GameObject toy)
    {
        toy.transform.parent = this.transform;

        if (toysInBox.Count > 0)
        {
            int rand = Random.Range(0, toysInBox.Count);
            GameObject outToy = toysInBox[rand];
            outToy.GetComponent<Rigidbody>().AddForce(new Vector3(1f, tossForce, 0f), ForceMode.Impulse);
        }

        toysInBox.Add(toy);
    }
}
