using UnityEngine;
using System.Collections.Generic;

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

    [Header("Transform Status")]
    public bool IsTransforming => isTransforming;

    [Header("Dialogue Control")]
    public bool canChase = false;

    [Header("Phase 1 Collision Damage")]
    public float damageCooldown = 1f; 
    private float lastDamageTime = 0f;

    [Header("Phase 2 Settings")]
    public float transformedSpeed = 4f; 
    public int healthThresholdToTransform = 2;

    [Header("Dialogo Post-Morte")]
    [Tooltip("Tempo di attesa (secondi) per far finire l'animazione di morte prima del monologo di Jack")]
    public float postDeathDelay = 3f;
    [Tooltip("Frasi di Jack dopo aver abbattuto il Boss")]
    public List<DialogueLine> postBossDialogue = new List<DialogueLine>();

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
        canChase = false;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyHealth = GetComponent<Enemy_Health>();
        enemyCombat = GetComponent<Enemy_Combat>();

        if (enemyHealth != null)
        {
            previousHealth = enemyHealth.currentHealth;
            enemyHealth.OnDeath += HandleBossDeath;
        }

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsTransforming", false);
        anim.SetBool("IsDemonIdle", false);
        anim.SetBool("IsDemonChasing", false);
    }

    private void HandleBossDeath()
    {
        NPCTriggerDialogue.IsBossDefeated = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBossMusic(1.5f); 
            AudioManager.Instance.Invoke("PlayRegularBGM", 1.5f); 
        }

        if (DialogueManager.Instance != null && postBossDialogue.Count > 0)
        {
            DialogueManager.Instance.StartDialogueSequenceWithDelay(postBossDialogue, false, postDeathDelay);
        }
    }

    private void Update()
    {
        if (!canChase)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            if (enemyState != EnemyState.Idle)
            {
                ChangeState(EnemyState.Idle);
            }
            return; 
        }

        if (!audioInitialized)
        {
            ChangeState(enemyState);
            audioInitialized = true;
        }

        CheckHealthAndTriggerHit();

        if (enemyState != EnemyState.Knockback && !isTransforming)
        {
            if (enemyState == EnemyState.Attacking)
            {
                rb.linearVelocity = Vector2.zero;
                return; 
            }

            CheckForPlayer();

            if (player != null)
            {
                if (!isFlamePhase)
                {
                    attackTimer -= Time.deltaTime; 

                    if (attackTimer <= 0f)
                    {
                        rb.linearVelocity = Vector2.zero; 

                        float directionToPlayer = player.position.x - transform.position.x;
                        Vector3 localScale = transform.localScale;
                        if (directionToPlayer > 0)
                        {
                            localScale.x = -Mathf.Abs(localScale.x);
                            facingDirection = 1;
                        }
                        else if (directionToPlayer < 0)
                        {
                            localScale.x = Mathf.Abs(localScale.x);
                            facingDirection = -1;
                        }
                        transform.localScale = localScale;

                        ChangeState(EnemyState.Attacking);
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
                        ChangeState(EnemyState.Chasing);
                    }
                    Chase();
                }
            }
        }
    }

    public void EnableBossChase()
    {
        canChase = true;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossMusic();
        }

        if (SongPopupUI.Instance != null)
        {
            SongPopupUI.Instance.ShowSongPopup();
        }

        if (BossHealthBarUI.Instance != null && enemyHealth != null)
        {
            BossHealthBarUI.Instance.InitializeBossBar(enemyHealth);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isFlamePhase && !isTransforming && enemyState != EnemyState.Knockback)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (Time.time >= lastDamageTime + damageCooldown)
                {
                    PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                    
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(1);
                        lastDamageTime = Time.time;
                    }
                }
            }
        }
    }

    private void CheckHealthAndTriggerHit()
    {
        if (enemyHealth == null) return;

        if (enemyHealth.currentHealth < previousHealth)
        {
            previousHealth = enemyHealth.currentHealth;
            
            if (anim != null)
            {
                if (isFlamePhase)
                {
                    anim.SetTrigger("hit");
                }
                else
                {
                    anim.SetTrigger("demonHit");
                }
            }
        }

        if (isFlamePhase && !isTransforming && enemyHealth.currentHealth <= healthThresholdToTransform)
        {
            StartTransformation();
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (isQuitting) return;

        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= HandleBossDeath;
        }
    }

    protected override void CheckForPlayer()
    {
        if (!canChase || isTransforming || enemyState == EnemyState.Attacking) return;

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
                if (audioSource != null && soundAttack1 != null) 
                    audioSource.PlayOneShot(soundAttack1, attackVolume);
                break;
            case 2:
                anim.SetTrigger("Attack2");
                if (audioSource != null && soundAttack2 != null) 
                    audioSource.PlayOneShot(soundAttack2, attackVolume);
                break;
            case 3:
                anim.SetTrigger("Attack3");
                if (audioSource != null && soundAttack3 != null) 
                    audioSource.PlayOneShot(soundAttack3, attackVolume);
                break;
        }
    }

    public void OnAttackFinished()
    {
        if (!isFlamePhase)
        {
            ChangeState(EnemyState.Chasing);
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

        if (isFlamePhase)
        {
            float directionToPlayer = player.position.x - transform.position.x;
            Vector3 localScale = transform.localScale;

            if (directionToPlayer > 0)
            {
                localScale.x = -Mathf.Abs(localScale.x);
                facingDirection = 1;
            }
            else if (directionToPlayer < 0)
            {
                localScale.x = Mathf.Abs(localScale.x);
                facingDirection = -1;
            }
            transform.localScale = localScale;

            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            base.Chase();
        }
    }

    public void FillBossSaveData(EnemySaveData data)
    {
        data.isFlamePhase = isFlamePhase;
        data.isTransforming = isTransforming;
        data.canChase = canChase;
        data.isBossActive = canChase && (GetComponent<Enemy_Health>() != null && GetComponent<Enemy_Health>().currentHealth > 0);
    }

    public void LoadBossData(EnemySaveData data)
    {
        isFlamePhase = data.isFlamePhase;
        data.isTransforming = isTransforming;
        canChase = data.canChase;

        if (data.isBossActive)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBossMusic();
            }
        }

        if (!isFlamePhase)
        {
            speed = transformedSpeed;
            anim.SetBool("IsFlameIdle", false);
            anim.SetBool("IsChasing", false);
            anim.SetBool("IsDemonIdle", true);
        }
        else if (isTransforming)
        {
            anim.SetBool("IsTransforming", true);
        }
        else
        {
            anim.SetBool("IsFlameIdle", true);
        }
    }
}