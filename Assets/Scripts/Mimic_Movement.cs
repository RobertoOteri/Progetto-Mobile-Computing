using UnityEngine;

public class Mimic_Movement : Enemy_Movement
{
    private bool isSleeping = true;
    private bool isTransforming = false;

    [Header("Audio Mimic")]
    public AudioSource audioSource;
    public AudioClip openSound;
    [Range(0f, 1f)] public float openSoundVolume = 1f;

    protected override void Start()
    {
        base.Start();

        isSleeping = true;
        isTransforming = false;

        // Se non viene assegnato manuale, prova a prenderlo dallo stesso GameObject
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        anim.SetBool("IsSleeping", true);
        anim.SetBool("IsOpening", false);
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);
    }

    protected override void CheckForPlayer()
    {
        if (enemyState == EnemyState.Knockback) return;

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

    protected override void Chase()
    {
        if (isSleeping || isTransforming)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        base.Chase();
    }

    private void StartTransformation()
    {
        isSleeping = false;
        isTransforming = true; 

        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsSleeping", false);
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);
        
        anim.SetBool("IsOpening", true);

        PlayOpenSound();
    }

    public void PlayOpenSound()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, openSoundVolume);
        }
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
    }
}