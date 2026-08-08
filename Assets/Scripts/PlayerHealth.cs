using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;

    private Animator anim;
    private bool isDead = false;
    public bool IsDead => isDead;

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

        // 1. Disattiva il Collider2D così i nemici NON lo vedono/colpiscono più
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 2. Blocca i movimenti del giocatore
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 3. Azzera la velocità
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 4. Animazione di morte
        if (anim != null)
        {
            anim.SetTrigger("die");
        }
    }
}