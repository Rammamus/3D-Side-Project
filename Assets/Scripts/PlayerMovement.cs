using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a script that moves the player in accordance to the direction it is facing - Adrian
public class PlayerMovement : MonoBehaviour
{
    //All variable relative to the movement - Adrian
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;

    [Header("Jumping")]
    public float maxFallSpeed;
    public bool grounded;
    public float playerHeight;
    public LayerMask whatIsGround;
    public float fallingGravity;

    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    CustomGravity customGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        //Makes the player actually fucking use gravity - Adrian
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void Update()
    {
        MyInput();
        Jump();
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

    private void Jump()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce * 10, ForceMode.Force);
        }
        if (rb.velocity.y < 0)
        {
            gravityScale *= fallingGravity;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
    }
}