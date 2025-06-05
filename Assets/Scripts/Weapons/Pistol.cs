using UnityEngine;


public class Pistol : GunBase
{

    protected override void Awake()
    {
        base.Awake();               // Call base setup 
        maxAmmo = 8;                
        currentAmmo = maxAmmo;
    }

    // Shooting logic for the pistol (one shot per trigger press)
    public override void Shoot()
    {
        if (!HasAmmo) return;       

        // Play shooting sound
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Spawn and launch bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;

        // Assign shooter tag for collision logic
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
