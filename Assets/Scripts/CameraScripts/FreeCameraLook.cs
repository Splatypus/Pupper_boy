using UnityEngine;
using System.Collections;
//using UnityEditor;

//public void MoveToPosition(Vector3 location, Vector3 lookAt, float duration)
//moves the camera to the desired location, looking towards location LookAt, and lerped over duration

//public void RestoreCamera(float duration)
//moves the camera back to the player character over the given duration


public class FreeCameraLook : MonoBehaviour {

    [Header("Related Objects")]
    public GameObject player;
    public Transform anchor;
    public GameObject LockCameraLocation;

    [Header("Control Values")]
    public LayerMask mask;
    public float maxDistance = 7.0f; //the base unmodified max distance for the camera
    [HideInInspector]public float trueMaxDistance = 7.0f;  //the actual modified maximum distance for the camera
    public float minDistance = 0.2f;
    public float turnSpeed = 1.5f;
    public float angleMin = 15f;
    public float angleMax = 80f;
    public bool controlLocked = false; //control of camera locks when npc stuff

    public float joypadXMultiplier = 2.0f;
    public float joypadYMultiplier = 2.0f;

    [Header("Settings Contorl Values")]
    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    private float lookAngle;
    private float tiltAngle;
    
    Vector3 previousFrameLocation;
    Vector3 cameraGoal;

    private void Start() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        previousFrameLocation = anchor.position;
        cameraGoal = transform.position;
    }

    // Update is called once per frame
    void Update() {

        //Used to lock the camera to a set location. Useful for capturing promotional material
        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L)) || (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.L))) {
            if (controlLocked) {
                RestoreCamera(0.0f);
            } else {
                controlLocked = true;
                transform.position = LockCameraLocation.transform.position;
                transform.rotation = LockCameraLocation.transform.rotation;
            }
        }

        if (!controlLocked) {
            
            HandleRotationMovement(); //new position is calculated

            //but keep it rotation locked despite that
            float angle = FindCameraAngle();
            transform.RotateAround(anchor.position, transform.right, ClampRotationAngle(angle, 0, angleMin, angleMax));
            //keep camera looking at player
            transform.LookAt(anchor);

            //move the camera in for collision
            DoCameraCollision(anchor.position, minDistance, trueMaxDistance);
        }
    }

    void HandleRotationMovement() {

        Vector3 anchorPosition = anchor.position;

        //the amount the player has moved awayfrom/towards the camer, or up and down since the last frame
        Vector3 movementDelta = Vector3.ProjectOnPlane(anchorPosition - previousFrameLocation, transform.right);
        transform.position += movementDelta;

        //apply mouse movement
        float x = (Input.GetAxis("Mouse X") + Input.GetAxis("RightJoystickX") * joypadXMultiplier) * xSensitivity;
        float y = (Input.GetAxis("Mouse Y") + Input.GetAxis("RightJoystickY") * joypadYMultiplier) * -ySensitivity;
        transform.RotateAround(anchorPosition, anchor.up, x);
        float angle = FindCameraAngle();
        transform.RotateAround(anchorPosition, transform.right, ClampRotationAngle(angle, y, angleMin, angleMax));

        previousFrameLocation = anchorPosition;

    }
    public void DoCameraCollision(Vector3 target,float minDis, float maxDis) {
        //find out how many units back the camera can be
        float newDistance = maxDis;
        Camera mainCamera = Camera.main;
        //Vector that when added to a position gets the worldspace coordinates of each camera corner if the camera were moved to that position
        Vector3[] cornerPoints = {
            mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)) - transform.position,
            mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)) - transform.position,
            mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane)) - transform.position,
            mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane)) - transform.position     
        };

        foreach (Vector3 v in cornerPoints) {
            Debug.Assert(cornerPoints[0].x != 0);
            Debug.Assert(cornerPoints[0].x != cornerPoints[1].x);
            RaycastHit hit;
            //raycast backwards from target points
            if (Physics.Raycast( target + v + -transform.forward * mainCamera.nearClipPlane, -transform.forward, out hit, maxDis, mask) ) {
                //if that distance is smaller than our current target distance, set it to that instead
                newDistance = Mathf.Min(newDistance, hit.distance);
            }
        }
        //do no go below the minimum distance
        newDistance = Mathf.Max(newDistance, minDis);

        //set the camera
        float currentDistance = Vector3.Distance(transform.position, target);
        if (currentDistance < newDistance) {
            newDistance = Mathf.Min(currentDistance + 10.0f * Time.deltaTime, newDistance);
        }
        transform.position = target + (transform.position - target).normalized * newDistance;
    }
    //returns the angle in degress that the camera is above the xz plane of the anchor
    private float FindCameraAngle() {
        float height = transform.position.y - anchor.transform.position.y;
        float distance = Mathf.Sqrt( Mathf.Pow(transform.position.x - anchor.transform.position.x, 2) + Mathf.Pow(transform.position.z - anchor.transform.position.z, 2) );
        return Mathf.Atan(height / distance) * Mathf.Rad2Deg;
    }
    //clamps a rotation delta so that it doesnt move below or above a max value when applies to an angle
    private float ClampRotationAngle(float angle, float delta, float min, float max) {
        if (angle + delta < min) {
            return min - angle;
        }
        if (angle + delta > max) {
            return max - angle;
        }
        return delta;
    }


    //moves the camera to the given position, facing lookat, over the duration given. Smooths movement and disables input.
    public void MoveToPosition(Vector3 location, Vector3 lookAt, float duration, System.Action OnComplete = null) {
        controlLocked = true;
        StartCoroutine(Pan(location, lookAt, duration, OnComplete));
    }
    //smoothly moves the camera to it's closest valid location, then reenables input
    public void RestoreCamera(float duration) {
        Vector3 targetPosition = anchor.position + (transform.position - anchor.position).normalized * maxDistance;
        //raycast towards targetPosition and move it in if needed to avoid camera collision
        RaycastHit hit;
        if (Physics.Raycast(anchor.position, transform.position - anchor.position, out hit, trueMaxDistance, mask)) {
            targetPosition = anchor.position + (transform.position - anchor.position).normalized * Mathf.Max(hit.distance, minDistance);
        }

        //start a pan coroutine to move camera, and an unlock control one to return control to the player as that ends
        StartCoroutine(Pan(targetPosition, anchor.position, duration, () => controlLocked = false));
    }
    //takes in a location and a spot to look at, and a duration. Pans to location, looking towards lookAt over the given duration
    IEnumerator Pan(Vector3 location, Vector3 lookAt, float duration, System.Action OnComplete = null) {
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
            transform.position = lookAt + ((Quaternion.Slerp(startAngle, targetAngle, scaledTime) * Vector3.forward).normalized * Mathf.Lerp(startDistance, targetDistance, scaledTime));
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, scaledTime);
            DoCameraCollision(lookAt, 0, Vector3.Distance(transform.position, lookAt));
            yield return new WaitForEndOfFrame();
        }
        transform.position = lookAt + (targetAngle * Vector3.forward).normalized * targetDistance;
        transform.rotation = targetRotation;
        cameraGoal = transform.position;
        OnComplete?.Invoke();
    }


    #region Used for camera movement from editor components
    //For dialog functions, call setposition, setlookat, then move the camera with RunSetMove
    GameObject targetPosition;
    GameObject targetLookAt;
    public void SetTargetPosition(GameObject t) { targetPosition = t; }
    public void SetTargetLookAt(GameObject t) { targetLookAt = t; }
    public void RunSetMove(float time) {
        MoveToPosition(targetPosition.transform.position, targetLookAt.transform.position, time);
    }

    #endregion

}