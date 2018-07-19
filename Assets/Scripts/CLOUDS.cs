using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLOUDS : MonoBehaviour {

    public float minSpeed = 2;
    public float maxSpeed = 5;
    float speed;
    public Vector3 direction;
    Rigidbody rb;
    public float Xmin, Xmax, Zmin, Zmax;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        speed = Random.Range(minSpeed, maxSpeed);
        rb.velocity = direction.normalized * speed;
	}
	
	// Update is called once per frame
	void Update () {
        //move in bounds
        if (transform.localPosition.x < Xmin) {
            transform.localPosition = new Vector3(Xmax, transform.position.y, transform.position.z);
        } else if (transform.localPosition.x > Xmax) {
            transform.localPosition = new Vector3(Xmin, transform.position.y, transform.position.z);
        } else if (transform.localPosition.z < Zmin) {
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, Zmax);
        } else if (transform.localPosition.z > Zmax) {
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, Zmin);
        }
    }
}
