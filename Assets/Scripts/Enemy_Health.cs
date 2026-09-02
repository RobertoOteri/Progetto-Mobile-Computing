using UnityEngine;
using System.Collections;
using System;

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

    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged; // Parametri: currentHealth, maxHealth

    private Animator anim;
    private Collider2D col;
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isDead = false;

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
        if (isDead) return;

        if (amount < 0)
        {
            DemonBoss_Movement bossMovement = GetComponent<DemonBoss_Movement>();
            if (bossMovement != null && bossMovement.IsTransforming)
            {
                Debug.Log("[DEBUG] Il boss è in fase di trasformazione ed è immune ai colpi!");
                return;
            }
        }

        currentHealth += amount;

        if (amount < 0 && currentHealth > 0)
        {
            if (anim != null)
            {
                anim.SetTrigger("hit");
            }

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

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
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
        if (isDead) return;
        isDead = true;

        if (GetComponent<DemonBoss_Movement>() != null)
        {
            NPCTriggerDialogue.IsBossDefeated = true;
        }

        OnDeath?.Invoke();

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

        Destroy(gameObject, deathDelay);
    }
}