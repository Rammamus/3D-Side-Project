using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a script that moves the player in accordance to the direction it is facing - Adrian
public class PlayerMovement : MonoBehaviour
{
    //All variable relative to the movement - Adrian
    public float moveSpeed;
    public float groundDrag;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        MyInput();
    }

    //Checks the player's movement inputs - Adrian
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; //Declares the players movedirection with the orientation (which way the camera is facing) and the players inputs - Adrian
        rb.drag = groundDrag;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force); //Adds force in the previously calculated movedirection - Adrian
    }
}