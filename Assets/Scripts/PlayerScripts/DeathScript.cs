using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DeathScript : MonoBehaviour
{
    public bool isPlayer = false;
    public GameObject pauseMenuCanvas;
    public GameObject resumeButton;
    public GameObject restartButton; // ✅ Nuevo campo
    public PlayerInput playerInput;

    public void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Player has died");

            // Mostrar menú de pausa
            if (pauseMenuCanvas != null)
            {
                pauseMenuCanvas.SetActive(true);
                Time.timeScale = 0f;
            }

            // Ocultar botón Resume
            if (resumeButton != null)
                resumeButton.SetActive(false);

            // Cambiar a mapa de acción UI
            if (playerInput != null)
                playerInput.SwitchCurrentActionMap("UI");

            // ✅ Seleccionar botón Restart si está asignado
            if (restartButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(restartButton);
            }

            // Desactivar movimiento del jugador
            var movement = GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;
        }
        else
        {
            Debug.Log("Enemy destroyed");
            Destroy(gameObject); // Enemigo sí se destruye
        }
    }
}
