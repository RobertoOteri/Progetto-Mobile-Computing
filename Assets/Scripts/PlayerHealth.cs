using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;

    private Animator anim;
    private bool isDead = false;

    private void Start()
    {
        // Prende l'Animator dallo stesso GameObject
        anim = GetComponent<Animator>();
    }

    public void ChangeHealth(int amount)
    {
        // Se è già morto, ignora ulteriori colpi
        if (isDead) return;

        currentHealth += amount;

        // Limita la vita tra 0 e maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // 1. Blocca i movimenti del giocatore
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 2. Azzera la velocità nel Rigidbody per fermare subito l'inerzia
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 3. Attiva il Trigger dell'animazione di morte
        if (anim != null)
        {
            anim.SetTrigger("die"); // Controlla che il Trigger nell'Animator si chiami proprio "die"
        }
    }
}