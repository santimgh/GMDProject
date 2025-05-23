using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private Transform player;
    private EnemyState currentState = EnemyState.Idle;
    private float lastShotTime;
    private bool hasSeenPlayer = false;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        // Estos dos son necesarios para que el navmesh funcione bien en 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                agent.ResetPath();
                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    hasSeenPlayer = true;
                    currentState = EnemyState.Shooting;
                }
                break;

            case EnemyState.Shooting:
                LookAtPlayer();

                if (Time.time - lastShotTime > shootingCooldown)
                {
                    ShootAtPlayer();
                    lastShotTime = Time.time;
                }

                if (!CanSeePlayer())
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                agent.SetDestination(player.position);

                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    currentState = EnemyState.Shooting;
                }
                break;
        }
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Walls", "Player"));

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    void LookAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void ShootAtPlayer()
    {
        Vector3 spawnPos = transform.position + transform.right * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
    }
}
