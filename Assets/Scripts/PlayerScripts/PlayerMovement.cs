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
    public Sprite playerWithoutGunSprite;
    public float dropForce = 8f;

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
    public float pushSlowdownFactor = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the original unarmed animation controller
        originalController = animator.runtimeAnimatorController;

       
        pauseMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if (isPaused) return;

        // Toggle walking animation state based on movement
        animator.SetBool("isWalking", movement.magnitude > 0);

        if (isAiming)
        {
            // Rotate toward mouse position while aiming
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = mousePos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (movement != Vector2.zero)
        {
            // Rotate toward movement direction
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        if (isPaused || isRolling)
            return;

        // Adjust movement speed if pushing an object
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
            gunHandler?.StopFiring();
            return;
        }

        if (context.started)
            gunHandler?.StartFiring();
        else if (context.canceled)
            gunHandler?.StopFiring();
    }

    public void OnRoll()
    {
        if (isPaused || !canRoll) return;
        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        isRolling = true;
        canRoll = false;

        // Visual feedback making a semi-transparent sprite
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // Temporarily ignore collisions with bullets
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
        spriteRenderer.color = originalColor;

        // Re-enable collisions with bullets
        foreach (var bulletCol in allBullets)
        {
            if (bulletCol != null && bulletCol.CompareTag("Bullet"))
                Physics2D.IgnoreCollision(playerCollider, bulletCol, false);
        }

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    // Runned out of time for implementing :(
    public void OnParry()
    {
        if (isPaused) return;
        Debug.Log("Parry!");
    }

    public void OnDropGun()
    {
        if (isPaused || gunHandler == null || !gunHandler.HasGun) return;

        Vector2 dropDir = transform.right;
        gunHandler.DropCurrentWeapon(dropDir, dropForce);

        // Reset animation controller to unarmed
        animator.runtimeAnimatorController = originalController;
    }

    public void OnLockAndAim(InputAction.CallbackContext context)
    {
        if (isPaused) return;

        isAiming = context.started;
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

        // Select resume button in UI
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

        // Force animation override reload
        animator.runtimeAnimatorController = null;
        animator.runtimeAnimatorController = armedOverrideController;
    }
}
