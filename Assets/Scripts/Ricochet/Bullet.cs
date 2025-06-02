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
   
        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null && !player.IsRolling)
            {
                Debug.Log("Bullet hit player!");
            
            }
            else
            {
                Debug.Log("Bullet ignored player (rolling)");
            }

            Destroy(gameObject);
            return;
        }

        
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
    
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag(shooterTag))
        {
            return;
        }

   
        if (shooterTag == "Player" && other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyDeath>()?.Die();
            Destroy(gameObject);
        }

        else if (shooterTag == "Enemy" && other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerMovement>();
            if (player != null && !player.IsRolling)
            {
                Debug.Log("Bullet hit player!");
              
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Bullet ignored player (rolling)");
            }
        }
  
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(gameObject);
        }
    }






}
