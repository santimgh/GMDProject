using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems; 
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private bool isAiming = false;
    public Sprite playerWithoutGunSprite; // Unarmed player sprite
    public float dropForce = 8f; // Throw speed

    private SpriteRenderer spriteRenderer;


    public AnimatorOverrideController armedOverrideController;
    private RuntimeAnimatorController originalController;

    public GameObject pauseMenuCanvas;

    private bool isPaused = false;

    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    private bool isRolling = false;
    public bool IsRolling => isRolling;

    public GameObject resumeButton;

    public GameObject quitButton;

    public PlayerInput playerInput;


    private bool canRoll = true;
    private float rollInvulDuration = 0.5f;
    private float rollCooldown = 3f;
    public float rollSpeed = 10f; 
    private Collider2D playerCollider;

    public PlayerGunHandler gunHandler;

    private bool isPushingObject = false;
    public float pushSlowdownFactor = 0.5f; // 50% slower



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


        // Save original (unarmed) animator controller
        originalController = animator.runtimeAnimatorController;
        pauseMenuCanvas.SetActive(false);

    }


    void Update()
    {   
        if (isPaused) return;
        // Set the "isWalking" parameter in the Animator to true if the player is moving (non-zero movement vector), false otherwise.
        // This is useful for switching between idle and walk animations.
        animator.SetBool("isWalking", movement.magnitude > 0);

        // If the player is currently in "lock and aim" mode 
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
        if (isPaused || isRolling)
            return; // Don't apply movement if rolling

        rb.velocity = isAiming ? Vector2.zero : movement * moveSpeed;
        float effectiveSpeed = isPushingObject ? moveSpeed * pushSlowdownFactor : moveSpeed;
        rb.velocity = isAiming ? Vector2.zero : movement * effectiveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Movable"))
        {
            isPushingObject = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Movable"))
        {
            isPushingObject = false;
        }
    }



    public void OnMovement(InputAction.CallbackContext context)
    {
        if (isPaused) return;
        movement = context.ReadValue<Vector2>();
    }

    
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            gunHandler?.StopFiring(); // Just in case the button is being held
            return;
        }

        if (context.started)
        {
            gunHandler?.StartFiring();
        }
        else if (context.canceled)
        {
            gunHandler?.StopFiring();
        }
    }







    public void OnRoll()
    {
        if (isPaused) return;

        if (!canRoll) return;

        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        isRolling = true;
        canRoll = false;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // Invulnerability
        Collider2D[] allBullets = FindObjectsOfType<Collider2D>();
        foreach (var bulletCol in allBullets)
        {
            if (bulletCol.CompareTag("Bullet"))
                Physics2D.IgnoreCollision(playerCollider, bulletCol, true);
        }

        Vector2 rollDir = isAiming ? transform.right.normalized : movement.normalized;
        if (rollDir == Vector2.zero)
            rollDir = transform.right.normalized;

        rb.velocity = rollDir * rollSpeed;

        yield return new WaitForSeconds(rollInvulDuration);

        rb.velocity = Vector2.zero;
        isRolling = false;

        // Restore color
        spriteRenderer.color = originalColor;

        foreach (var bulletCol in allBullets)
        {
            if (bulletCol != null && bulletCol.CompareTag("Bullet"))
            {
                Physics2D.IgnoreCollision(playerCollider, bulletCol, false);
            }
        }

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }




    public void OnParry()
    {
        if (isPaused) return;
    }

    public void OnDropGun()
    {
        if (isPaused) return;

        if (gunHandler == null || !gunHandler.HasGun) return;

        Vector2 dropDir = transform.right;
        gunHandler.DropCurrentWeapon(dropDir, dropForce);

        animator.runtimeAnimatorController = originalController;
    }
    



    public void OnLockAndAim(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        if (context.started)
        {
            isAiming = true;
        }
        else if (context.canceled)
        {
            isAiming = false;
        }
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isPaused) ResumeGame();
        else PauseGame();
    }



    private void PauseGame()
    {

        Time.timeScale = 0f;
        isPaused = true;

        pauseMenuCanvas.SetActive(true);

        playerInput.SwitchCurrentActionMap("UI");

        gunHandler?.StopFiring();

        StartCoroutine(SelectResumeNextFrame());
    }

    private IEnumerator SelectResumeNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    private void ResumeGame()
    {

        Time.timeScale = 1f;
        isPaused = false;

        pauseMenuCanvas.SetActive(false);

        playerInput.SwitchCurrentActionMap("Default");
    }



    public void QuitGame()
    {
        Debug.Log("Returning to Main Menu...");
        SceneManager.LoadScene("MainMenuUI"); 
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }





    public void PickupGun()
    {

        if (isPaused) return;
        // Reset runtime controller to force refresh
        animator.runtimeAnimatorController = null;
        animator.runtimeAnimatorController = armedOverrideController;

        Debug.Log("Gun picked up — controller set to armedOverrideController");
    }



}