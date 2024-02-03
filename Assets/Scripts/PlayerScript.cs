using NetworkMessages;
using NetworkObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{
    [Header("Player objects/prefabs")]

    public string playerId;

    public Transform camara;

    public GameObject activeGun;

    public int activeGunIndex = -1;

    public GameObject[] gunInventory;

    public Transform gunPosition;

    [Header("Player variables")]
    public int health;

    public bool dead = false;

    public float timeUntilRespawn = 0f;

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

    private float playFireAnimation = 0f;

    private float nextTimeToFire = 0f;

    private Rigidbody rb;

    [Header("Animator variables")]
    public Animator miAnimator;

    [Header("Server variables")]
    public Server server;

    private int time;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        server = GameObject.Find("Server").GetComponent<Server>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {

        miAnimator.SetFloat("walk_axis", Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)));
        miAnimator.SetFloat("fire_aim_axis", Mathf.Clamp01(Mathf.Abs(playFireAnimation) + Mathf.Abs(aimInput)));
        miAnimator.SetFloat("velocidad_x", horizontalInput);
        miAnimator.SetFloat("velocidad_y", verticalInput);
        miAnimator.SetFloat("aim_axis", aimInput);
        miAnimator.SetFloat("fire_axis", playFireAnimation);
        miAnimator.SetBool("isGrounded", isGrounded);

        NetworkAnimation networkAnimation = new NetworkAnimation();
        networkAnimation.velocidad_x = horizontalInput;
        networkAnimation.velocidad_y = verticalInput;
        networkAnimation.aim_axis = aimInput;
        networkAnimation.fire_axis = (int)playFireAnimation;
        networkAnimation.isGrounded = isGrounded;

        server.SendPlayerAnimation(playerId, networkAnimation);
    }

    void Update()
    {
        if (health <= 0)
        {
            health = 0;
        }

        if (health > 100)
        {
            health = 100;
        }

        if (timeUntilRespawn > 0f)
        {
            timeUntilRespawn -= Time.deltaTime;
            if (timeUntilRespawn <= Mathf.Epsilon)
            {
                timeUntilRespawn = 0f;
            }
        }

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
            //This didn't work before, but now it does
            if (fireInput == 1 && Time.time >= nextTimeToFire && gunScript.ammoInMag > 0)
            {
                playFireAnimation = 1f;
                nextTimeToFire = Time.time + 1f / gunScript.fireRate;
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
                    gunScript.idPlayer = playerId;
                    gunScript.cameraTransform = camara.transform;
                }

                if (auxGun.TryGetComponent<ShotgunScript>(out ShotgunScript shotgunScript))
                {
                    shotgunScript.idPlayer = playerId;
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

    void StopGunFireAnimation()
    {
        playFireAnimation = 0f;
    }

    void FireGun()
    {
        if (activeGun.TryGetComponent<GunScript>(out GunScript gunScript))
        {
            gunScript.Fire();
        }
    }

    public void RestoreAmmo()
    {
        foreach (GameObject gun in gunInventory)
        {
            if (gun.TryGetComponent<GunScript>(out GunScript gunScript))
            {
                gunScript.RestoreAmmo();
            }
        }
    }

    private void OnTriggerEnter(Collider colldier)
    {
        if (colldier.tag == "Medkit")
        {
            if (health >= 100)
            {
                return;
            }
            health += 50;
        }

        if (colldier.tag == "ammo")
        {
            if (activeGun.TryGetComponent<GunScript>(out GunScript gunScript))
            {
                gunScript.ammoCount += (int)Mathf.Round(gunScript.startingAmmo / 2f);
            }
        }

        colldier.gameObject.SetActive(false);
    }
}
