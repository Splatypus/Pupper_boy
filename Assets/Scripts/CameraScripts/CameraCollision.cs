using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    public float maxCamDis = 7.0f;
    private float curCamDis;
    public float moveInPaddingDistance = 0.5f;

    public GameObject player;

    // Use this for initialization
    void Start () {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        curCamDis = maxCamDis;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitForward, hitBackward;
        float newCamDis = maxCamDis;
        int layermask = 1<<2;
        layermask = ~layermask;
        //raycast forward to see if we collide with anything, if we do move the camera in front of it
        if (Physics.Raycast(transform.position, transform.forward, out hitForward, curCamDis, layermask)) {
            if (hitForward.collider.gameObject != player) {
                newCamDis = curCamDis - hitForward.distance - moveInPaddingDistance;
            }
        }
        if (Physics.Raycast(transform.position + transform.forward * curCamDis, -transform.forward, out hitBackward, curCamDis, layermask)){
            if (hitBackward.collider.gameObject != player) {
                newCamDis = Mathf.Min(hitBackward.distance - moveInPaddingDistance, newCamDis);
            }
        }
        //check to see if it can be moved backward.
        if (Physics.Raycast(transform.position - transform.forward * (maxCamDis - curCamDis), transform.forward, out hitForward, maxCamDis - curCamDis, layermask)) {
            if (hitForward.collider.gameObject != player) {
                newCamDis =Mathf.Min(maxCamDis - hitForward.distance - moveInPaddingDistance, newCamDis);
            }
        }
        if (Physics.Raycast(transform.position, -transform.forward, out hitBackward, maxCamDis - curCamDis, layermask)) {
            if (hitBackward.collider.gameObject != player) {
                newCamDis = Mathf.Min(curCamDis + hitBackward.distance - moveInPaddingDistance, newCamDis);
            }
        }

        //then set camera distance
        if (newCamDis > maxCamDis) {
            newCamDis = maxCamDis;
        }
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -newCamDis);
        curCamDis = newCamDis;

    }
}
