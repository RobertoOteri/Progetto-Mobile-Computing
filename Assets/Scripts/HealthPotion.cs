using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 2; 

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
                    
                    // Distrugge la pozione dalla mappa dopo che è stata raccolta
                    Destroy(gameObject);
                }
            }
        }
    }
}