using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Movement Speed")]
    public float moveSpeed;
    public float wallRunSpeed;
    public float groundDrag;

    [Header("Ground And Wall Check")]
    public bool grounded;
    public bool wallToLeft;
    public bool wallToRight;
    public bool canWallRun;
    public bool isWallRunning;
    public float playerHeight;
    public float playerWidth;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;

    [Header("Dashing")]
    public float dashForce;
    public int dashes;
    public float dashCooldown;
    public float dashTimer;
    public Slider sliderL, sliderR;
    public Image imageL, imageR;

    [Header("Jumping")]
    public bool canJump;
    public bool canDoubleJump;
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
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        GroundWallCheck();
        MyInput();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(canJump || canDoubleJump){
                Jump();
            }
        }

        if (Input.GetKey(KeyCode.Space)){
            if (!grounded){
                canWallRun = true;
            }

            if (canWallRun)
            {
                WallRun();
            }
        }
        else if (isWallRunning){
            if (!wallToLeft && !wallToRight)
            {
                isWallRunning = false;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (wallToLeft)
                {
                    Jump();
                    rb.AddForce(orientation.right * jumpForce * 3);
                }
                if (wallToRight)
                {
                    Jump();
                    rb.AddForce(-orientation.right * jumpForce * 3);
                }
            }
            Debug.Log("<color=green>boing</color>");
            isWallRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashes > 0)
        {
            Dash();
        }
        

        #region Dash timer and cooldown + UI
        if (dashes == 2)
        {
            sliderL.value = sliderR.value = 1;
            imageL.color = imageR.color = new Color32(43, 190, 224, 225); //blue
        }
        else if (dashes < 2)
        {
            dashTimer += Time.deltaTime;
            if(dashes == 1){
                sliderL.value = 1;
                sliderR.value = dashTimer/dashCooldown;
                imageL.color = new Color32(43, 190, 224, 225); //blue
                imageR.color = new Color32(255, 255, 255, 225); //white
            }
            else if (dashes == 0){
                sliderL.value = dashTimer/dashCooldown;
                sliderR.value = 0;
                imageL.color = new Color32(255, 255, 255, 225); //white
                imageR.color = new Color32(255, 255, 255, 225); //white
            }
            
            if (dashTimer > dashCooldown)
            {
                dashes++;
                dashTimer = 0;
            }
        }
        #endregion

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
        rb.AddForce(moveDirection.normalized * moveSpeed * 0.1f, ForceMode.VelocityChange); //Adds force in the previously calculated movedirection - Adrian
    }
    #endregion

    private void Dash()
    {
        rb.AddForce(camera.transform.forward * dashForce, ForceMode.Impulse);
        dashes--;
    }

    private void WallRun()
    {
        isWallRunning = true;
        SetGravityScale(0.1f);

        rb.AddForce(orientation.forward * wallRunSpeed * Time.deltaTime);

        if (wallToRight)
        {
            rb.AddForce(orientation.right * wallRunSpeed * Time.deltaTime);
        }
        else if (wallToLeft)
        {
            rb.AddForce(-orientation.right * wallRunSpeed * Time.deltaTime);
        }
        //Debug.Log("<color=blue>left wallrun</color>");
    }

    private void Jump()
    {
        if(canJump && canDoubleJump){
            canJump = false;
        }
        else if(!canJump && canDoubleJump){
            canDoubleJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //Sets falling velocity to 0 so you don't get a fucked up doublejump sometimes
            SetGravityScale(1);
        }
            
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
    }

    #region General Methods
    public void SetGravityScale(float scale)
    {
        gravityScale = scale;
    }

    public void GroundWallCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight, whatIsGround);
        Vector3 playerSide = orientation.transform.right;
        wallToLeft = Physics.Raycast(transform.position, -playerSide, playerWidth * 0.5f, whatIsWall);
        wallToRight = Physics.Raycast(transform.position, playerSide, playerWidth * 0.5f, whatIsWall);


        //debugging
        Debug.DrawRay(transform.position, Vector3.down * playerHeight, Color.green); // Ray down
        Debug.DrawRay(transform.position, -playerSide * playerWidth, Color.red); // Ray to the left
        Debug.DrawRay(transform.position, playerSide * playerWidth, Color.blue); // Ray to the right
        
        
        if (grounded)
        {
            canJump = true;
            canDoubleJump = true;
            canWallRun = false;
        }
        else
        {
            canJump = false;
        }

        if (wallToLeft){
        }

        if (wallToRight){
        }
    }
    #endregion
}