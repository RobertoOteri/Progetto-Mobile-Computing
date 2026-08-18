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
        anim = GetComponent<Animator>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;

        if (amount < 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHurtSound();
            }

            // Se subisce danno senza essere passato da Knockback, lancia comunque l'hit
            if (anim != null && currentHealth > 0)
            {
                anim.SetTrigger("hit");
            }
        }

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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDieSound();
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (anim != null)
        {
            anim.SetTrigger("die");
        }
    }
}