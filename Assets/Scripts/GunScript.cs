using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform firePoint;

    public GameObject bulletHolePrefab;

    public Server server;

    public int ammoCount;
    public int magSize;
    public int ammoInMag;
    public int damage;
    public float fireRate;
    public float reloadTime;

    private float nextTimeToFire = 0f;

    private void Start()
    {
        ammoCount = magSize;
        server = GameObject.Find("Server").GetComponent<Server>();
    }

    public virtual void Fire()
    {
        if (Time.time >= nextTimeToFire)
        {
            if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, Mathf.Infinity))
            {
                //Change to pool and move to client
                if (hit.transform.tag != "Player")
                {
                    server.CreateBulletHole(hit);
                }
                else 
                {
                    hit.collider.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, hit.distance);
                }
            }
            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }
}
