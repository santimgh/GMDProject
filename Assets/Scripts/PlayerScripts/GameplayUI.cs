using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class GameplayUI : MonoBehaviour
{
    public TextMeshProUGUI enemyCounterText;
    public GameObject ammoPanel;
    public TextMeshProUGUI ammoText;
    public Image weaponIconImage; 

    public GameObject pausePanel;
    public GameObject resumeButton;
    public TextMeshProUGUI dieMessageText;

    public GameObject winCanvas;                
    public TextMeshProUGUI winMessageText;     
    public GameObject winRestartButton;         
    public GameObject winQuitButton;            


    

    private void Start()
    {

        winCanvas.SetActive(false);
        if (dieMessageText != null)
            dieMessageText.text = "";

        UpdateEnemyCount(0);
        ShowAmmo(false);
    }

    public void ShowDeathMessage()
    {
        if (dieMessageText != null)
            dieMessageText.text = "YOU DIED!!!";
    }


    // Triggered when the player wins the game
    public void ShowVictoryScreen()
    {
        if (winCanvas != null)
            winCanvas.SetActive(true);  //  Show the win canvas

        if (winMessageText != null)
            winMessageText.text = "YOU WIN!!!";

        // Switch to UI input map 
        var input = FindObjectOfType<UnityEngine.InputSystem.PlayerInput>();
        if (input != null)
            input.SwitchCurrentActionMap("UI");

        // Auto-select the restart button for controller/keyboard users
        if (winRestartButton != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(winRestartButton);
        }
    }

    // Update the enemy counter UI
    public void UpdateEnemyCount(int count)
    {
        enemyCounterText.text = $"Enemies: {count}";
    }

    // Show or hide the ammo panel and weapon icon
    public void ShowAmmo(bool visible)
    {
        ammoPanel.SetActive(visible);
        weaponIconImage.gameObject.SetActive(visible);
    }

    // Update the ammo text 
    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"Ammo: {current}/{max}";
    }

    // Set the weapon icon sprite 
    public void SetWeaponIcon(Sprite icon)
    {
        weaponIconImage.sprite = icon;
    }
}
