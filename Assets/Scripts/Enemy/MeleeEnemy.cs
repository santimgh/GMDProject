using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// State enum for melee enemy behavior
public enum MeleeEnemyState
{
    Idle,
    Chasing,
    Attacking
}


public class MeleeEnemy : MonoBehaviour
{
    public float visionRadius = 8f;
    public float loseSightRadius = 12f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int damage = 1;

    private Transform player;
    private NavMeshAgent agent;
    private MeleeEnemyState currentState = MeleeEnemyState.Idle;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        EnemyManager.Instance.RegisterEnemy();

        // Needed for NavMeshAgent to work correctly in 2D
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
            case MeleeEnemyState.Idle:
                agent.ResetPath();

                // Start chasing if player is seen within vision radius
                if (distanceToPlayer <= visionRadius && CanSeePlayer())
                {
                    currentState = MeleeEnemyState.Chasing;
                }
                break;

            case MeleeEnemyState.Chasing:
                agent.SetDestination(player.position);
                LookAtPlayer();

                // Switch to attacking when in range
                if (distanceToPlayer <= attackRange)
                {
                    currentState = MeleeEnemyState.Attacking;
                }
                // Return to idle if player is out of sight and far away
                else if (!CanSeePlayer() && distanceToPlayer > loseSightRadius)
                {
                    currentState = MeleeEnemyState.Idle;
                }
                break;

            case MeleeEnemyState.Attacking:
                agent.ResetPath();
                LookAtPlayer();

                // Attack only after cooldown
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }

                // Resume chasing if player moves out of range
                if (distanceToPlayer > attackRange)
                {
                    currentState = MeleeEnemyState.Chasing;
                }
                break;
        }
    }

    // Rotate enemy to face the player
    void LookAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Damage logic when attacking the player
    void AttackPlayer()
    {
        Debug.Log("Player hit!");

        var death = player.GetComponent<DeathScript>();
        if (death != null && death.isPlayer)
        {
            death.Die();
        }
    }

    // Line-of-sight check using raycast
    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Walls", "Player"));

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    // Draw vision, chase, and attack ranges in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
