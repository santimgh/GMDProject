using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int maxRicochets = 3;
    private int ricochetCount = 0;

    public string shooterTag; // Used to prevent friendly fire

    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // Automatically destroy the bullet after 5 seconds to clean up
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Draw a short trail using LineRenderer
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(rb.velocity.normalized * 0.4f);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        // Destroy bullet on hitting movable objects 
        if (other.CompareTag("Movable"))
        {
            Debug.Log("Bullet hit a movable object!");
            Destroy(gameObject);
            return;
        }

        // Break windows on hit
        if (other.CompareTag("Window"))
        {
            BreakableWindow window = other.GetComponent<BreakableWindow>();
            if (window != null)
            {
                Vector2 bulletDir = rb.velocity.normalized;
                window.Break(bulletDir);
            }

            Destroy(gameObject);
            return;
        }

        // Damage player if not rolling
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerMovement>();
            if (player != null && !player.IsRolling)
            {
                Debug.Log("Bullet hit player! Killing.");
                other.GetComponent<DeathScript>()?.Die();
                Destroy(gameObject);
                return;
            }
            else
            {
                Debug.Log("Bullet ignored player (rolling)");
            }
        }

        // Damage enemy only if bullet wasn't fired by another enemy
        else if (other.CompareTag("Enemy"))
        {
            if (shooterTag != "Enemy")
            {
                Debug.Log("Bullet hit enemy!");
                other.GetComponent<DeathScript>()?.Die();
                Destroy(gameObject);
                return;
            }
        }

        // Ricochet logic
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal;

            // Reflect bullet's velocity based on surface normal
            rb.velocity = Vector2.Reflect(rb.velocity, normal);
            ricochetCount++;

            // Destroy bullet if ricochet limit is exceeded
            if (ricochetCount >= maxRicochets)
            {
                Destroy(gameObject);
            }
        }
    }
}
