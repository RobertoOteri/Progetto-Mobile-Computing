using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public int facingDirection = 1;
    public Rigidbody2D rb;

    public Animator anim;

    private bool isKnockedBack;
    
    public Player_Combat player_Combat;

    // --- MEMORIA DIREZIONE ---
    private float lastVertical = 0f;
    private float lastHorizontal = 1f; // Di default guarda a destra


    private void Update()
    {
        // Aggiorniamo la memoria ogni volta che premi un tasto direzionale
        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");

        if (rawH != 0 || rawV != 0)
        {
            lastHorizontal = rawH;
            lastVertical = rawV;
        }

        if (Input.GetButtonDown("Slash"))
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");

            // Passiamo gli input attuali + la memoria (lastV e lastH)
            if (player_Combat != null)
            {
                player_Combat.Attack(v, h, lastVertical, lastHorizontal);
            }
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack == false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Giriamo lo sprite se necessario
            if ((horizontal > 0 && transform.localScale.x > 0) ||
                (horizontal < 0 && transform.localScale.x < 0))
            {
                Flip();
            }

            // AGGIORNA L'ANIMATOR SOLO SE NON STAI ATTACCANDO
            if (anim != null && !anim.GetBool("isAttacking"))
            {
                if (horizontal != 0)
                {
                    anim.SetFloat("horizontal", horizontal);
                    anim.SetFloat("vertical", 0);
                }
                else
                {
                    anim.SetFloat("horizontal", horizontal);
                    anim.SetFloat("vertical", vertical);
                }
            }

            rb.linearVelocity = new Vector2(horizontal, vertical) * speed; 
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockBackCounter(stunTime));
    }

    IEnumerator KnockBackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}