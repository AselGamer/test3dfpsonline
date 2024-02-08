using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelScript : MonoBehaviour
{
    public Vector3 viewModelScale;
    public Vector3 viewModelPosition;
    public Vector3 viewModelEulerAngle;

    public ParticleSystem particles;

    public bool aiming = false;
    public bool firing = false;

    private Animator miAnimator;
    void Start()
    {
        transform.localEulerAngles = viewModelEulerAngle;
        transform.localScale = viewModelScale;
        transform.localPosition = viewModelPosition;
        miAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        miAnimator.SetBool("aiming", aiming);
        miAnimator.SetBool("firing", firing);
    }

    public void EnableMuzzleFlash()
    {
        particles.Play();
    }

    public void DisableMuzzleFlash()
    {
        particles.Clear();
        particles.Stop();
    }
}
