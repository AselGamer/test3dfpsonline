using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptClient : MonoBehaviour
{
    [Header("Player objects/prefabs")]
    private GameObject activeGun;

    public GameObject[] gunInventory;

    public GameObject[] gunInventoryScene;

    public Transform gunPosition;

    public void LoadLoadOut()
    {
        //Move to server script later
        gunInventoryScene = new GameObject[gunInventory.Length];
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
                gun.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
