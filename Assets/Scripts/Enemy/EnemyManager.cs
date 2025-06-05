using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public bool playerIsDead = false;

    private int enemiesRemaining = 0;

    void Awake()
    {
        // Singleton pattern to allow global access
        Instance = this;
    }

    public void RegisterEnemy()
    {
        enemiesRemaining++;
        // Update UI to reflect current number of enemies
        FindObjectOfType<GameplayUI>()?.UpdateEnemyCount(enemiesRemaining);
    }

    public void UnregisterEnemy()
    {
        enemiesRemaining--;
        if (enemiesRemaining < 0) enemiesRemaining = 0;

        // Update UI again after one enemy is removed
        FindObjectOfType<GameplayUI>()?.UpdateEnemyCount(enemiesRemaining);

        // Show victory screen ONLY if player is still alive
        if (enemiesRemaining == 0 && !playerIsDead)
        {
            Debug.Log("Every enemy Has been killed!");
            FindObjectOfType<GameplayUI>()?.ShowVictoryScreen();
            Time.timeScale = 0f; // Pause the game
        }
    }
}
