using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Grounding")]
    public bool grounded;
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Dashing")]
    public float dashForce;
    public int dashes;
    public float dashCooldown;
    public float dashTimer;

    [Header("Jumping")]
    public bool canJump;
    public bool isJumping;
    public float jumpForce;

    [Header("Falling")]
    public float maxFallSpeed;
    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;
    public float fallingGravity;
    public float hangTimeGravity;
    public float hangTimeFallingThreshold;
    public float hangTimeRisingThreshold;

    [Header("Orientation")]
    public Transform orientation;
    public new Camera camera;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        gravityScale = 1;
        dashes = 2;
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
        GroundCheck();
        MyInput();
        Jump();
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashes > 0)
        {
            Debug.Log("shift");
            Dash();
        }

        if (dashes < 2)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer > dashCooldown)
            {
                dashes++;
                dashTimer = 0;
            }
        }
    }

    #region Move Player
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
    #endregion

    private void Dash()
    {
        rb.AddForce(camera.transform.forward * dashForce, ForceMode.Impulse);
        dashes--;
        Debug.Log("dash");
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        #region Gravity and falling
        if (rb.velocity.y < 0)
        {
            SetGravityScale(fallingGravity);
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed), rb.velocity.z);
        }
        else if (rb.velocity.y < hangTimeFallingThreshold && rb.velocity.y > hangTimeRisingThreshold && !grounded)
        {
            SetGravityScale(hangTimeGravity);
        }
        else if (grounded)
        {
            SetGravityScale(1);
            isJumping = false;
        }
        #endregion
    }

    #region General Methods
    public void SetGravityScale(float scale)
    {
        gravityScale = scale;
    }

    public void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }
    #endregion
}