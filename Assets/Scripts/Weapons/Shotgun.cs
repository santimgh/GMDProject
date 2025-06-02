using UnityEngine;

public class Shotgun : GunBase
{
    public float spreadAngle = 15f;

    private void Awake()
    {
        maxAmmo = 4;
        currentAmmo = maxAmmo;
    }

    public override void Shoot()
    {
        if (!HasAmmo) return;

        for (int i = -1; i <= 1; i++)
        {
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, i * spreadAngle);
            Vector3 offset = firePoint.up * i * 0.1f;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position + offset, rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = rotation * Vector3.right * bulletSpeed;

            var bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.shooterTag = "Player";
            }

        }
        
        currentAmmo--;
    }
}
