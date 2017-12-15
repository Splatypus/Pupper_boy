using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerContact : MonoBehaviour {

    MeshDeformer deformer;
    public float requiredSpeed;

    // Use this for initialization
    void Start () {
        deformer = GetComponent<MeshDeformer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //constant collision detection
    private void OnCollisionStay(Collision collision) {
        //for each point of contact
        foreach (ContactPoint contact in collision.contacts) {
            //MeshDeformer deformer =  contact.otherCollider.GetComponent<MeshDeformer>();
            if (deformer) {
                Vector3 point = contact.point;
                deformer.FlattenPoint(point);
            }
        }
    }

    private void OnTriggerStay(Collider collision) {
        if (deformer && collision.GetComponent<Rigidbody>() && collision.GetComponent<Rigidbody>().velocity.magnitude > requiredSpeed) {
            Vector3 point = collision.transform.position;
            deformer.FlattenPoint(point);
        }
    }

}
