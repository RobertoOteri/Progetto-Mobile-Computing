using UnityEngine;

public enum BossState
{
    Spawn,
    Idle,
    Walking,
    Hangry,
    Charging,
    Attacking,
    Death,
    Knockback
}

public class Boss_Controller : MonoBehaviour
{
    [Header("Parametri Movimento e Rilevamento")]
    public float speed = 2f;
    public float playerDetectRange = 6f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    public LayerMask playerLayer;
    public Transform detectionPoint;

    [Header("Parametri Rincorsa (Charging)")]
    public float chargeSpeed = 6f;
    public float chargeDuration = 0.8f;
    private float chargeTimer;

    [Header("Parametri Combattimento")]
    public int damage = 2;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public float knockbackForce = 5f;
    public float stunTime = 0.2f;

    [Header("--- Audio Attacco Boss ---")]
    [SerializeField] private AudioClip attackSFX; 
    [Range(0f, 1f)]
    [SerializeField] private float attackVolume = 0.4f;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private BossState bossState;
    private float attackCooldownTimer;
    private bool isDead = false;
    private bool hasTriggeredHangry = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(BossState.Spawn);
    }

    void Update()
    {
        if (isDead) return;

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        switch (bossState)
        {
            case BossState.Spawn:
            case BossState.Attacking:
            case BossState.Hangry:
                break;

            case BossState.Idle:
            case BossState.Walking:
                CheckForPlayer();
                break;

            case BossState.Charging:
                if (player != null)
                {
                    Vector2 chargeDir = (player.position - transform.position).normalized;
                    rb.linearVelocity = chargeDir * chargeSpeed;
                }

                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0)
                {
                    ChangeState(BossState.Attacking);
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (bossState == BossState.Idle || bossState == BossState.Hangry || bossState == BossState.Attacking)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * 15f);
        }
    }

    void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            // Forza sempre lo stato di camminata verso il player
            if (bossState != BossState.Walking)
            {
                ChangeState(BossState.Walking);
            }
            
            MoveTowardsPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (bossState != BossState.Idle)
            {
                ChangeState(BossState.Idle);
            }
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (anim != null)
        {
            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);
            
            // DEBUG: Stampa in console i valori reali passati all'animator
            Debug.Log($"Direzione Calcolata -> MoveX: {direction.x}, MoveY: {direction.y}");
        }
    }

    public void ChangeState(BossState newState)
    {
        if (isDead) return;

        ResetAnimatorParameters();
        bossState = newState;

        switch (bossState)
        {
            case BossState.Spawn:
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("Spawn");
                break;
            case BossState.Idle:
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("IsIdle", true);
                break;
            case BossState.Walking:
                anim.SetBool("IsWalking", true);
                break;
            case BossState.Hangry:
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("Hangry");
                break;
            case BossState.Charging:
                chargeTimer = chargeDuration;
                anim.SetBool("IsWalking", true);
                break;
            case BossState.Attacking:
                rb.linearVelocity = Vector2.zero;
                if (player != null && player.position.y > transform.position.y)
                    anim.SetTrigger("AttackUp");
                else
                    anim.SetTrigger("AttackDown");
                break;
            case BossState.Death:
                isDead = true;
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("Death");
                break;
        }
    }

    void ResetAnimatorParameters()
    {
        if (anim == null) return;
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsWalking", false);
    }

    public void Attack()
    {
        if (attackPoint == null) return;

        if (attackSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(attackSFX, attackVolume);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0)
        {
            PlayerHealth playerHealth = hits[0].GetComponent<PlayerHealth>();

            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.ChangeHealth(-damage);

                PlayerMovement playerMovement = hits[0].GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.Knockback(transform, knockbackForce, stunTime);
                }
            }
        }
    }

    public void OnSpawnFinished() => ChangeState(BossState.Idle);
    public void OnHangryFinished() => ChangeState(BossState.Charging);
    public void OnAttackFinished() => ChangeState(BossState.Idle);
    public void Die() => ChangeState(BossState.Death);

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, attackRange);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }
    }
}