using UnityEngine;

public class DemonBoss_Movement : Enemy_Movement
{
    private bool isFlamePhase = true;
    private bool isTransforming = false;
    private SpriteRenderer spriteRenderer;
    private Enemy_Health enemyHealth;
    private Enemy_Combat enemyCombat;
    private int previousHealth;
    private float attackTimer = 0f;

    [Header("Phase 2 Settings")]
    public float transformedSpeed = 4f; 
    public int healthThresholdToTransform = 2;

    protected override void Start()
    {
        base.Start();
        isFlamePhase = true;
        isTransforming = false;

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
                    // --- FASE 2: Logica del Demone ---
                    attackTimer -= Time.deltaTime; // Scala continuamente il timer indipendentemente dallo stato

                    // Può attaccare solo se il timer è sceso a zero o meno
                    if (attackTimer <= 0f)
                    {
                        rb.linearVelocity = Vector2.zero; // Ferma subito il movimento

                        // --- CORRETTO: Gira verso il player PRIMA di scegliere l'attacco ---
                        if ((player.position.x > transform.position.x && facingDirection == -1) || 
                            (player.position.x < transform.position.x && facingDirection == 1))
                        {
                            Flip();
                        }

                        ChooseRandomAttack();
                        attackTimer = 3f; // Pausa di 3 secondi esatti prima del prossimo attacco
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
                    // --- FASE 1: Logica standard ereditata ---
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

        if (isFlamePhase && !isTransforming && enemyHealth.currentHealth <= healthThresholdToTransform)
        {
            Debug.Log("[DEBUG] Condizione di trasformazione raggiunta! Avvio StartTransformation()");
            StartTransformation();
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

            // Gira sempre il boss verso il player se si trova sul lato opposto
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
        attackTimer = 3f; // Imposta il cooldown iniziale dopo la trasformazione

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

    public void ChooseRandomAttack()
    {
        if (isTransforming || player == null) return;

        int randomAttack = Random.Range(1, 4); 

        switch (randomAttack)
        {
            case 1:
                anim.SetTrigger("Attack1");
                Debug.Log("[DEBUG] Attacco scelto: Fire Breath");
                break;
            case 2:
                anim.SetTrigger("Attack2");
                Debug.Log("[DEBUG] Attacco scelto: Cleave");
                break;
            case 3:
                anim.SetTrigger("Attack3");
                Debug.Log("[DEBUG] Attacco scelto: Smash");
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
        transform.localScale = scale; // Corretto da transform.Scale a transform.localScale

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