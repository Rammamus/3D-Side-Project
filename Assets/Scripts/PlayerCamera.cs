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

    private void Start()
    {
        //Locks and hides cursor - Adrian
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}