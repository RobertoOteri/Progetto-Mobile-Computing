using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 2; 

    [Header("--- Audio ---")]
    [SerializeField] private AudioClip healSFX; // Trascina il suono (mela o pozione) nell'Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Controlla se a toccare la pozione è il Giocatore
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Cura il giocatore solo se non ha già la vita al massimo
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    playerHealth.ChangeHealth(healAmount);

                    // Riproduce il suono normale assegnato a questa pozione/mela
                    if (healSFX != null && AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX(healSFX);
                    }
                    
                    // Distrugge l'oggetto dalla mappa dopo la raccolta
                    Destroy(gameObject);
                }
            }
        }
    }
}