using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : Controller
{
    private CharacterController myController;
    private readonly float gravity = -9.8f;
    private bool isGrounded;

    private Vector3 movementDirection;
    private Vector3 velocity = Vector3.zero;

    public float movementSpeed = 10;
    public float jumpForce = 1.1f;

    private bool jumped = false;
    private bool jumping = false;
    private float jumpTimer = 0;
    public float jumpBeginDuration = 0.2f;
    public float jumpFloatDuration = 0.5f;
    [Range(0.01f, 1)]
    public float jumpFloatAmount = 0.4f;

    void Start()
    {
        myController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        isGrounded = myController.isGrounded;

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
        myController.Move(movementDirection.normalized * Time.fixedDeltaTime * movementSpeed);

        //Apply Movement To Character If movementDirection Is Not 0
        if (movementDirection != Vector3.zero)
            transform.forward = movementDirection;
    }

    void UpdateVelocity()
    {
        if (jumping)
        {
            JumpBegin();
        }
        else if (jumped)
        {
            JumpFloat();
        }
        else
        {
            //Apply Gravity To Velocity
            velocity.y += gravity * Time.fixedDeltaTime;

            //Apply Velocity To Character
            myController.Move(velocity);

            //Readjust Gravity For If Player Is Grounded
            velocity.y = myController.isGrounded ? 0 : velocity.y;

            //Check For Jumping, Beginning Next Frame
            if (Input.GetButtonDown("Jump"))
            {
                velocity = Vector3.zero;
                jumping = true;
            }
        }
    }

    //Runs Durring the Top of the Jump Arc
    void JumpFloat()
    {
        velocity.y -= Time.fixedDeltaTime * (1 / jumpFloatAmount);
        myController.Move(velocity);

        jumpTimer += Time.fixedDeltaTime;

        if (jumpTimer >= jumpFloatDuration)
        {
            jumpTimer = 0;
            jumped = false;
        }
    }

    //Runs Durring the Beginning of the Jump Arc
    void JumpBegin()
    {
        velocity.y += jumpForce * Time.fixedDeltaTime;
        myController.Move(velocity);

        jumpTimer += Time.fixedDeltaTime;

        if (jumpTimer >= jumpBeginDuration)
        {
            jumpTimer = 0;
            jumping = false;
            jumped = true;
        }
    }

    void AddForce(Vector3 forceVector, bool isAddative = true)
    {
        if (!isAddative)
            velocity = Vector3.zero;

        velocity += forceVector;
    }
}
