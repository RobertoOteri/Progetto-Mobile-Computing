using UnityEngine;

public class DemonBoss_Movement : Enemy_Movement
{
    private bool isFlamePhase = true;
    private bool isTransforming = false;
    private bool audioInitialized = false;
    private SpriteRenderer spriteRenderer;
    private Enemy_Health enemyHealth;
    private Enemy_Combat enemyCombat;
    private int previousHealth;
    private float attackTimer = 0f;
    private bool isQuitting = false;

    [Header("Phase 2 Settings")]
    public float transformedSpeed = 4f; 
    public int healthThresholdToTransform = 2;

    [Header("Audio Settings - Attacks")]
    public AudioSource audioSource;
    public AudioClip soundAttack1;
    public AudioClip soundAttack2;
    public AudioClip soundAttack3;
    [Range(0f, 1f)] public float attackVolume = 0.2f;

    [Header("Audio Settings - Loops & States")]
    public AudioSource loopAudioSource; 
    public AudioClip soundTransform;
    [Range(0f, 1f)] public float transformVolume = 0.5f;
    
    public AudioClip soundPhase1Idle;
    public AudioClip soundPhase1Walk;
    public AudioClip soundPhase2Walk;
    [Range(0f, 1f)] public float movementAudioVolume = 0.3f;

    protected override void Start()
    {
        base.Start();
        isFlamePhase = true;
        isTransforming = false;
        audioInitialized = false;
        isQuitting = false;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyHealth = GetComponent<Enemy_Health>();
        enemyCombat = GetComponent<Enemy_Combat>();

        if (enemyHealth != null)
        {
            previousHealth = enemyHealth.currentHealth;
            Debug.Log($"[DEBUG] Start: Health iniziale = {previousHealth}");
        }

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsTransforming", false);
        anim.SetBool("IsDemonIdle", false);
        anim.SetBool("IsDemonChasing", false);
    }

    private void Update()
    {
        if (!audioInitialized)
        {
            ChangeState(enemyState);
            audioInitialized = true;
        }

        CheckHealthAndTriggerHit();

        if (enemyState != EnemyState.Knockback && !isTransforming)
        {
            CheckForPlayer();

            if (player != null)
            {
                float currentAttackRange = (enemyCombat != null) ? enemyCombat.weaponRange : 1.5f;
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (!isFlamePhase)
                {
                    attackTimer -= Time.deltaTime; 

                    if (attackTimer <= 0f)
                    {
                        rb.linearVelocity = Vector2.zero; 

                        if ((player.position.x > transform.position.x && facingDirection == -1) || 
                            (player.position.x < transform.position.x && facingDirection == 1))
                        {
                            Flip();
                        }

                        ChooseRandomAttack();
                        attackTimer = 3f; 
                    }
                    else
                    {
                        if (enemyState != EnemyState.Chasing)
                        {
                            ChangeState(EnemyState.Chasing);
                        }
                        Chase();
                    }
                }
                else
                {
                    if (enemyState == EnemyState.Chasing)
                    {
                        Chase();
                    }
                    else
                    {
                        rb.linearVelocity = Vector2.zero;
                    }
                }
            }
        }
    }

    private void CheckHealthAndTriggerHit()
    {
        if (enemyHealth == null) return;

        if (enemyHealth.currentHealth != previousHealth)
        {
            Debug.Log($"[DEBUG] Vita cambiata! Vecchia: {previousHealth}, Nuova: {enemyHealth.currentHealth}");
        }

        if (enemyHealth.currentHealth < previousHealth)
        {
            previousHealth = enemyHealth.currentHealth;
            
            if (anim != null)
            {
                if (isFlamePhase)
                {
                    Debug.Log("[DEBUG] Trigger animazione 'hit' (Fase 1)");
                    anim.SetTrigger("hit");
                }
                else
                {
                    Debug.Log("[DEBUG] Trigger animazione 'demonHit' (Fase 2)");
                    anim.SetTrigger("demonHit");
                }
            }
        }

        // Se la vita arriva a 0 o meno, consideriamo il boss sconfitto
        if (enemyHealth.currentHealth <= 0)
        {
            if (BossMusicManager.Instance != null)
            {
                BossMusicManager.Instance.StopBossMusicWithFade();
            }
        }

        if (isFlamePhase && !isTransforming && enemyHealth.currentHealth <= healthThresholdToTransform)
        {
            Debug.Log("[DEBUG] Condizione di trasformazione raggiunta! Avvio StartTransformation()");
            StartTransformation();
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        // Se l'applicazione si chiude non facciamo partire il fade della musica
        if (isQuitting) return;

        // Se il boss viene distrutto e la sua vita è a 0 (o il componente salute non esiste più perché distrutto), spegne la musica
        if (enemyHealth == null || enemyHealth.currentHealth <= 0)
        {
            if (BossMusicManager.Instance != null)
            {
                BossMusicManager.Instance.StopBossMusicWithFade();
            }
        }
    }

    protected override void CheckForPlayer()
    {
        if (isTransforming) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            PlayerHealth playerHealth = hits[0].GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.IsDead)
            {
                player = null;
                rb.linearVelocity = Vector2.zero;
                if (enemyState != EnemyState.Idle) ChangeState(EnemyState.Idle);
                return;
            }

            player = hits[0].transform;

            if ((player.position.x > transform.position.x && facingDirection == -1) || 
                (player.position.x < transform.position.x && facingDirection == 1))
            {
                Flip();
            }

            if (isFlamePhase)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                
                if (distanceToPlayer <= attackRange)
                {
                    rb.linearVelocity = Vector2.zero;
                    if (enemyState != EnemyState.Attacking)
                    {
                        ChangeState(EnemyState.Attacking);
                    }
                }
                else
                {
                    if (enemyState != EnemyState.Chasing)
                    {
                        ChangeState(EnemyState.Chasing);
                    }
                }
            }
        }
        else
        {
            player = null;
            rb.linearVelocity = Vector2.zero;
            if (enemyState != EnemyState.Idle)
            {
                ChangeState(EnemyState.Idle);
            }
        }
    }

    private void StartTransformation()
    {
        isTransforming = true;
        rb.linearVelocity = Vector2.zero;

        if (loopAudioSource != null) loopAudioSource.Stop();

        if (audioSource != null && soundTransform != null)
        {
            audioSource.PlayOneShot(soundTransform, transformVolume);
        }

        anim.SetBool("IsFlameIdle", false);
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsTransforming", true);
        Debug.Log("[DEBUG] Animazione di trasformazione avviata (IsTransforming = true)");
    }

    public void OnTransformationFinished()
    {
        isTransforming = false;
        isFlamePhase = false;
        speed = transformedSpeed; 
        attackTimer = 3f; 

        anim.SetBool("IsTransforming", false);
        
        enemyState = EnemyState.Idle;
        anim.SetBool("IsDemonIdle", true);
        anim.SetBool("IsDemonChasing", false);
        
        Debug.Log("[DEBUG] Trasformazione completata! Passato a Fase 2 (Demon). IsDemonIdle = true");
    }

    public override void ChangeState(EnemyState newState)
    {
        if (anim == null) return;
        if (isTransforming) return; 

        enemyState = newState;

        HandleStateAudio();

        if (isFlamePhase)
        {
            if (enemyState == EnemyState.Idle) anim.SetBool("IsFlameIdle", true);
            else anim.SetBool("IsFlameIdle", false);

            if (enemyState == EnemyState.Chasing) anim.SetBool("IsChasing", true);
            else anim.SetBool("IsChasing", false);
        }
        else
        {
            anim.SetBool("IsFlameIdle", false);
            anim.SetBool("IsChasing", false);

            if (enemyState == EnemyState.Idle) 
            {
                anim.SetBool("IsDemonIdle", true);
                anim.SetBool("IsDemonChasing", false);
            }
            else 
            {
                anim.SetBool("IsDemonIdle", false);
            }

            if (enemyState == EnemyState.Chasing) 
            {
                anim.SetBool("IsDemonChasing", true);
                anim.SetBool("IsDemonIdle", false);
            }
            else 
            {
                anim.SetBool("IsDemonChasing", false);
            }
        }
    }

    private void HandleStateAudio()
    {
        if (loopAudioSource == null)
        {
            loopAudioSource = GetComponent<AudioSource>();
            if (loopAudioSource == null)
            {
                loopAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        AudioClip targetClip = null;

        if (isFlamePhase)
        {
            if (enemyState == EnemyState.Idle) targetClip = soundPhase1Idle;
            else if (enemyState == EnemyState.Chasing) targetClip = soundPhase1Walk;
        }
        else
        {
            if (enemyState == EnemyState.Chasing) targetClip = soundPhase2Walk;
        }

        if (targetClip != null)
        {
            if (loopAudioSource.clip != targetClip || !loopAudioSource.isPlaying)
            {
                loopAudioSource.clip = targetClip;
                loopAudioSource.loop = true;
                loopAudioSource.volume = movementAudioVolume;
                loopAudioSource.Play();
                Debug.Log($"[DEBUG AUDIO] Avviato loop per: {targetClip.name}");
            }
        }
        else
        {
            if (loopAudioSource.isPlaying)
            {
                loopAudioSource.Stop();
                loopAudioSource.clip = null;
            }
        }
    }

    public void ChooseRandomAttack()
    {
        if (isTransforming || player == null) return;

        int randomAttack = Random.Range(1, 4); 

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        switch (randomAttack)
        {
            case 1:
                anim.SetTrigger("Attack1");
                Debug.Log("[DEBUG] Attacco scelto: Fire Breath");
                if (audioSource != null && soundAttack1 != null) 
                    audioSource.PlayOneShot(soundAttack1, attackVolume);
                break;
            case 2:
                anim.SetTrigger("Attack2");
                Debug.Log("[DEBUG] Attacco scelto: Cleave");
                if (audioSource != null && soundAttack2 != null) 
                    audioSource.PlayOneShot(soundAttack2, attackVolume);
                break;
            case 3:
                anim.SetTrigger("Attack3");
                Debug.Log("[DEBUG] Attacco scelto: Smash");
                if (audioSource != null && soundAttack3 != null) 
                    audioSource.PlayOneShot(soundAttack3, attackVolume);
                break;
        }
    }

    public void TriggerCombatAttack()
    {
        if (enemyCombat != null)
        {
            enemyCombat.Attack();
        }
    }

    protected override void Flip()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale; 
    }

    protected override void Chase()
    {
        if (player == null) return;

        if ((player.position.x > transform.position.x && facingDirection == 1) || 
            (player.position.x < transform.position.x && facingDirection == -1))
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
}