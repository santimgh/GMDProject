using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int maxRicochets = 3;
    private int ricochetCount = 0;

    public string shooterTag;

    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(rb.velocity.normalized * 0.4f);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        Debug.Log("Bullet collided with: " + collision.gameObject.name + " (Layer: " + LayerMask.LayerToName(collision.gameObject.layer) + ")");

        if (other.CompareTag("Movable"))
        {
            Debug.Log("Bullet hit a movable object!");
            Destroy(gameObject);
            return;
        }

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


        // Player
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

        // Enemy
        else if (other.CompareTag("Enemy"))
        {
            if (shooterTag != "Enemy") // ✅ Solo mata enemigos si NO fue disparada por otro enemigo
            {
                Debug.Log("Bullet hit enemy!");
                other.GetComponent<DeathScript>()?.Die();
                Destroy(gameObject);
                return;
            }
        }


        // Rebote
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal;
            rb.velocity = Vector2.Reflect(rb.velocity, normal);

            ricochetCount++;

            //  Solo destruir si excede rebotes permitidos
            if (ricochetCount >= maxRicochets)
            {
                Destroy(gameObject);
            }
        }



    }



}
