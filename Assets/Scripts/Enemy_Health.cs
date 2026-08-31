using UnityEngine;
using System.Collections;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Effetto Flash Danno")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.12f;

    [Header("Audio Personalizzato Nemico")]
    public AudioClip hitSound; 
    public float hitSoundVolume = 0.5f;

    [Space]
    public float deathDelay = 0.7f;
    public AudioClip deathSound; 
    public float deathSoundVolume = 0.5f; 

    private Animator anim;
    private Collider2D col;
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            originalColor = sr.color;
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Se ha subito danno ed è ancora vivo
        if (amount < 0 && currentHealth > 0)
        {
            if (anim != null)
            {
                anim.SetTrigger("hit");
            }

            // Avvia il flash rosso
            if (sr != null)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(DamageFlashRoutine());
            }

            if (hitSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXWithVolume(hitSound, hitSoundVolume);
            }
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            Die(); 
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    private void Die()
    {
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            if (sr != null) sr.color = originalColor;

            Enemy_Movement movement = GetComponent<Enemy_Movement>();
            if (movement != null) 
            {
                movement.enabled = false;
            }

            if (anim != null)
            {
                anim.SetTrigger("die");
            }

            if (deathSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXWithVolume(deathSound, deathSoundVolume);
            }

            // Usa la variabile pubblica invece di un numero fisso
            Destroy(gameObject, deathDelay);
        }
    }
}