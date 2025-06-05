using UnityEngine;
using System.Collections;

// Handles player weapon logic: equipping, shooting, and dropping guns
public class PlayerGunHandler : MonoBehaviour
{
    public Transform weaponHoldPoint;        // Where the gun will be attached on the player
    private GunBase currentGun;              // Reference to the currently equipped gun

    public bool HasGun => currentGun != null;

    private bool isFiring = false;
    public float lastDroppedTime = -1f;

    void Update()
    {
        // For automatic weapons: keep shooting while fire input is held
        if (isFiring && currentGun != null && currentGun.IsAutomatic)
        {
            currentGun.Shoot();
        }
    }

   
    public void EquipWeapon(GameObject weapon)
    {
        if (currentGun != null)
        {
            return;
        }

        currentGun = weapon.GetComponent<GunBase>();

        // Update UI with new weapon info
        var ui = FindObjectOfType<GameplayUI>();
        ui.ShowAmmo(true);
        ui.UpdateAmmo(currentGun.CurrentAmmo, currentGun.maxAmmo);
        ui.SetWeaponIcon(currentGun.weaponIcon);

        // Attach weapon to the hold point
        weapon.transform.SetParent(weaponHoldPoint);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        // Disable physics on equipped weapon
        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // Disable collider while held
        Collider2D col = weapon.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log(" Arma equipada: " + currentGun.GetType().Name);
    }


    public void StartFiring()
    {
        isFiring = true;

        // Semi-automatic weapons shoot once per input
        if (currentGun != null && !currentGun.IsAutomatic)
        {
            currentGun.Shoot();
        }
    }

    // Called when fire input is released
    public void StopFiring()
    {
        isFiring = false;
    }

  
    public void DropCurrentWeapon(Vector2 dropDirection, float dropForce)
    {
        if (currentGun == null)
        {
            return;
        }

        GameObject droppedWeapon = currentGun.gameObject;

        // Detach weapon from player
        droppedWeapon.transform.SetParent(null);

        // Reactivate physics so it can fall and spin
        Rigidbody2D rb = droppedWeapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.gravityScale = 0f;
            rb.velocity = dropDirection.normalized * dropForce;
            rb.angularVelocity = 360f;
        }

        // Re-enable collider, set to solid, then trigger after a delay
        Collider2D col = droppedWeapon.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
            StartCoroutine(EnablePickupTriggerAfterDelay(col, 3f)); // Prevent instant re-pickup
        }

        currentGun = null;
        lastDroppedTime = Time.time;

        // Hide ammo UI when no gun is held
        FindObjectOfType<GameplayUI>().ShowAmmo(false);

    }

    // After delay, make the dropped gun pickup-able again by switching to trigger mode
    private IEnumerator EnablePickupTriggerAfterDelay(Collider2D col, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null)
        {
            col.isTrigger = true;
        
        }
    }
}
