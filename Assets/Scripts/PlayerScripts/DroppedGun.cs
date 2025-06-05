using UnityEngine;

public class DroppedGun : MonoBehaviour
{
    private float pickupDelay = 0.5f;
    private float lifeTime = 10f;
    private Collider2D col;
    private bool wasDropped = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
        Invoke(nameof(EnableCollider), pickupDelay);
    }

    void EnableCollider()
    {
        col.enabled = true;

        // Only start the destruction timer after enabling the collider
        // and only if this gun was dropped by the player
        if (wasDropped)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!col.enabled) return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>()?.PickupGun();
            Destroy(gameObject);
        }
    }

    // Called by PlayerMovement when dropping
    public void MarkAsDropped()
    {
        wasDropped = true;
    }
}