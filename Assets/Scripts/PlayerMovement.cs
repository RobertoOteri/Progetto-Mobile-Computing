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
    public Player_Rifle player_Rifle;

    // --- MEMORIA DIREZIONE ---
    private float lastVertical = 0f;
    private float lastHorizontal = 1f;


    private void Start()
    {
        if (player_Combat == null) player_Combat = GetComponent<Player_Combat>();
        if (player_Rifle == null) player_Rifle = GetComponent<Player_Rifle>();
    }

    private void Update()
    {
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
            // === GESTIONE SPARO: FORZA IDLE SENZA ATTIVARE LA CAMMINATA ===
            if (player_Rifle != null && player_Rifle.IsShooting)
            {
                rb.linearVelocity = Vector2.zero; // Ferma il movimento fisico

                if (anim != null)
                {
                    // Azzera i float per evitare che scattino le transizioni di camminata
                    anim.SetFloat("horizontal", 0);
                    anim.SetFloat("vertical", 0);

                    Vector2 aim = player_Rifle.AimDirection;

                    // Forza la riproduzione dello stato Idle corretto senza passare dalle frecce dell'Animator
                    if (aim.y > 0)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Rifle-Idle-Up"))
                            anim.Play("Rifle-Idle-Up");
                    }
                    else if (aim.y < 0)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Rifle-Idle-Down"))
                            anim.Play("Rifle-Idle-Down");
                    }
                    else if (aim.x != 0)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Rifle-Idle-Side"))
                            anim.Play("Rifle-Idle-Side");

                        // Giriamo lo sprite se necessario usando la tua logica di Flip originale
                        if ((aim.x > 0 && transform.localScale.x > 0) || (aim.x < 0 && transform.localScale.x < 0))
                        {
                            Flip();
                        }
                    }
                }
                return;
            }

            // === NORMALE MOVIMENTO ===
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if ((horizontal > 0 && transform.localScale.x > 0) ||
                (horizontal < 0 && transform.localScale.x < 0))
            {
                Flip();
            }

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