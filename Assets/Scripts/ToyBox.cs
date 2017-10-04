using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyBox : MonoBehaviour {

    private List<GameObject> toysInBox = new List<GameObject>();             //list of all toys in the box
    [SerializeField] private float tossForce;                                //force applied to ball when tossed
    [SerializeField] private Vector3 storePosition;                          //position where toys are stored



	// Use this for initialization
	void Start () {
		
	}

    private void LateUpdate()
    {
       foreach (GameObject toy in toysInBox)
        {
            toy.transform.rotation = this.transform.rotation;
            toy.transform.localPosition = storePosition;
        }

    }

    public void TossInBox(GameObject toy)
    {
        toy.transform.parent = this.transform;
        toy.GetComponent<Rigidbody>().useGravity = true;
        Vector3 distance = transform.position - toy.transform.position;
        distance.Normalize();
        toy.GetComponent<Rigidbody>().AddForce(new Vector3(distance.x, tossForce, distance.z), ForceMode.Impulse);
    }

    public void AddToy(GameObject toy)
    {
        toy.transform.parent = this.transform;
        toy.transform.localPosition = storePosition;
        toy.GetComponent<Rigidbody>().useGravity = false;

        if (toysInBox.Count > 0)
        {
            int rand = Random.Range(0, toysInBox.Count);
            GameObject outToy = toysInBox[rand];
            outToy.transform.parent = null;
            toysInBox.Remove(outToy);
            outToy.GetComponent<Rigidbody>().useGravity = true;
            outToy.transform.Translate(new Vector3(0f, 2f, 0f));
            outToy.GetComponent<Rigidbody>().AddForce(new Vector3(4f, tossForce, 0f), ForceMode.Impulse);
            Debug.Log("Toy Removed");
        }

        toysInBox.Add(toy);
        Debug.Log("Toy Added");
    }
}
