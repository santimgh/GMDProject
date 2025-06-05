using UnityEngine;


public class Shotgun : GunBase
{
    public float spreadAngle = 15f; // Angle between each bullet

    
    protected override void Awake()
    {
        base.Awake();               
        maxAmmo = 4;                
        currentAmmo = maxAmmo;
    }

    
    public override void Shoot()
    {
        if (!HasAmmo) return;

        // Play firing sound
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Loop to spawn 3 bullets with spread (-1, 0, +1)
        for (int i = -1; i <= 1; i++)
        {
            // Adjust rotation for spread angle
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, i * spreadAngle);

            // Offset vertically to slightly spread the bullets visually
            Vector3 offset = firePoint.up * i * 0.1f;

            // Instantiate and launch bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position + offset, rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = rotation * Vector3.right * bulletSpeed;

            // Set shooter tag for friendly fire logic
            var bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.shooterTag = "Player";
            }
        }

        // Decrease ammo count and update UI
        currentAmmo--;
        FindObjectOfType<GameplayUI>().UpdateAmmo(currentAmmo, maxAmmo);
    }
}
