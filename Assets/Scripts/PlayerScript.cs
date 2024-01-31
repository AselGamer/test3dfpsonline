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

    public GameObject activeGun;

    public int activeGunIndex = -1;

    public GameObject[] gunInventory;

    public Transform gunPosition;

    [Header("Player variables")]
    public int health;

    public bool dead = false;

    [Header("Controller variables")]

    public float speedQuantity = 1;
    public float moveSpeed;
    public float jumpForce;

    private float rotationZ;

    public bool isGrounded = false;

    public short verticalInput;
    public short horizontalInput;

    public short leanInput;

    public bool jumpInput;

    public byte reloadInput;

    public byte fireInput;

    public byte aimInput;

    private Rigidbody rb;

    [Header("Animator variables")]
    public Animator miAnimator;

    [Header("Server variables")]
    public Server server;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        miAnimator = GetComponent<Animator>();
        server = GameObject.Find("Server").GetComponent<Server>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (health <= 0)
        {
            health = 0;
        }

        miAnimator.SetFloat("walk_axis", Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)));
        miAnimator.SetFloat("fire_aim_axis", Mathf.Clamp01(Mathf.Abs(fireInput) + Mathf.Abs(aimInput)));
        miAnimator.SetFloat("velocidad_x", horizontalInput);
        miAnimator.SetFloat("velocidad_y", verticalInput);
        miAnimator.SetFloat("aim_axis", aimInput);
        miAnimator.SetFloat("fire_axis", fireInput);
        miAnimator.SetBool("isGrounded", isGrounded);

        server.SendPlayerAnimation(gameObject);
    }

    void FixedUpdate()
    {

        isGrounded = false;


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 1.10f))
        {
            isGrounded = true;
        }

        rotationZ = Mathf.Lerp(rotationZ, 30f * leanInput, 10f * Time.deltaTime);

        Vector3 moveDirection = new Vector3(verticalInput, 0, horizontalInput);

        Vector3 movement = moveDirection * (moveSpeed * speedQuantity) * Time.deltaTime;

        if (isGrounded && jumpInput && leanInput==0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        jumpInput = false;

        transform.rotation = Quaternion.Euler(0, camara.eulerAngles.y, rotationZ);

        camara.GetComponent<CameraScript>().Zrotation = this.rotationZ;

        transform.Translate(movement);

        if (activeGun.TryGetComponent<GunScript>(out GunScript gunScript))
        {
            if (fireInput == 1)
            {
                gunScript.Fire();
            }

            if (reloadInput == 1 && fireInput == 0)
            {
                gunScript.reloading = false;
            } else if(reloadInput == 1 && fireInput == 1)
            {
                gunScript.reloading = true;
            }

            if (aimInput == 1)
            {
                speedQuantity = 0.75f;
            }
            else
            {
                speedQuantity = 1f;
            }

        }
    }

    public void UpdateMovementVariables(PlayerInputMsg pInputMsg)
    {
        verticalInput = pInputMsg.verticalInput;
        horizontalInput = pInputMsg.horizontalInput;
        leanInput = pInputMsg.leanInput;
        fireInput = pInputMsg.fireInput;
        aimInput = pInputMsg.aimInput;
        reloadInput = pInputMsg.reloadInput;
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
                auxGun.SetActive(false);

                //This code sucks

                if (auxGun.TryGetComponent<GunScript>(out GunScript gunScript))
                {
                    gunScript.cameraTransform = camara.transform;
                }

                if (auxGun.TryGetComponent<ShotgunScript>(out ShotgunScript shotgunScript))
                {
                    shotgunScript.cameraTransform = camara.transform;
                }

                auxGun.transform.parent = gunPosition.transform;
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

    public void TakeDamage(float damage, float distance)
    {
        //TODO: Add this to the weapon script
        float closeRange = 5f; 
        float farRange = 20f;

        float closeRangeBonus = Mathf.Clamp01(1f - distance / closeRange);

        float farRangePenalty = Mathf.Clamp01(distance / farRange);

        // Calculate the final damage
        float finalDamage = damage * (1f + closeRangeBonus) * (1f - farRangePenalty);

        health -= (int)finalDamage;
    }
}
