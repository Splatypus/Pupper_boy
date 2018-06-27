using UnityEngine;
using System.Collections;
//using UnityEditor;

public class FreeCameraLook : MonoBehaviour {

    public GameObject player;

    public float maxDistance = 7.0f;
    public float minDistance = 4.0f;
    public float turnSpeed = 1.5f;
    public float tiltMax = 75f;
    public float tiltMin = 45f;
    public bool controlLocked = false; //control of camera locks when npc stuff

    public float joypadXMultiplier = 2.0f;
    public float joypadYMultiplier = 2.0f;

    private float lookAngle;
    private float tiltAngle;

    public Transform anchor;

    private void Start() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update() {
        if (!controlLocked)
            HandleRotationMovement();
    }


    void HandleRotationMovement() {

        float x = Input.GetAxis("Mouse X") + Input.GetAxis("RightJoystickX") * joypadXMultiplier;
        float y = Input.GetAxis("Mouse Y") + Input.GetAxis("RightJoystickY") * joypadYMultiplier;
        //apply mouse movement
        transform.RotateAround(anchor.position, anchor.up, x);
        transform.RotateAround(anchor.position, transform.right, -y);


        //find out how many units back the camera can be
        RaycastHit hitForward, hitBackward;
        float camDis = 0.0f; //how far back the camera is allowed to be
        int layermask = 1 << 2; //ignore raycast layer is skipped
        layermask = ~layermask;
        //if (Physics.Raycast(transform.position, transform.forward, out hitForward, curCamDis, layermask)) {
        //  if (hitForward.collider.gameObject != player) {
        //}
        //}


        //if it moves to far from the player, follow them, if it moves too close, push it back (as long as it has space)
        if (Vector3.Distance(transform.position, player.transform.position) > maxDistance) {
            Vector3 newPosition = player.transform.position + (transform.position - player.transform.position).normalized * maxDistance;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        } else if (Vector3.Distance(transform.position, player.transform.position) < minDistance) {
            Vector3 newPosition = player.transform.position + (transform.position - player.transform.position).normalized * minDistance;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }

        //always look at the player
        transform.LookAt(anchor);

    }


    public void MoveToPosition(Vector3 location, Vector3 lookAt, float duration) {
        controlLocked = true;
        StartCoroutine(Pan(location, lookAt, duration));
    }

    //takes in a location and a spot to look at, and a duration. Pans to location, looking towards lookAt over the given duration
    IEnumerator Pan(Vector3 location, Vector3 lookAt, float duration) {
        float startTime = Time.time;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookAt - location);
        float startDistance = Vector3.Distance(lookAt, transform.position);
        float targetDistance = Vector3.Distance(lookAt, location);
        Quaternion startAngle = Quaternion.LookRotation(transform.position - lookAt);
        Quaternion targetAngle = Quaternion.LookRotation(location - lookAt);

        while (Time.time < startTime + duration) {
            float scaledTime = (Time.time - startTime) / duration;
            //keeps the camera distance away from lookat point, at an angle slerping between target and start.
            transform.position = lookAt - ((Quaternion.Slerp(startAngle, targetAngle, scaledTime) * Vector3.forward).normalized * Mathf.Lerp(startDistance, targetDistance, scaledTime));
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, scaledTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = lookAt - targetAngle.eulerAngles.normalized * targetDistance;
        transform.rotation = targetRotation;
    }

}
