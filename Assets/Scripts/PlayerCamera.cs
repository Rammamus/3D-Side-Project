using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is in charge of changing the camera's and player's orientation by reading the mouse input - Adrian
public class PlayerCamera : MonoBehaviour
{
    public float sensitivity;

    public Transform orientation;
    public new GameObject camera;

    float xRotation;
    float yRotation;

    float wallRunCameraTilt;
    public float maxWallRunCameraTilt;

    PlayerMovement pm;

    private void Start()
    {
        //Locks and hides cursor - Adrian
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //get the mouse input - Adrian
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        //"store" how much the mouse has been moved - Adrian
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //restrict the xRotation so you can't "do flips" - Adrian

        //rotate the camera and player orientation - Adrian
        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRunCameraTilt);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


        if (Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && pm.isWallRunning && pm.wallToLeft)
        {
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 3;
        }
        if (Mathf.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && pm.isWallRunning && pm.wallToRight)
        {
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 3;
        }

        if (!pm.isWallRunning)
        {
            if (wallRunCameraTilt < 0)
            {
                wallRunCameraTilt += Time.deltaTime * 20;
            }
            if (wallRunCameraTilt > 0)
            {
                wallRunCameraTilt -= Time.deltaTime * 20;
            }
        }
    }
}