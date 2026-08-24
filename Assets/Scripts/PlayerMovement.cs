using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public int facingDirection = 1;
    public Rigidbody2D rb;

    public Animator anim;

    [Header("Mobile Controls")]
    [Tooltip("Trascina qui il prefab del Fixed Joystick presente nel Canvas")]
    public Joystick movementJoystick;

    [Header("Durata Hit / Visibilità Arma")]
    public float hitAnimationDuration = 0.45f;

    private bool isKnockedBack = false;
    
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
        if (isKnockedBack) return;
        if (player_Combat != null && player_Combat.IsThrowingBomb) return;

        // Legge l'input da Joystick o Tastiera
        float rawH = GetHorizontalInput();
        float rawV = GetVerticalInput();

        if (rawH != 0 || rawV != 0)
        {
            lastHorizontal = rawH;
            lastVertical = rawV;
        }

        // Tasto attacco / bomba da tastiera (per i test da PC)
        if (Input.GetButtonDown("Slash") || (Input.GetKeyDown(KeyCode.Q) && player_Combat != null && player_Combat.hasBomb))
        {
            TriggerAttack();
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopWalkSound();
            }
            return;
        }

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

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopWalkSound();
            }

            if (anim != null)
            {
                anim.SetFloat("horizontal", 0f);
                anim.SetFloat("vertical", 0f);

                Vector2 aim = isShootingRifle ? player_Rifle.AimDirection : player_Gun.AimDirection;

                string idleUp = isShootingGun ? "Gun_Idle_up" : "Rifle-Idle-Down";
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
        float horizontal = GetHorizontalInput();
        float vertical = GetVerticalInput();

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

        // === GESTIONE SUONO PASSI ===
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

    // --- FUNZIONI INPUT IBRIDE (Tastiera + Touch) ---
    private float GetHorizontalInput()
    {
        if (movementJoystick != null && Mathf.Abs(movementJoystick.Horizontal) > 0.1f)
        {
            return movementJoystick.Horizontal;
        }
        return Input.GetAxis("Horizontal");
    }

    private float GetVerticalInput()
    {
        if (movementJoystick != null && Mathf.Abs(movementJoystick.Vertical) > 0.1f)
        {
            return movementJoystick.Vertical;
        }
        return Input.GetAxis("Vertical");
    }

    // Funzione pubblica per il pulsante touch d'attacco a schermo
    public void TriggerAttack()
    {
        Debug.Log("--- PULSANTE ATTACCO PREMUTO CON SUCCESSO! ---");

        if (player_Combat != null)
        {
            float v = GetVerticalInput();
            float h = GetHorizontalInput();
            player_Combat.Attack(v, h, lastVertical, lastHorizontal);
        }
        else
        {
            Debug.LogError("Player_Combat è NULL su PlayerMovement!");
        }
        
        if (player_Combat != null)
        {
            float v = GetVerticalInput();
            float h = GetHorizontalInput();
            player_Combat.Attack(v, h, lastVertical, lastHorizontal);
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

    // === GESTIONE KNOCKBACK & HIT ANIMATION ===
    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;

        if (player_Combat != null)
        {
            player_Combat.HideWeapons();
        }
        
        Vector2 direction = Vector2.zero;
        if (enemy != null)
        {
            direction = (transform.position - enemy.position).normalized;
        }
        else
        {
            direction = new Vector2(-lastHorizontal, -lastVertical).normalized;
        }

        rb.linearVelocity = direction * force;

        if (anim != null)
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 0f);

            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                if (direction.y > 0)
                {
                    anim.Play("Hit_A_down", 0, 0f);
                }
                else
                {
                    anim.Play("Hit_A_up", 0, 0f);
                }
            }
            else
            {
                anim.Play("Hit_A_side", 0, 0f);

                if ((direction.x < 0 && transform.localScale.x < 0) || 
                    (direction.x > 0 && transform.localScale.x > 0))
                {
                    Flip();
                }
            }
        }

        StopCoroutine(nameof(KnockBackCounter));
        StartCoroutine(KnockBackCounter(stunTime));
    }

    IEnumerator KnockBackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;

        float remainingTime = hitAnimationDuration - stunTime;
        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        isKnockedBack = false;

        if (player_Combat != null)
        {
            player_Combat.RestoreWeapons();
        }
    }
}