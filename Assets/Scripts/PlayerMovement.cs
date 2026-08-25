using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public int facingDirection = 1;
    public Rigidbody2D rb;

    public Animator anim;

    [Header("Mobile Controls")]
    public Joystick movementJoystick;

    [Header("Durata Hit / Visibilità Arma")]
    public float hitAnimationDuration = 0.45f;

    private bool isKnockedBack = false;
    
    public Player_Combat player_Combat;
    public Player_Rifle player_Rifle;
    public Player_Gun player_Gun;

    // --- MEMORIA DIREZIONE ---
    [HideInInspector] public float lastVertical = 0f;
    [HideInInspector] public float lastHorizontal = 1f;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (player_Combat == null) player_Combat = GetComponent<Player_Combat>();
        if (player_Rifle == null) player_Rifle = GetComponent<Player_Rifle>();
        if (player_Gun == null) player_Gun = GetComponent<Player_Gun>();
        if (movementJoystick == null) movementJoystick = FindFirstObjectByType<Joystick>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (isKnockedBack) return;
        if (player_Combat != null && player_Combat.IsThrowingBomb) return;

        float rawH = GetHorizontalInput();
        float rawV = GetVerticalInput();

        if (rawH != 0 || rawV != 0)
        {
            lastHorizontal = rawH;
            lastVertical = rawV;
        }

        // Singolo colpo da tastiera
        if (Input.GetButtonDown("Slash") || (Input.GetKeyDown(KeyCode.Q) && player_Combat != null && player_Combat.hasBomb))
        {
            TriggerAttack();
        }

        // Raffica Fucile da tastiera tenendo premuto K o Slash
        if (Input.GetButton("Slash") && player_Combat != null && player_Combat.hasRifle)
        {
            TriggerAttack();
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StopWalkSound();
            return;
        }

        // === BLOCCO LANCIO BOMBA ===
        if (player_Combat != null && player_Combat.IsThrowingBomb)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // === BLOCCO ATTACCO CORPO A CORPO ===
        if (anim != null && anim.GetBool("isAttacking"))
        {
            rb.linearVelocity = Vector2.zero;
            if (AudioManager.Instance != null) AudioManager.Instance.StopWalkSound();
            return;
        }

        // === BLOCCO SPARO (Fucile o Pistola) ===
        bool isShootingRifle = player_Rifle != null && player_Rifle.IsShooting;
        bool isShootingGun = player_Gun != null && player_Gun.IsShooting;

        if (isShootingRifle || isShootingGun)
        {
            rb.linearVelocity = Vector2.zero;

            if (AudioManager.Instance != null) AudioManager.Instance.StopWalkSound();

            if (anim != null)
            {
                anim.SetFloat("horizontal", 0f);
                anim.SetFloat("vertical", 0f);

                Vector2 aim = isShootingRifle ? player_Rifle.AimDirection : player_Gun.AimDirection;
                string weaponPrefix = isShootingGun ? "Gun_" : "Rifle_";

                string idleUp = weaponPrefix + "Idle_up";
                string idleDown = weaponPrefix + "Idle_down";
                string idleSide = weaponPrefix + "Idle_side";

                if (aim.y > 0)
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleUp)) anim.Play(idleUp);
                }
                else if (aim.y < 0)
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleDown)) anim.Play(idleDown);
                }
                else if (aim.x != 0)
                {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleSide)) anim.Play(idleSide);

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
            UpdatePlayerAnimations(horizontal, vertical);
        }

        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;

        // === GESTIONE SUONO PASSI ===
        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);
        if (isMoving)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StartWalkSound();
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StopWalkSound();
        }
    }

    private void UpdatePlayerAnimations(float h, float v)
    {
        // Se stiamo attaccando con la spada o martello, non dobbiamo sovrascrivere l'animazione di colpo!
        if (anim != null && anim.GetBool("isAttacking")) return;

        float absH = Mathf.Abs(h);
        float absV = Mathf.Abs(v);

        // 1. Invia sempre i float discretizzati
        if (absH > 0.15f || absV > 0.15f)
        {
            if (absH >= absV)
            {
                anim.SetFloat("horizontal", h > 0 ? 1f : -1f);
                anim.SetFloat("vertical", 0f);
            }
            else
            {
                anim.SetFloat("horizontal", 0f);
                anim.SetFloat("vertical", v > 0 ? 1f : -1f);
            }
        }
        else
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 0f);
        }

        if (player_Combat == null) return;

        // 2. Determina se l'arma necessita del Play() diretto (armi da fuoco e bomba)
        // NOTA: Spada e Martello usano le loro transizioni di Slash nell'Animator quando si attacca
        string prefix = "";
        bool useDirectPlay = false;

        if (player_Combat.hasGun) { prefix = "Gun_"; useDirectPlay = true; }
        else if (player_Combat.hasRifle) { prefix = "Rifle_"; useDirectPlay = true; }
        else if (player_Combat.hasBomb) { prefix = "Bomb_"; useDirectPlay = true; }
        else if (player_Combat.hasSword) { prefix = "Sword_"; useDirectPlay = true; }
        else if (player_Combat.hasHammer) { prefix = "Hammer_"; useDirectPlay = true; }

        if (useDirectPlay)
        {
            if (absH > 0.15f || absV > 0.15f)
            {
                if (absH >= absV)
                {
                    string walkSide = prefix + "walk_side";
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(walkSide))
                        anim.Play(walkSide);
                }
                else
                {
                    if (v > 0)
                    {
                        string walkUp = prefix + "walk_up";
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(walkUp))
                            anim.Play(walkUp);
                    }
                    else
                    {
                        string walkDown = prefix + "walk_down";
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(walkDown))
                            anim.Play(walkDown);
                    }
                }
            }
            else
            {
                if (Mathf.Abs(lastHorizontal) >= Mathf.Abs(lastVertical))
                {
                    string idleSide = prefix + "Idle_side";
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleSide))
                        anim.Play(idleSide);
                }
                else
                {
                    if (lastVertical > 0)
                    {
                        string idleUp = prefix + "Idle_up";
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleUp))
                            anim.Play(idleUp);
                    }
                    else
                    {
                        string idleDown = prefix + "Idle_down";
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(idleDown))
                            anim.Play(idleDown);
                    }
                }
            }
        }
    }

    public void ForceIdleAndStop()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopWalkSound();
        }

        if (anim != null)
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 0f);

            string prefix = "";
            bool hasWeapon = false;

            if (player_Combat != null)
            {
                if (player_Combat.hasGun) { prefix = "Gun_"; hasWeapon = true; }
                else if (player_Combat.hasRifle) { prefix = "Rifle_"; hasWeapon = true; }
                else if (player_Combat.hasSword) { prefix = "Sword_"; hasWeapon = true; }
                else if (player_Combat.hasHammer) { prefix = "Hammer_"; hasWeapon = true; }
                else if (player_Combat.hasBomb) { prefix = "Bomb_"; hasWeapon = true; }
            }

            if (hasWeapon)
            {
                if (Mathf.Abs(lastHorizontal) >= Mathf.Abs(lastVertical))
                {
                    anim.Play(prefix + "Idle_side");
                }
                else
                {
                    if (lastVertical > 0)
                        anim.Play(prefix + "Idle_up");
                    else
                        anim.Play(prefix + "Idle_down");
                }
            }
        }
    }

    public float GetHorizontalInput()
    {
        if (movementJoystick != null && Mathf.Abs(movementJoystick.Horizontal) > 0.1f)
        {
            return movementJoystick.Horizontal;
        }
        return Input.GetAxis("Horizontal");
    }

    public float GetVerticalInput()
    {
        if (movementJoystick != null && Mathf.Abs(movementJoystick.Vertical) > 0.1f)
        {
            return movementJoystick.Vertical;
        }
        return Input.GetAxis("Vertical");
    }

    public void TriggerAttack()
    {
        float v = GetVerticalInput();
        float h = GetHorizontalInput();

        if (player_Combat != null)
        {
            player_Combat.ExecuteCurrentWeaponAction(v, h, lastVertical, lastHorizontal);
        }
    }

    public void StopMovement()
    {
        ForceIdleAndStop();
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        if (player_Combat != null) player_Combat.HideWeapons();
        
        Vector2 direction = Vector2.zero;
        if (enemy != null)
            direction = (transform.position - enemy.position).normalized;
        else
            direction = new Vector2(-lastHorizontal, -lastVertical).normalized;

        rb.linearVelocity = direction * force;

        if (anim != null)
        {
            anim.SetFloat("horizontal", 0f);
            anim.SetFloat("vertical", 0f);

            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                if (direction.y > 0) anim.Play("Hit_A_down", 0, 0f);
                else anim.Play("Hit_A_up", 0, 0f);
            }
            else
            {
                anim.Play("Hit_A_side", 0, 0f);
                if ((direction.x < 0 && transform.localScale.x < 0) || (direction.x > 0 && transform.localScale.x > 0))
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
        if (remainingTime > 0f) yield return new WaitForSeconds(remainingTime);

        isKnockedBack = false;
        if (player_Combat != null) player_Combat.RestoreWeapons();
    }
}