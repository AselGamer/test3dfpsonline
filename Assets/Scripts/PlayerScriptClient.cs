using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void HideLoadOut(GameObject[] gunInvReplace)
    {
        gunInventoryScene.ToList().ForEach(x => Destroy(x));
        gunInventoryScene = new GameObject[gunInvReplace.Length];
        activeGun = null;
        for (int i = 0; i < gunInvReplace.Length; i++)
        {
            if (gunInvReplace[i] != null)
            {
                var auxGun = Instantiate(gunInvReplace[i], viewModelPosition.transform);
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
        miAnimator.SetFloat("walk_axis", (int)Mathf.Clamp01(Mathf.Abs(animation.velocidad_x) + Mathf.Abs(animation.velocidad_y)));
        miAnimator.SetFloat("fire_aim_axis", Mathf.Clamp01(Mathf.Abs(animation.fire_axis) + Mathf.Abs(animation.aim_axis)));
        miAnimator.SetFloat("fire_axis", animation.fire_axis);
        miAnimator.SetFloat("aim_axis", animation.aim_axis);
        miAnimator.SetFloat("velocidad_x", animation.velocidad_x);
        miAnimator.SetFloat("velocidad_y", animation.velocidad_y);
        miAnimator.SetBool("isGrounded", animation.isGrounded);
        //Gun Animations
        if (activeGun != null)
        {
            if (activeGun.TryGetComponent<ViewModelScript>(out ViewModelScript viewModelScript))
            {
                viewModelScript.aiming = animation.aim_axis > 0;
                viewModelScript.firing = animation.fire_axis > 0;
            }
            
        }
    }
}
