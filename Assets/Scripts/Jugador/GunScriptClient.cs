using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScriptClient : MonoBehaviour
{
    public Vector3 eulerAngleOverride;
    public Vector3 positionOverride;

    public void Start()
    {
        transform.localPosition = positionOverride;
        transform.localEulerAngles = eulerAngleOverride;
    }
}
