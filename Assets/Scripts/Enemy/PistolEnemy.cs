using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Enum for enemy behavior states
public enum EnemyState
{
    Idle,
    Shooting,
    Chasing
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public float visionRadius = 8f;
    public float loseSightRadius = 12f;
    public float shootingCooldown = 1f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

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

        // Required to make NavMeshAgent work properly in 2D
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

                // Transition to shooting if player is in sight
                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    hasSeenPlayer = true;
                    currentState = EnemyState.Shooting;
                }
                break;

            case EnemyState.Shooting:
                LookAtPlayer();

                // Shoot only when cooldown has elapsed
                if (Time.time - lastShotTime > shootingCooldown)
                {
                    ShootAtPlayer();
                    lastShotTime = Time.time;
                }

                // If player is no longer visible, start chasing
                if (!CanSeePlayer())
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                // Move towards the player's last known position
                agent.SetDestination(player.position);

                // Resume shooting if player is visible again
                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    currentState = EnemyState.Shooting;
                }
                break;
        }
    }

    // Check line of sight using a 2D raycast
    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Walls", "Player"));

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    // Rotate enemy to face the player
    void LookAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Spawn and shoot a bullet toward the player
    void ShootAtPlayer()
    {
        Vector3 spawnPos = transform.position + transform.right * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;

        var bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.shooterTag = "Enemy";
        }

        // Play shooting sound 
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
    }
}
