using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Impostazioni Vita")]
    public int maxHealth = 6;
    public int currentHealth;

    // Memoria statica per preservare i cuori tra le scene (porte/teleport)
    public static int sessionHealth = -1;

    [Header("Impostazioni Audio")]
    [Tooltip("Clip audio di morte (opzionale se gestito già in AudioManager)")]
    public AudioClip deathSound;

    [Header("Impostazioni Game Over & Morte")]
    [Tooltip("Tempo di attesa per far finire l'animazione di morte prima del Game Over")]
    public float deathAnimationDuration = 1.5f;
    [Tooltip("Ritardo prima di avviare il fade del Game Over")]
    public float gameOverDelay = 0.5f;

    // Proprietà per il controllo dei nemici (Enemy_Combat / Enemy_Movement)
    public bool IsDead => currentHealth <= 0;
    public bool isDead => currentHealth <= 0;

    private Animator anim;
    private PlayerMovement movement;
    private Rigidbody2D rb;
    private bool hasDied = false;

    private void Awake()
    {
        Instance = this;
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        // Recupera la vita dai passaggi di scena precedenti
        if (sessionHealth > 0)
        {
            currentHealth = sessionHealth;
        }
        else
        {
            currentHealth = maxHealth;
            sessionHealth = maxHealth;
        }
    }

    public void ChangeHealth(int amount)
    {
        if (IsDead && amount < 0) return;

        // Se subisce danno ma NON è ancora morto, suona l'hit sound
        if (amount < 0 && (currentHealth + amount > 0))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHurtSound();
            }

            if (anim != null)
            {
                anim.SetTrigger("hit");
            }
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Aggiorna il valore per il cambio scena
        sessionHealth = currentHealth;

        // Controllo Morte
        if (currentHealth <= 0 && !hasDied)
        {
            hasDied = true;
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        ChangeHealth(-damage);
    }

    private void Die()
    {
        Debug.Log("Player sconfitto.");

        // 1. Suona l'effetto sonoro di morte
        if (deathSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(deathSound, 6f);
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDieSound();
        }

        // 2. Ferma subito la fisica e i movimenti del Player
        if (movement != null)
        {
            movement.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 3. Riproduce l'animazione di morte
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        // 4. Avvia la sequenza con attesa per l'animazione e fade
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        // Aspetta che l'animazione di morte finisca
        yield return new WaitForSeconds(deathAnimationDuration);

        // Eventuale ritardo aggiuntivo prima del fade
        if (gameOverDelay > 0f)
        {
            yield return new WaitForSeconds(gameOverDelay);
        }

        // Mostra la schermata di Game Over con la sua dissolvenza
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOver();
        }
    }
}