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

    public GameObject[] gunInventoryScene;

    public Transform gunPosition;

    public Transform viewModelPosition;

    public void LoadLoadOut(GameObject[] gunInvAdd)
    {
        //Move to server script later
        gunInventoryScene = new GameObject[gunInvAdd.Length];
        for (int i = 0; i < gunInvAdd.Length; i++)
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
                gunInventoryScene[i] = auxGun;
            }
        }
    }

    public void HideLoadOut()
    {
        foreach (var gun in gunInventoryScene)
        {
            if (gun != null)
            {
                gun.transform.position = viewModelPosition.position;
            }
        }

    }

    internal void SwitchGun(int gunIndex)
    {
        gunInventoryScene[activeGunIndex].SetActive(false);
        activeGunIndex = gunIndex;
        gunInventoryScene[activeGunIndex].SetActive(true);
        activeGun = gunInventoryScene[activeGunIndex];
    }
}
