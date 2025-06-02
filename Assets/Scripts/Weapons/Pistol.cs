using UnityEngine;

public class Pistol : GunBase
{
    private void Awake()
    {
        maxAmmo = 8;
        currentAmmo = maxAmmo;
    }

    public override void Shoot()
    {
        if (!HasAmmo) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;


        var bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.shooterTag = "Player";
        }

        currentAmmo--;
    }

}
