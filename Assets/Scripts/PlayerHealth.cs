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
        if (isDead) return;

        currentHealth += amount;

        if (amount < 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHurtSound();
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