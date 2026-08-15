using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public int facingDirection = 1;
    public Rigidbody2D rb;

    public Animator anim;

    private bool isKnockedBack;
    
    public Player_Combat player_Combat;
    public Player_Rifle player_Rifle;
    public Player_Gun player_Gun;

    // --- MEMORIA DIREZIONE ---
    private float lastVertical = 0f;
    private float lastHorizontal = 1f;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (player_Combat == null) player_Combat = GetComponent<Player_Combat>();
        if (player_Rifle == null) player_Rifle = GetComponent<Player_Rifle>();
        if (player_Gun == null) player_Gun = GetComponent<Player_Gun>();
    }

    private void Update()
    {
        if (player_Combat != null && player_Combat.IsThrowingBomb) return;

        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");

        if (rawH != 0 || rawV != 0)
        {
            lastHorizontal = rawH;
            lastVertical = rawV;
        }

        if (Input.GetButtonDown("Slash") || (Input.GetKeyDown(KeyCode.Q) && player_Combat != null && player_Combat.hasBomb))
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
            // === BLOCCO LANCIO BOMBA ===
            if (player_Combat != null && player_Combat.IsThrowingBomb)
            {
                rb.linearVelocity = Vector2.zero;
                if (anim != null)
                {
                    anim.SetFloat("horizontal", 0f);
                    anim.SetFloat("vertical", 0f);
                }
                return;
            }

            // === BLOCCO SPARO (Fucile o Pistola) ===
            bool isShootingRifle = player_Rifle != null && player_Rifle.IsShooting;
            bool isShootingGun = player_Gun != null && player_Gun.IsShooting;

            if (isShootingRifle || isShootingGun)
            {
                rb.linearVelocity = Vector2.zero;

                // Se sta sparando fermiamo l'effetto audio della camminata
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopWalkSound();
                }

                if (anim != null)
                {
                    anim.SetFloat("horizontal", 0f);
                    anim.SetFloat("vertical", 0f);

                    Vector2 aim = isShootingRifle ? player_Rifle.AimDirection : player_Gun.AimDirection;

                    string idleUp = isShootingGun ? "Gun_Idle_up" : "Rifle-Idle-Up";
                    string idleDown = isShootingGun ? "Gun_Idle_down" : "Rifle-Idle-Down";
                    string idleSide = isShootingGun ? "Gun_Idle_side" : "Rifle-Idle-Side";

                    if (aim.y > 0)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleUp))
                            anim.Play(idleUp);
                    }
                    else if (aim.y < 0)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleDown))
                            anim.Play(idleDown);
                    }
                    else if (aim.x != 0)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleSide))
                            anim.Play(idleSide);

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
                    anim.SetFloat("vertical", 0f);
                }
                else
                {
                    anim.SetFloat("horizontal", horizontal);
                    anim.SetFloat("vertical", vertical);
                }
            }

            rb.linearVelocity = new Vector2(horizontal, vertical) * speed;

            // === LOGICA RIPRODUZIONE / INTERRUZIONE SUONO PASSI ===
            bool isMoving = (horizontal != 0 || vertical != 0);

            if (isMoving)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StartWalkSound();
                }
            }
            else
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopWalkSound();
                }
            }
        }
        else
        {
            // Se subisce knockback e non può muoversi, fermiamo il suono della camminata
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopWalkSound();
            }
        }
    }

    public void StopMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 0f);
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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