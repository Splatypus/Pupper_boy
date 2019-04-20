using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : Controller
{
    private CharacterController myController;
    private readonly float gravity = -9.8f;

    private Vector3 movementDirection;
    private Vector3 velocity;

    public float movementSpeed = 7.5f;
    public float jumpForce = 3.5f;

    void Start()
    {
        myController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        UpdateMovement();

        UpdateVelocity();
    }

    void UpdateMovement()
    {
        //Get Input, and Map It To Camera Direction
        Vector3 cameraRight = Vector3.ProjectOnPlane(Camera.main.transform.right, transform.up) * Input.GetAxis("Horizontal");
        Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.up) * Input.GetAxis("Vertical");

        //Apply Values To Movement Direction
        movementDirection = cameraForward + cameraRight;
        myController.Move(movementDirection.normalized * Time.deltaTime * movementSpeed);

        //Apply Movement To Character If movementDirection Is Not 0
        if (movementDirection != Vector3.zero)
            transform.forward = movementDirection;
    }

    void UpdateVelocity()
    {
        print(myController.isGrounded);
        //Apply Gravity To Velocity
        velocity.y += gravity * Time.deltaTime;

        //Apply Jump Force, If Jump Is Pressed, To Velocity
        velocity.y += Input.GetButtonDown("Jump") ? jumpForce : 0;

        velocity.y -= gravity * Time.deltaTime;
        //Apply Velocity To Character
        myController.Move(velocity);
    }
}
