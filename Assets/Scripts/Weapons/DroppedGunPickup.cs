using UnityEngine;

public class DroppedGunPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var handler = other.GetComponent<PlayerGunHandler>();
            if (handler != null)
            {
                // ⏱️ No recoger si fue soltada hace menos de 0.2 segundos
                if (Time.time - handler.lastDroppedTime < 0.2f)
                {
                    Debug.Log("⏳ Ignorando recogida inmediata tras soltar.");
                    return;
                }

                handler.EquipWeapon(gameObject);
                Debug.Log("🔄 Arma recogida del suelo");
            }
        }
    }

}
