using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform firePoint;

    public GameObject bulletHolePrefab;

    public Server server;

    public Transform cameraTransform;

    public Transform handlePoint;

    public Vector3 overrideEulerAngles;
    public Vector3 overridePosition;

    public int ammoCount;
    public int magSize;
    public int ammoInMag;
    public int damage;
    public float fireRate;
    public float reloadTime;

    public bool reloading = true;

    public string gunType;

    private void Start()
    {
        ammoInMag = magSize;
        ammoCount -= magSize;
        server = GameObject.Find("Server").GetComponent<Server>();

        transform.localPosition = overridePosition;
        transform.localEulerAngles = overrideEulerAngles;
        StartCoroutineReload();
    }

    void OnEnable()
    {
        StartCoroutineReload();
    }

    void OnDisable()
    {
        reloading = true;
    }

    public virtual void Fire()
    {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.tag != "Player")
                {
                    server.CreateBulletHole(hit);
                }
                else 
                {
                    hit.collider.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, hit.distance);
                }
            }
            ammoInMag--;
    }

    public void StartCoroutineReload()
    {
        StopAllCoroutines();
        StartCoroutine(Reload());
    }

    public virtual IEnumerator Reload()
    {
        while (true)
        {
            yield return new WaitForSeconds(reloadTime);
            if (ammoInMag < magSize && ammoCount > 0 && !reloading)
            {
                reloading = true;
                ammoInMag++;
                ammoCount--;
                reloading = false;
                
            }
        }
    }
}
