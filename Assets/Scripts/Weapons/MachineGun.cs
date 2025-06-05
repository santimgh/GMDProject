using UnityEngine;

public class MachineGun : GunBase
{
    public float fireRate = 0.1f;   // Time between consecutive shots
    private float lastFireTime;

    // Overrides the base to indicate this gun is automatic (can shoot by holding fire button)
    public override bool IsAutomatic => true;


    protected override void Awake()
    {
        base.Awake();               // Call base class setup (assign audio source, etc.)
        maxAmmo = 15;               
        currentAmmo = maxAmmo;      
    }


    public override void Shoot()
    {
        
        if (Time.time >= lastFireTime + fireRate && HasAmmo)
        {
            // Play shooting sound 
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // Spawn and launch the bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;

            lastFireTime = Time.time;

            // Assign shooter tag for friendly fire logic
            var bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.shooterTag = "Player";
            }

            // Reduce ammo and update UI
            currentAmmo--;
            FindObjectOfType<GameplayUI>().UpdateAmmo(currentAmmo, maxAmmo);
        }
    }
}
