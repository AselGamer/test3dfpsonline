using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelScript : MonoBehaviour
{
    public Vector3 viewModelScale;
    public Vector3 viewModelPosition;
    public Vector3 viewModelEulerAngle;

    public ParticleSystem particles;
    private ParticleSystem.EmissionModule muzzleEmision;

    public bool aiming = false;
    public bool firing = false;

    private Animator miAnimator;
    void Start()
    {
        transform.localEulerAngles = viewModelEulerAngle;
        transform.localScale = viewModelScale;
        transform.localPosition = viewModelPosition;
        miAnimator = GetComponent<Animator>();
        muzzleEmision = particles.emission;
    }

    void Update()
    {
        miAnimator.SetBool("aiming", aiming);
        miAnimator.SetBool("firing", firing);
    }

    public void EnableMuzzleFlash()
    {
        muzzleEmision.enabled = true;
        Debug.Log("Muzzle flash enabled");
    }

    public void DisableMuzzleFlash()
    {
        muzzleEmision.enabled = false;
        Debug.Log("Muzzle flash disabled");
    }
}
