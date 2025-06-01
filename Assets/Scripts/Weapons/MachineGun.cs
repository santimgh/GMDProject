using UnityEngine;

public class MachineGun : GunBase
{
    public float fireRate = 0.1f;
    private float lastFireTime;

    public override bool IsAutomatic => true;

    private void Awake()
    {
        maxAmmo = 15;
        currentAmmo = maxAmmo;
    }

    public override void Shoot()
    {
        if (Time.time >= lastFireTime + fireRate && HasAmmo)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;
            lastFireTime = Time.time;

            currentAmmo--;
        }
    }
}
