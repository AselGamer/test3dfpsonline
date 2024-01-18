using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float sensitivity;

    public Transform orientation;

    public float Zrotation;

    private float yRotation;
    private float xRotation;

    void Start()
    {
        
    }

    
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, Zrotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
