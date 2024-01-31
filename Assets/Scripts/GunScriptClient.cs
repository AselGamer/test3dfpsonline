using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScriptClient : MonoBehaviour
{
    public Vector3 eulerAngleOverride;
    public Vector3 positionOverride;

    //TODO: remove this

    public bool isViewModel;

    public Vector3 viewModelScale;
    public Vector3 viewModelPosition;
    public Vector3 viewModelEulerAngle;

    public void Start()
    {
        transform.localPosition = positionOverride;
        transform.localEulerAngles = eulerAngleOverride;
    }

    void Update()
    {
        if (isViewModel)
        {
            SetViewModel();
        }
    }

    public void SetViewModel()
    {
        transform.localEulerAngles = viewModelEulerAngle;
        transform.localScale = viewModelScale;
        transform.localPosition = viewModelPosition;
    }
}
