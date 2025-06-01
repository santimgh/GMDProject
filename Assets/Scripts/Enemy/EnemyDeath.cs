using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public void Die()
    {
        Debug.Log("Enemigo destruido");
        Destroy(gameObject);
    }
}
