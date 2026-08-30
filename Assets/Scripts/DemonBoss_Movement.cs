using UnityEngine;

public class DemonBoss_Movement : Enemy_Movement
{
    private bool isFlamePhase = true;
    private bool isTransforming = false;
    private SpriteRenderer spriteRenderer;
    private Enemy_Health enemyHealth;
    private int previousHealth;

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

        if (enemyHealth != null)
        {
            previousHealth = enemyHealth.currentHealth;
        }

        anim.SetBool("IsChasing", false);
        anim.SetBool("IsTransforming", false);
        anim.SetBool("IsDemonIdle", false);
        anim.SetBool("IsDemonChasing", false);
    }

    private void Update()
    {
        // Controlla autonomamente se la vita è diminuita
        CheckHealthAndTriggerHit();

        if (enemyState != EnemyState.Knockback && !isTransforming)
        {
            CheckForPlayer();

            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
        }
    }

    private void CheckHealthAndTriggerHit()
    {
        if (enemyHealth == null) return;

        // Se la salute attuale è inferiore alla precedente, il boss è stato colpito
        if (enemyHealth.currentHealth < previousHealth)
        {
            previousHealth = enemyHealth.currentHealth;
            
            // Fa partire l'animazione hurt1
            if (anim != null)
            {
                anim.SetTrigger("hit");
            }

            // Controlla se deve trasformarsi
            if (isFlamePhase && !isTransforming && enemyHealth.currentHealth <= healthThresholdToTransform)
            {
                StartTransformation();
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

            if (enemyState != EnemyState.Chasing)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    private void StartTransformation()
    {
        isTransforming = true;
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsFlameIdle", false);
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsTransforming", true);
    }

    public void OnTransformationFinished()
    {
        isTransforming = false;
        isFlamePhase = false;
        speed = transformedSpeed; 

        anim.SetBool("IsTransforming", false);
        anim.SetBool("IsDemonIdle", true);
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
            if (enemyState == EnemyState.Idle) anim.SetBool("IsDemonIdle", true);
            else anim.SetBool("IsDemonIdle", false);

            if (enemyState == EnemyState.Chasing) anim.SetBool("IsDemonChasing", true);
            else anim.SetBool("IsDemonChasing", false);
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