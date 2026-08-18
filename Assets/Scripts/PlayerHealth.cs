using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 5;

    [Header("Riferimenti")]
    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;

    [Header("Effetto Flash Danno")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.12f;

    private Animator anim;
    private bool isDead = false;
    public bool IsDead => isDead;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (playerSr == null) playerSr = GetComponent<SpriteRenderer>();

        if (playerSr != null)
        {
            originalColor = playerSr.color;
        }
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

            if (anim != null && currentHealth > 0)
            {
                anim.SetTrigger("hit");
            }

            // Avvia il flash rosso sul Player
            if (playerSr != null && currentHealth > 0)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(DamageFlashRoutine());
            }
        }

        // Limita la vita tra 0 e maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        playerSr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        playerSr.color = originalColor;
    }

    private void Die()
    {
        isDead = true;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        if (playerSr != null) playerSr.color = originalColor;

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