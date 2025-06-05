using UnityEngine;

public class BreakableWindow : MonoBehaviour
{
    public GameObject intactGlass;         // The full glass sprite
    public GameObject brokenGlassPrefab;   // Prefab with broken glass fragments
    public Transform spawnPoint;           // Where fragments should appear

    public AudioClip glassBreakSound;
    private AudioSource audioSource;

    private bool isBroken = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Call this to break the window with a force direction
    public void Break(Vector2 direction)
    {
        if (isBroken) return; // Prevent breaking more than once
        isBroken = true;

        // Hide the intact glass sprite
        if (intactGlass != null)
            intactGlass.SetActive(false);

        // Disable the window's collider to stop interactions
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Spawn broken glass pieces with physical force and sound
        if (brokenGlassPrefab != null && spawnPoint != null)
        {
            GameObject broken = Instantiate(brokenGlassPrefab, spawnPoint.position, Quaternion.identity);

            // Play glass-breaking sound
            if (glassBreakSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(glassBreakSound);
            }

            // Apply force and random torque to each glass piece
            foreach (Rigidbody2D piece in broken.GetComponentsInChildren<Rigidbody2D>())
            {
                piece.AddForce(direction.normalized * Random.Range(2f, 5f), ForceMode2D.Impulse);
                piece.AddTorque(Random.Range(-1f, 1f));
            }
        }
    }
}
