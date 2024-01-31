using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelScript : MonoBehaviour
{
    public Vector3 viewModelScale;
    public Vector3 viewModelPosition;
    public Vector3 viewModelEulerAngle;
    void Start()
    {
        transform.localEulerAngles = viewModelEulerAngle;
        transform.localScale = viewModelScale;
        transform.localPosition = viewModelPosition;
    }
}
