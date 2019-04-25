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

    public BasicDraggable draggedItem;

    bool isMoving = false;

    void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        mouth = GetComponentInChildren<PuppyPickup>();
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetButtonDown("Interact")) {
            mouth.DoInputAction();
        }

        if (!isMoving)
            DoMovement();

    }

    void DoMovement() {
        float horizontal = -Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (draggedItem.setMoveAmount > 0 && vertical != 0) {
            isMoving = true;
            StartCoroutine(  Move(draggedItem.setMoveAmount / draggedItem.moveSpeed, draggedItem.setMoveAmount, vertical > 0)  );

        } else if (draggedItem.setRotateAmount > 0 && horizontal != 0) {
            isMoving = true;
            StartCoroutine(  Rotate(draggedItem.setRotateAmount / draggedItem.rotationSpeed, draggedItem.setRotateAmount, horizontal > 0)  );

        } else {
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

    IEnumerator Move(float time, float distance, bool isForward) {
        yield return new WaitForSeconds(10);
        isMoving = false;
    }

    IEnumerator Rotate(float time, float distance, bool isRight) {
        yield return new WaitForSeconds(10);
        isMoving = false;
    }


}
