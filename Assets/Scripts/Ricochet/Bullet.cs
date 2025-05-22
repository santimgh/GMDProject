using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int maxRicochets = 3;
    private int ricochetCount = 0;

    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        Destroy(gameObject, 5f); // auto-destroy after 5s
    }

    void Update()
    {
        // Bullet direction line effect
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(rb.velocity.normalized * 0.4f); // Short visible trail

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ricochet only on non-player collisions
        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null && !player.IsRolling)
            {
                Debug.Log("Bullet hit player!");
                // TODO: damage logic
            }
            else
            {
                Debug.Log("Bullet ignored player (rolling)");
            }

            Destroy(gameObject);
            return;
        }

        // Only bounce if contact exists
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal;
            rb.velocity = Vector2.Reflect(rb.velocity, normal);
        }


        ricochetCount++;
        if (ricochetCount >= maxRicochets)
        {
            Destroy(gameObject);
        }
    }





}
