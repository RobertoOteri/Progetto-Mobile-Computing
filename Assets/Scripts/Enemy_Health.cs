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

    // Notifica ad altri script (come il Boss) che questo nemico è morto
    public event Action OnDeath;

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

        // CONTROLLO IMMUNITÀ TRASFORMAZIONE BOSS
        if (amount < 0) // Se sta subendo danno
        {
            DemonBoss_Movement bossMovement = GetComponent<DemonBoss_Movement>();
            if (bossMovement != null && bossMovement.IsTransforming)
            {
                Debug.Log("[DEBUG] Il boss è in fase di trasformazione ed è immune ai colpi!");
                return; // Esce senza scalare la vita o attivare effetti di danno
            }
        }

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
        if (isDead) return;
        isDead = true;

        // CONTROLLO DIRETTO: Se questo nemico ha il componente DemonBoss_Movement, è il boss!
        if (GetComponent<DemonBoss_Movement>() != null)
        {
            NPCTriggerDialogue.IsBossDefeated = true;
        }

        // Avvisa chiunque sia in ascolto
        OnDeath?.Invoke();

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        if (sr != null) sr.color = originalColor;

        if (col != null) col.enabled = false;

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