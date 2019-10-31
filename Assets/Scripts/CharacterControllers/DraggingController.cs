using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggingController : Controller
{
    #region Component Variables
    CharacterController controller;
    Animator anim;
    [HideInInspector] public PuppyPickup mouth;
    #endregion

    [HideInInspector] public BasicDraggable draggedItem;
    bool isMoving = false;

    [Tooltip("When rotating to grab something, this is the speed in degrees per second that doggo moves")]
    public float grabRotationSpeed;
    public float grabStandDistance;
    public AnimationCurve grabLerpCurve;

    //start
    void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        mouth = GetComponentInChildren<PuppyPickup>();
    }

    //if rotation is in set amounts, lock to one of those amounts at the start
    public void Init(BasicDraggable d) {
        draggedItem = d;
        if (d.setRotateAmount == 0) {
            StartCoroutine( InitialGrab(0.0f, grabRotationSpeed) );
            return;
        }

        //find the angle to the dog
        float angleToDog = Vector3.SignedAngle(d.transform.forward, transform.position - d.transform.position, Vector3.up) + 360.0f;
        //find the closest multiple of setAngleAmount to that rotation
        float goalAngle = (angleToDog + 0.5f * d.setRotateAmount);
        goalAngle -= goalAngle % d.setRotateAmount;
        //convert to -180:180 range
        goalAngle = goalAngle%360.0f;
        if (goalAngle > 180)
            goalAngle -= 360;
        
        //find the difference between goal angle and angle to dog, and then rotate the dog by that amount
        float amountToRotate = (goalAngle - angleToDog);
        amountToRotate += amountToRotate > 180 ? -360 : (amountToRotate < -180 ? 360 : 0); //if above 180, subtract 360. If below -180, add 360
        
        StartCoroutine(InitialGrab(amountToRotate, grabRotationSpeed));
        //start drag anim
        anim.SetBool("IsDragging", true);
    }

    // Update is called once per frame
    void Update(){

        if (!isMoving) {
            if (Input.GetButtonDown("Interact")) {
                mouth.DoInputAction(); //This should drop the item
            }
            DoMovement();
        }

    }

    void DoMovement() {
        float horizontal = -Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        //Deal with set movement amount
        if (draggedItem.setMoveAmount > 0 && vertical != 0 ) {
            if (draggedItem.CanMove(Vector3.SignedAngle(draggedItem.transform.forward, transform.forward * (vertical > 0 ? 1 : -1), Vector3.up))) {
                StartCoroutine(Move(draggedItem.setMoveAmount / draggedItem.moveSpeed, draggedItem.setMoveAmount, vertical > 0));
            }

            //deal with set rotation amount
        } else if (draggedItem.setRotateAmount > 0 && horizontal != 0) {
            if (draggedItem.CanRotate(horizontal > 0)) {
                StartCoroutine(Rotate(draggedItem.setRotateAmount / draggedItem.rotationSpeed, draggedItem.setRotateAmount, horizontal > 0));
                //rotation animation
                if (horizontal < 0) {
                    anim.SetTrigger("RotateCCW");
                } else {
                    anim.SetTrigger("RotateCW");
                }
            }

            //Otherwise normal dragging. this section should be looked at...
        } else if (draggedItem.setMoveAmount == 0 || draggedItem.setRotateAmount == 0) {
            //rotate
            float rotationAmount = horizontal * draggedItem.rotationSpeed * Time.deltaTime;
            transform.RotateAround(draggedItem.transform.position, Vector3.up, rotationAmount);
            transform.LookAt(new Vector3(draggedItem.transform.position.x, transform.position.y, draggedItem.transform.position.z));
            draggedItem.transform.Rotate(Vector3.up, rotationAmount);

            //move
            Vector3 v = transform.forward * vertical * draggedItem.moveSpeed;
            v.y = -2 / Time.deltaTime;
            Vector3 preMoveLocation = transform.position;
            controller.Move(v * Time.deltaTime);
            draggedItem.transform.position += transform.position - preMoveLocation;

        }

    }

    public override void OnDeactivated() {
        base.OnDeactivated();
        anim.SetBool("IsDragging", false); //end drag animation
    }

    IEnumerator InitialGrab(float rotateAroundAmount, float speed) {
        isMoving = true;
        float startTime = Time.time;

        //rotation
        Quaternion initialRotation = transform.rotation;
        //Angle to dog is going to be the current angle rotated by rotateAroundAmount
        float lookAngle = Vector3.SignedAngle(draggedItem.transform.forward, transform.position - draggedItem.transform.position, Vector3.up);
        lookAngle += rotateAroundAmount + draggedItem.transform.rotation.eulerAngles.y;
        //get the opposite angle for dog to object
        lookAngle = (lookAngle + 540) % 360.0f;
        Quaternion goalRotation = Quaternion.AngleAxis(lookAngle, Vector3.up);

        //position
        Vector3 initialPosition = transform.position;
        Vector3 goalPosition =  draggedItem.transform.position 
                                + Quaternion.AngleAxis(lookAngle - draggedItem.transform.rotation.eulerAngles.y, Vector3.up) 
                                * -draggedItem.transform.forward  
                                * grabStandDistance;
        goalPosition.y = transform.position.y;

        //how long this rotation should take based off the current speed
        float time = Quaternion.Angle(transform.rotation, goalRotation) / speed;

        //do the movement
        while (Time.time < startTime + time) {
            float t = grabLerpCurve.Evaluate((Time.time - startTime) / time);
            //transform.RotateAround(draggedItem.transform.position, Vector3.up, rotateAroundAmount / time * Time.deltaTime);
            transform.position = Vector3.Lerp(initialPosition, goalPosition, t);
            transform.rotation = Quaternion.Lerp(initialRotation, goalRotation, t);
            yield return new WaitForEndOfFrame();
        }
        transform.position = goalPosition;
        transform.rotation = goalRotation;
        isMoving = false;
    }

    IEnumerator Move(float time, float distance, bool isForward) {
        isMoving = true;
        //initial conditions
        Vector3 initialPosition = transform.position;
        Vector3 objectInitPos = draggedItem.transform.position;
        Vector3 endPosition = transform.position + transform.forward * distance * (isForward?1:-1);
        Vector3 objectEndPos = draggedItem.transform.position + transform.forward * distance * (isForward ? 1 : -1);
        float startTime = Time.time;

        //do the movement
        while (Time.time < startTime + time) {
            float t = draggedItem.dragLerpCurve.Evaluate((Time.time - startTime) / time);
            transform.position = Vector3.Lerp(initialPosition, endPosition, t);
            draggedItem.transform.position = Vector3.Lerp(objectInitPos, objectEndPos, t);
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;
        draggedItem.transform.position = objectEndPos;
        
        isMoving = false;
        draggedItem.AfterMove( Vector3.SignedAngle(draggedItem.transform.forward, transform.forward * (isForward ? 1 : -1), Vector3.up) );
    }

    IEnumerator Rotate(float time, float distance, bool isRight) {
        isMoving = true;

        //initial conditions
        Quaternion initialObjectRotation = draggedItem.transform.rotation;
        Quaternion endObjectRotation = draggedItem.transform.rotation * Quaternion.AngleAxis(distance * (isRight?1:-1), Vector3.up);
        float startTime = Time.time;

        while (Time.time < startTime + time) {
            float t = draggedItem.rotateLerpCurve.Evaluate((Time.time - startTime) / time);
            draggedItem.transform.rotation = Quaternion.LerpUnclamped(initialObjectRotation, endObjectRotation, t);
            yield return new WaitForEndOfFrame();
        }
        draggedItem.transform.rotation = endObjectRotation;
        
        isMoving = false;
        draggedItem.AfterRotate(isRight);
    }


}
