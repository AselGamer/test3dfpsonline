using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform orientation;

    public float Zrotation;

    private float yRotation;
    private float xRotation;

    public float mouseX;
    public float mouseY;

    void Start()
    {
        
    }

    
    void Update()
    {
        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, Zrotation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
