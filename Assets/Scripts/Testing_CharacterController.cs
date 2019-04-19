using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_CharacterController : Controller
{
    private CharacterController _controller;
    private readonly float gravity = -9.8f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _controller.Move(move * Time.deltaTime * 50);
        if (move != Vector3.zero)
            transform.forward = move;

        Vector3 _velocity = Vector3.zero;
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity);
    }
}
