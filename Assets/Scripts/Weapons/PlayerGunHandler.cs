using UnityEngine;
using System.Collections;

public class PlayerGunHandler : MonoBehaviour
{
    public Transform weaponHoldPoint;
    private GunBase currentGun;

    public bool HasGun => currentGun != null;

    private bool isFiring = false;

    public float lastDroppedTime = -1f;


    void Update()
    {
        if (isFiring && currentGun != null && currentGun.IsAutomatic)
        {
            currentGun.Shoot();
        }
    }

    public void EquipWeapon(GameObject weapon)
    {
        if (currentGun != null)
        {
            Debug.Log(" Ya tienes un arma equipada. Suéltala primero.");
            return;
        }

        currentGun = weapon.GetComponent<GunBase>();

        
        weapon.transform.SetParent(weaponHoldPoint);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

       
        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // Desactivar collider
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

        if (currentGun != null && !currentGun.IsAutomatic)
        {
            currentGun.Shoot(); 
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }

   public void DropCurrentWeapon(Vector2 dropDirection, float dropForce)
    {
        if (currentGun == null)
        {
            Debug.LogWarning(" No hay arma equipada.");
            return;
        }

        GameObject droppedWeapon = currentGun.gameObject;

        
        droppedWeapon.transform.SetParent(null);

    
        Rigidbody2D rb = droppedWeapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.gravityScale = 0f;
            rb.velocity = dropDirection.normalized * dropForce;
            rb.angularVelocity = 360f;
        }

       
        Collider2D col = droppedWeapon.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; 
            StartCoroutine(EnablePickupTriggerAfterDelay(col, 3f)); 

        }

        currentGun = null;
        lastDroppedTime = Time.time;

        Debug.Log(" Arma soltada: " + droppedWeapon.name);
    }


    private IEnumerator EnablePickupTriggerAfterDelay(Collider2D col, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null)
        {
            col.isTrigger = true;
            Debug.Log(" Collider cambiado a trigger para permitir recogida");
        }
    }

}
