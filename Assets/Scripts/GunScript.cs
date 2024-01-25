using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform firePoint;

    public GameObject bulletHolePrefab;

    public Server server;

    public Transform cameraTransform;

    public int ammoCount;
    public int magSize;
    public int ammoInMag;
    public int damage;
    public float fireRate;
    public float reloadTime;

    public bool reloading = false;

    private float nextTimeToFire = 0f;

    private void Start()
    {
        ammoInMag = magSize;
        ammoCount-=magSize;
        server = GameObject.Find("Server").GetComponent<Server>();
        cameraTransform = transform.parent;
    }

    public virtual void Fire()
    {
        if (Time.time >= nextTimeToFire && ammoInMag > 0)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, Mathf.Infinity))
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
            ammoInMag--;
        }
    }

    public virtual IEnumerator Reload()
    {
        if (reloading)
        {
            yield break;
        }
        while (ammoInMag < magSize && ammoCount > 0)
        {
            reloading = true;
            if (!gameObject.activeSelf)
            {
                break;
            }
            ammoInMag++;
            ammoCount--;
            yield return new WaitForSeconds(reloadTime);
        }
        reloading = false;
    }
}
