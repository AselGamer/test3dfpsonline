using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptClient : MonoBehaviour
{

    [Header("Player objects/prefabs")]
    private GameObject activeGun;

    private int activeGunIndex = -1;

    public GameObject camara;

    public GameObject playerModel;

    public GameObject[] gunInventoryScene;

    [Header("Animation variables")]

    public Transform gunPosition;
    public Transform viewModelPosition;
    public Animator miAnimator;
    public Transform aimDirection;
    public Transform leanAngles;

    void Update()
    {
        Vector3 newPosition = camara.transform.position + camara.transform.forward * 2 + Vector3.zero;
        aimDirection.transform.position = newPosition;
    }

    public void LoadLoadOut(GameObject[] gunInvAdd)
    {
        //Move to server script later
        gunInventoryScene = new GameObject[gunInvAdd.Length];
        for (int i = 0; i < gunInvAdd.Length; i++)
        {
            if (gunInvAdd[i] != null)
            {
                var auxGun = Instantiate(gunInvAdd[i], gunPosition.transform.position, gunPosition.transform.rotation);
                auxGun.transform.parent = gunPosition.transform;
                auxGun.SetActive(false);
                if (activeGun == null)
                {
                    auxGun.SetActive(true);
                    activeGun = auxGun;
                    activeGunIndex = i;
                }
                gunInventoryScene[i] = auxGun;
            }
        }
    }

    public void HideLoadOut()
    {
        foreach (var gun in gunInventoryScene)
        {
            gun.transform.parent = viewModelPosition;
            gun.GetComponent<GunScriptClient>().isViewModel = true;
        }
        playerModel.SetActive(false);

    }

    internal void SwitchGun(int gunIndex)
    {
        gunInventoryScene[activeGunIndex].SetActive(false);
        activeGunIndex = gunIndex;
        gunInventoryScene[activeGunIndex].SetActive(true);
        activeGun = gunInventoryScene[activeGunIndex];
    }

    public void PlayAnimations(NetworkObject.NetworkAnimation animation)
    {
        miAnimator.SetInteger("walking", animation.walking);
        miAnimator.SetInteger("strafing", animation.strafing);
        miAnimator.SetInteger("firing", animation.firing);
        miAnimator.SetBool("isGrounded", animation.isGrounded);
    }
}
