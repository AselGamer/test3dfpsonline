using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScriptClient : MonoBehaviour
{
    public Vector3 gunPositionOverride;
    public bool overrideX, overrideY, overrideZ = false;

    public void Start()
    {
        if (gunPositionOverride != null)
        {
            if (overrideX)
            {
                transform.localPosition = new Vector3(gunPositionOverride.x, transform.localPosition.y, transform.localPosition.z);
            }
            if (overrideY)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, gunPositionOverride.y, transform.localPosition.z);
            }
            if (overrideZ)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, gunPositionOverride.z);
            }
        }
    }
}
