using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private bool isAiming = false;
    private bool hasGun = false;
    public Sprite playerWithGunSprite; // Sprite for player when holding the gun

    public GameObject placedGunPrefab;   // For dynamic scene placement
    public GameObject droppedGunPrefab;  //  This one will be used when dropping

    public Sprite playerWithoutGunSprite; // Unarmed player sprite
    public float dropForce = 8f; // Throw speed

    public AnimatorOverrideController armedOverrideController;
    private RuntimeAnimatorController originalController;

    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    private bool isRolling = false;
    public bool IsRolling => isRolling;


    private bool canRoll = true;
    private float rollInvulDuration = 0.5f;
    private float rollCooldown = 3f;
    public float rollSpeed = 10f; 
    private Collider2D playerCollider;








    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>(); 

        // Save original (unarmed) animator controller
        originalController = animator.runtimeAnimatorController;
    }


    void Update()
    {
        // Set the "isWalking" parameter in the Animator to true if the player is moving (non-zero movement vector), false otherwise.
        // This is useful for switching between idle and walk animations.
        animator.SetBool("isWalking", movement.magnitude > 0);

        // If the player is currently in "lock and aim" mode (i.e., holding the aim button):
        if (isAiming)
        {
            // Get the current mouse position on the screen and convert it to world coordinates.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Calculate the direction vector from the player to the mouse position.
            Vector2 direction = mousePos - transform.position;

            // Calculate the angle (in degrees) between the x-axis and the direction vector using atan2.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate the player to face the calculated angle (point toward the mouse).
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // If the player is not aiming, rotate based on the movement direction.
            if (movement != Vector2.zero)
            {
                // Calculate the angle from the movement vector.
                float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;

                // Rotate the player to face the direction they are moving in.
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }





    void FixedUpdate()
    {
        if (isRolling)
            return; // Don't apply movement if rolling

        rb.velocity = isAiming ? Vector2.zero : movement * moveSpeed;
    }



    // Movement
    public void OnMovement(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    // Other Inputs
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!hasGun || !context.performed) return;

        // Spawn bullet slightly in front of player
        Vector3 spawnPos = transform.position + transform.right * 0.5f;
        spawnPos.z = 0f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * bulletSpeed;
    }



    public void OnRoll()
    {
        if (!canRoll) return;

        Debug.Log("Rolling!");
        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        isRolling = true;
        canRoll = false;

        Debug.Log("Player is invulnerable");

        // Disable collision with all bullets
        Collider2D[] allBullets = FindObjectsOfType<Collider2D>();
        foreach (var bulletCol in allBullets)
        {
            if (bulletCol.CompareTag("Bullet"))
                Physics2D.IgnoreCollision(playerCollider, bulletCol, true);
        }

        // Determine roll direction
        Vector2 rollDir = isAiming ? transform.right.normalized : movement.normalized;
        if (rollDir == Vector2.zero)
            rollDir = transform.right.normalized;

        rb.velocity = rollDir * rollSpeed;

        yield return new WaitForSeconds(rollInvulDuration);

        rb.velocity = Vector2.zero;
        isRolling = false;
        Debug.Log("Player is vulnerable again");

        // Re-enable collision with bullets
        foreach (var bulletCol in allBullets)
        {
            if (bulletCol != null && bulletCol.CompareTag("Bullet"))
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCol, false);
            }
        }


        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
        Debug.Log("Roll is ready again");
    }



    public void OnParry()
    {
        Debug.Log("Parry!");
    }

    public void OnDropGun()
    {
        if (!hasGun) return;

        Debug.Log("Dropping gun!");

        Vector3 spawnOffset = transform.right * 0.5f;
        GameObject droppedGun = Instantiate(droppedGunPrefab, transform.position + spawnOffset, transform.rotation);

        Rigidbody2D gunRb = droppedGun.GetComponent<Rigidbody2D>();
        gunRb.bodyType = RigidbodyType2D.Dynamic;
        gunRb.gravityScale = 0f;
        gunRb.velocity = transform.right * dropForce;
        gunRb.angularVelocity = 360f;

        droppedGun.GetComponent<DroppedGun>()?.MarkAsDropped();

        hasGun = false;
        animator.runtimeAnimatorController = originalController;
        
    }


    public void OnLockAndAim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAiming = true;
            Debug.Log("Started aiming");
        }
        else if (context.canceled)
        {
            isAiming = false;
            Debug.Log("Stopped aiming");
        }
    }


    public void OnPause()
    {
        Debug.Log("Game paused!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasGun && other.CompareTag("Gun"))
        {
            Debug.Log("Picking up gun!");
            PickupGun();
            other.gameObject.SetActive(false);
        }
    }



    public void PickupGun()
    {
        hasGun = true;

        // Reset runtime controller to force refresh
        animator.runtimeAnimatorController = null;
        animator.runtimeAnimatorController = armedOverrideController;

        Debug.Log("Gun picked up — controller set to armedOverrideController");
    }



}
