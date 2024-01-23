using NetworkMessages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    [Header("Player objects/prefabs")]

    public Transform camara;

    private GameObject activeGun;

    public int activeGunIndex = -1;

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

        if (activeGun.TryGetComponent<GunScript>(out GunScript gunScript))
        {
            if (fireInput == 1)
            {
                gunScript.Fire();
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

    public void LoadLoadOut(GameObject[] gunInvAdd)
    {
        //Move to server script later
        gunInventory = new GameObject[gunInvAdd.Length];

        for (int i = 0; i < gunInventory.Length; i++)
        {
            if (gunInvAdd[i] != null)
            {
                var auxGun = Instantiate(gunInvAdd[i], gunPosition.transform.position, gunPosition.transform.rotation);
                auxGun.transform.parent = camara.transform;
                auxGun.SetActive(false);
                if (activeGun == null)
                {
                    auxGun.SetActive(true);
                    activeGun = auxGun;
                    activeGunIndex = i;
                }
                gunInventory[i] = auxGun;
            }
        }
    }

    internal void SwitchGun(PlayerSwitchGunMsg pSwitchGunMsg)
    {
        if (activeGunIndex != -1)
        {
            if (pSwitchGunMsg.switchGunInput != 0 && pSwitchGunMsg.switchGunInput <= gunInventory.Length)
            {
                gunInventory[activeGunIndex].SetActive(false);
                activeGunIndex = pSwitchGunMsg.switchGunInput - 1;
                gunInventory[activeGunIndex].SetActive(true);
                activeGun = gunInventory[activeGunIndex];
            }
            
            if (pSwitchGunMsg.mouseScrollInput != 0)
            {
                gunInventory[activeGunIndex].SetActive(false);
                if (pSwitchGunMsg.mouseScrollInput == 1)
                {
                    if (activeGunIndex == gunInventory.Length - 1)
                    {
                        activeGunIndex = 0;
                    }
                    else
                    {
                        activeGunIndex++;
                    }
                }
                else
                {
                    if (activeGunIndex == 0)
                    {
                        activeGunIndex = gunInventory.Length - 1;
                    }
                    else
                    {
                        activeGunIndex--;
                    }
                }
                gunInventory[activeGunIndex].SetActive(true);
                activeGun = gunInventory[activeGunIndex];
            }
        }
    }
}
