using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyBox : MonoBehaviour {

    [SerializeField] private GameObject[] toysInBox;             //list of all toys in the box
    [SerializeField] private float tossForce;                    //force applied to ball when tossed
    [SerializeField] private Vector3 launchPosition;             //position where toys are tossed



	// Use this for initialization
	void Start () {
		
	}

    private void Update()
    {
        
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
        Destroy(toy);

        int rand = Random.Range(0, toysInBox.Length);
        GameObject outToy = (GameObject)Instantiate(toysInBox[rand], this.transform.position + launchPosition, Quaternion.identity) as GameObject;
        outToy.GetComponent<Rigidbody>().AddForce(new Vector3(4f, tossForce, 0f), ForceMode.Impulse);
        Debug.Log("Toy Added");
    }
}
