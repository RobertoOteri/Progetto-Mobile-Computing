using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth = 5;

    [Header("Riferimenti")]
    public SpriteRenderer playerSr;
    public PlayerMovement playerMovement;
    public Player_Combat playerCombat;

    [Header("Effetto Flash Danno")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.12f;

    [Header("Impostazioni Game Over")]
    [Tooltip("Tempo di attesa prima che appaia il Game Over (per lasciare finire l'animazione di morte)")]
    public float gameOverDelay = 1.5f;

    private Animator anim;
    private bool isDead = false;
    public bool IsDead => isDead;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private Coroutine hitResetCoroutine;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (playerCombat == null) playerCombat = GetComponent<Player_Combat>();
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
                anim.ResetTrigger("hit");
                anim.SetTrigger("hit");

                if (hitResetCoroutine != null) StopCoroutine(hitResetCoroutine);
                hitResetCoroutine = StartCoroutine(ResetHitTriggerRoutine());
            }

            if (playerSr != null && currentHealth > 0)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(DamageFlashRoutine());
            }
        }

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
            AudioManager.Instance.StopWalkSound();
        }

        // 1. DISARMO TOTALE: resetta bool nell'Animator e nasconde tutti gli oggetti arma
        if (playerCombat == null) playerCombat = GetComponent<Player_Combat>();
        if (playerCombat != null)
        {
            playerCombat.ForceDisarmOnDeath();
        }

        // 2. Disabilita script di sparo
        Player_Gun gun = GetComponent<Player_Gun>();
        if (gun != null) gun.enabled = false;

        Player_Rifle rifle = GetComponent<Player_Rifle>();
        if (rifle != null) rifle.enabled = false;

        // 3. Ferma il movimento fisico e disabilita lo script
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.StopMovement();
            playerMovement.enabled = false;
        }

        // 4. Nasconde i controlli touch (Joystick e Tasto SHOOT/ATTACK)
        GameObject mobileCanvas = GameObject.Find("MobileControls");
        if (mobileCanvas == null) mobileCanvas = GameObject.Find("MobileButtonsCanvas");
        if (mobileCanvas != null) mobileCanvas.SetActive(false);

        // 5. Avvia l'animazione di morte
        if (anim != null)
        {
            anim.ResetTrigger("hit");
            anim.SetTrigger("die");
        }

        // 6. Avvia il delay prima del popup di Game Over
        StartCoroutine(ShowGameOverRoutine());
    }
    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(gameOverDelay);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOver();
        }
    }

    private IEnumerator ResetHitTriggerRoutine()
    {
        yield return new WaitForEndOfFrame();
        if (anim != null)
        {
            anim.ResetTrigger("hit");
        }
    }
}