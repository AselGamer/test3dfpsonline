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

    public short jumpInput;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isGrounded)
        {
            if (jumpInput == 1)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpInput = 0;
            }
        }
    }


    void FixedUpdate()
    {
        /*
        float verticalInput = Input.GetKey("d") ? 1f : Input.GetKey("a") ? -1f : 0f;
        float horizontalInput = Input.GetKey("w") ? 1f : Input.GetKey("s") ? -1f : 0f;

        float leanInput = Input.GetKey("e") ? -1f : Input.GetKey("q") ? 1f : 0f;
        */

        isGrounded = false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.25f))
        {
            isGrounded = true;
        }

        rotationZ = Mathf.Lerp(rotationZ, 20f * leanInput, 10f * Time.deltaTime);

        camara.GetComponent<CameraScript>().Zrotation = rotationZ;

        Vector3 forward = camara.transform.forward;
        Vector3 right = camara.transform.right;
        forward.y = 0;
        right.y = 0;

        Vector3 moveDirection = forward * horizontalInput + right * verticalInput;

        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(camara.rotation.x, camara.rotation.y, rotationZ);

        camara.GetComponent<CameraScript>().Zrotation = this.rotationZ;

        transform.Translate(movement);
    }

    public void UpdateMovementVariables(PlayerInputMsg pInputMsg)
    {
        verticalInput = pInputMsg.verticalInput;
        horizontalInput = pInputMsg.horizontalInput;
        leanInput = pInputMsg.leanInput;
        jumpInput = pInputMsg.jumpInput;
    }
}
