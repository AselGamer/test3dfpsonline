using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform firePoint;

    public int ammoCount;
    public int magSize;
    public int damage;
    public float fireRate;
    public float reloadTime;

    private float nextTimeToFire = 0f;

    public void Start()
    {
        ammoCount = magSize;
    }

    public void Fire()
    {
        if (Time.time >= nextTimeToFire)
        {
            RaycastHit hit;
            Physics.Raycast(firePoint.position, firePoint.forward, out hit, Mathf.Infinity);
            Debug.DrawRay(firePoint.position, firePoint.forward * hit.distance, Color.blue, 0.5f);

            nextTimeToFire = Time.time + 1f / fireRate;
        }

    }
}
