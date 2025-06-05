using UnityEngine;

public class BreakableWindow : MonoBehaviour
{
    public GameObject intactGlass;         // Sprite del cristal entero
    public GameObject brokenGlassPrefab;   // Prefab de los fragmentos
    public Transform spawnPoint;           // Donde instanciar los fragmentos

    private bool isBroken = false;

    public void Break(Vector2 direction)
    {
        if (isBroken) return;
        isBroken = true;

        if (intactGlass != null)
            intactGlass.SetActive(false); // Oculta el sprite intacto

        //  Desactiva colisionador de la ventana
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (brokenGlassPrefab != null && spawnPoint != null)
        {
            GameObject broken = Instantiate(brokenGlassPrefab, spawnPoint.position, Quaternion.identity);

            foreach (Rigidbody2D piece in broken.GetComponentsInChildren<Rigidbody2D>())
            {
                piece.AddForce(direction.normalized * Random.Range(2f, 5f), ForceMode2D.Impulse);
                piece.AddTorque(Random.Range(-1f, 1f));
            }
        }
    }

}
