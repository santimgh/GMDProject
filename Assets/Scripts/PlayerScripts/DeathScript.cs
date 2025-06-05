using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DeathScript : MonoBehaviour
{
    public bool isPlayer = false;
    public GameObject pauseMenuCanvas;
    public GameObject resumeButton;
    public GameObject restartButton; 
    public PlayerInput playerInput;

    public void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Player has died");
            EnemyManager.Instance.playerIsDead = true;

            // Show pause menu
            if (pauseMenuCanvas != null)
            {
                pauseMenuCanvas.SetActive(true);
                Time.timeScale = 0f;

                // Show death message
                var ui = FindObjectOfType<GameplayUI>();
                ui?.ShowDeathMessage();

            }


            // Hide resume button
            if (resumeButton != null)
                resumeButton.SetActive(false);

            // Change Action map to UI
            if (playerInput != null)
                playerInput.SwitchCurrentActionMap("UI");

            
            if (restartButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(restartButton);
            }

            // Disable movement for player
            var movement = GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;
        }
        else
        {
            Debug.Log("Enemy destroyed");
            EnemyManager.Instance?.UnregisterEnemy(); // Unregister the enemy from the counter
            Destroy(gameObject); 
        }
    }
}
