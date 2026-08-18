using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 2;
    public float attackCooldown = 2;
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer;
    protected Rigidbody2D rb; 
    protected Transform player;
    protected Animator anim;

    protected EnemyState enemyState;
    protected int facingDirection = 1;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    protected virtual void Chase()
    {
        if (player == null) return;

        if ((player.position.x > transform.position.x && facingDirection == -1) || 
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    protected void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    protected virtual void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            PlayerHealth playerHealth = hits[0].GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.IsDead)
            {
                player = null;
                rb.linearVelocity = Vector2.zero;
                
                if (enemyState != EnemyState.Idle)
                {
                    ChangeState(EnemyState.Idle);
                }
                return;
            }

            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange && enemyState != EnemyState.Attacking)
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

    public virtual void ChangeState(EnemyState newState)
    {
        if (anim == null) return;

        // Disattiva i bool dello stato precedente
        if (enemyState == EnemyState.Idle) anim.SetBool("IsIdle", false);
        else if (enemyState == EnemyState.Chasing) anim.SetBool("IsChasing", false);
        else if (enemyState == EnemyState.Attacking) anim.SetBool("IsAttacking", false);

        enemyState = newState;

        // Attiva il bool del nuovo stato (se Knockback, restano tutti false finché dura l'animazione)
        if (enemyState == EnemyState.Idle) anim.SetBool("IsIdle", true);
        else if (enemyState == EnemyState.Chasing) anim.SetBool("IsChasing", true);
        else if (enemyState == EnemyState.Attacking) anim.SetBool("IsAttacking", true);
    }

    public void OnAttackFinished()
    {
        ChangeState(EnemyState.Idle);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}