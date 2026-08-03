using UnityEngine;

public class Mimic_Movement : Enemy_Movement
{
    private bool isSleeping = true;
    private bool isTransforming = false;

    protected override void Start()
    {
        base.Start();

        isSleeping = true;
        isTransforming = false;

        anim.SetBool("IsSleeping", true);
        anim.SetBool("IsOpening", false);
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);
    }

    protected override void CheckForPlayer()
    {
        if (enemyState == EnemyState.Knockback)
        {
            return;
        }
        
        if (isTransforming)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isSleeping)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

            if (hits.Length > 0)
            {
                player = hits[0].transform;
                
                if ((player.position.x > transform.position.x && facingDirection == -1) || 
                    (player.position.x < transform.position.x && facingDirection == 1))
                {
                    Flip();
                }

                StartTransformation();
            }
            return;
        }

        base.CheckForPlayer();
    }

    private void StartTransformation()
    {
        isSleeping = false;
        isTransforming = true; 

        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsSleeping", false);
        anim.SetBool("IsOpening", true);
    }

    public override void ChangeState(EnemyState newState)
    {
        base.ChangeState(newState);

        if (newState == EnemyState.Knockback)
        {
            if (isSleeping || isTransforming)
            {
                isSleeping = false;
                isTransforming = false;
                anim.SetBool("IsSleeping", false);
                anim.SetBool("IsOpening", false);
            }
        }
    }

    public void OnOpeningFinished()
    {
        isTransforming = false; 
        anim.SetBool("IsOpening", false);

        ChangeState(EnemyState.Idle);
    }
}