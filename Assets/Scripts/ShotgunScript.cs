using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ShotgunScript : GunScript
{
    public int pelletsCount = 10;
    public float spreadAngle = 30f;
    public float shotRange = 10f;


    public override void Fire()
    {
        reloading = true;
        for (int i = 0; i < pelletsCount; i++)
        {
            float currentSpread = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);

            Vector3 direction = Quaternion.Euler(0, currentSpread, 0) * cameraTransform.forward;

            if (Physics.Raycast(cameraTransform.position, direction, out RaycastHit hit, shotRange))
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

            //Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 20f, Color.magenta, 0.5f);
        }
        ammoInMag--;
    }

    public override IEnumerator Reload()
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
