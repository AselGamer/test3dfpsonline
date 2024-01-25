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
                transform.localPosition = new Vector3(gunPositionOverride.x, transform.position.y, transform.position.z);
            }
            if (overrideY)
            {
                transform.localPosition = new Vector3(transform.position.x, gunPositionOverride.y, transform.position.z);
            }
            if (overrideZ)
            {
                transform.localPosition = new Vector3(transform.position.x, transform.position.y, gunPositionOverride.z);
            }
        }
    }
}
