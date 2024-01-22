using NetworkMessages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    [Header("Player objects/prefabs")]

    public Transform camara;

    private GameObject activeGun;

    public GameObject[] gunInventory;

    public Transform gunPosition;

    [Header("Player variables")]
    public int health;

    [Header("Controller variables")]

    public float moveSpeed;
    public float jumpForce;

    private float rotationZ;

    private bool isGrounded = false;

    public short verticalInput;
    public short horizontalInput;

    public short leanInput;

    public bool jumpInput;

    public byte fireInput;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        if (activeGun != null)
        {
            if (fireInput == 1)
            {
                activeGun.GetComponent<GunScript>().Fire();
            }
        }
    }

    public void UpdateMovementVariables(PlayerInputMsg pInputMsg)
    {
        verticalInput = pInputMsg.verticalInput;
        horizontalInput = pInputMsg.horizontalInput;
        leanInput = pInputMsg.leanInput;
        fireInput = pInputMsg.fireInput;
    }

    public void LoadLoadOut()
    {
        //Move to server script later
        for (int i = 0; i < gunInventory.Length; i++)
        {
            if (gunInventory[i] != null)
            {
                var auxGun = Instantiate(gunInventory[i], gunPosition.transform.position, gunPosition.transform.rotation);
                auxGun.transform.parent = transform;
                auxGun.SetActive(false);
                if (i == 0 && gunInventory[0] != null)
                {
                    auxGun.SetActive(true);
                    activeGun = auxGun;
                }
            }
        }
    }
}
