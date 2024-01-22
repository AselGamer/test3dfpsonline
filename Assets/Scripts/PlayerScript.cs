using NetworkMessages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    public Transform camara;
    
    public float moveSpeed;
    public float jumpForce;

    private float rotationX;
    private float rotationY;

    private float rotationZ;

    private bool isGrounded = false;

    public short verticalInput;
    public short horizontalInput;

    public short leanInput;

    public bool jumpInput;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        
    }


    void FixedUpdate()
    {

        isGrounded = false;


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 1.10f))
        {
            isGrounded = true;
        }

        rotationZ = Mathf.Lerp(rotationZ, 20f * leanInput, 10f * Time.deltaTime);

        camara.GetComponent<CameraScript>().Zrotation = rotationZ;

        Vector3 moveDirection = new Vector3(verticalInput, 0, horizontalInput);

        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        if (isGrounded && jumpInput && leanInput==0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jumpInput = false;


        transform.rotation = Quaternion.Euler(0, camara.eulerAngles.y, camara.eulerAngles.z);

        camara.GetComponent<CameraScript>().Zrotation = this.rotationZ;

        transform.Translate(movement);
    }

    public void UpdateMovementVariables(PlayerInputMsg pInputMsg)
    {
        verticalInput = pInputMsg.verticalInput;
        horizontalInput = pInputMsg.horizontalInput;
        leanInput = pInputMsg.leanInput;
    }
}
