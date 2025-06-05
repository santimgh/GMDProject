using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    public GameObject startButton; // First selected
    public string firstLevelSceneName = "FirstPlayable"; 

    void Start()
    {
        Time.timeScale = 1f; // In case the game was paused
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
