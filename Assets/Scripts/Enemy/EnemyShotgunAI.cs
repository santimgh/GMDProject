using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShotgunAI : MonoBehaviour
{
    public float visionRadius = 8f;
    public float loseSightRadius = 12f;
    public float shootingCooldown = 2f;

    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float spreadAngle = 15f; 

    public AudioClip shootSound;
    private AudioSource audioSource;

    private Transform player;
    private EnemyState currentState = EnemyState.Idle;
    private float lastShotTime;
    private bool hasSeenPlayer = false;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        EnemyManager.Instance.RegisterEnemy();
        audioSource = GetComponent<AudioSource>();

        // Allow NavMeshAgent to work in a 2D environment
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                agent.ResetPath();
                // Transition to shooting if player is in sight and within vision radius
                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    hasSeenPlayer = true;
                    currentState = EnemyState.Shooting;
                }
                break;

            case EnemyState.Shooting:
                LookAtPlayer();

                // Fire only if cooldown has passed
                if (Time.time - lastShotTime > shootingCooldown)
                {
                    ShootShotgun();
                    lastShotTime = Time.time;
                }

                // Switch to chasing if player is no longer visible
                if (!CanSeePlayer())
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                agent.SetDestination(player.position);

                // Resume shooting if player is visible again
                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    currentState = EnemyState.Shooting;
                }
                break;
        }
    }

    // Raycast to check for line of sight to the player
    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Walls", "Player"));

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    // Rotate the enemy to face the player
    void LookAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Fires three bullets with spread effect
    void ShootShotgun()
    {
        Vector3 spawnPos = transform.position + transform.right * 0.5f;

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        for (int i = -1; i <= 1; i++)
        {
            // Create bullet spread by modifying angle and spawn offset
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, i * spreadAngle);
            Vector3 offset = transform.up * i * 0.1f;

            GameObject bullet = Instantiate(bulletPrefab, spawnPos + offset, rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = rotation * Vector3.right * bulletSpeed;

            var bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.shooterTag = "Enemy";
            }
        }
    }

    // Visualize vision radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
    }
}
