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

    [HideInInspector] public float lastVertical = 0f;
    [HideInInspector] public float lastHorizontal = 1f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (player_Combat == null) player_Combat = GetComponent<Player_Combat>();
        if (player_Rifle == null) player_Rifle = GetComponent<Player_Rifle>();
        if (player_Gun == null) player_Gun = GetComponent<Player_Gun>();
    }

    private void Start()
    {
        if (movementJoystick == null)
        {
            movementJoystick = FindFirstObjectByType<Joystick>(FindObjectsInactive.Include);
        }
    }

    private void Update()
    {
        if (isKnockedBack) return;
        if (player_Combat != null && player_Combat.IsThrowingBomb) return;

        float h = GetHorizontalInput();
        float v = GetVerticalInput();

        if (h != 0 || v != 0)
        {
            lastHorizontal = h;
            lastVertical = v;
        }

        if (Input.GetButtonDown("Slash") || (Input.GetKeyDown(KeyCode.Q) && player_Combat != null && player_Combat.hasBomb))
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

        if (player_Combat != null && player_Combat.IsThrowingBomb)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        bool isShootingRifle = player_Rifle != null && player_Rifle.enabled && player_Rifle.IsShooting;
        bool isShootingGun = player_Gun != null && player_Gun.enabled && player_Gun.IsShooting;

        if (isShootingRifle || isShootingGun)
        {
            rb.linearVelocity = Vector2.zero;
            if (AudioManager.Instance != null) AudioManager.Instance.StopWalkSound();
            return;
        }

        float horizontal = GetHorizontalInput();
        float vertical = GetVerticalInput();

        // Flip per sprite orientato a sinistra nativamente
        if (horizontal > 0 && transform.localScale.x > 0)
        {
            Flip();
        }
        else if (horizontal < 0 && transform.localScale.x < 0)
        {
            Flip();
        }

        // GESTIONE ASSE DOMINANTE: permette a Gun_walk_up e Gun_walk_down di attivarsi
        if (anim != null)
        {
            if (Mathf.Abs(horizontal) > 0.05f || Mathf.Abs(vertical) > 0.05f)
            {
                if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical))
                {
                    anim.SetFloat("horizontal", horizontal > 0 ? 1f : -1f);
                    anim.SetFloat("vertical", 0f);
                }
                else
                {
                    anim.SetFloat("horizontal", 0f);
                    anim.SetFloat("vertical", vertical > 0 ? 1f : -1f);
                }
            }
            else
            {
                anim.SetFloat("horizontal", 0f);
                anim.SetFloat("vertical", 0f);
            }
        }

        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;

        bool isMoving = (Mathf.Abs(horizontal) > 0.05f || Mathf.Abs(vertical) > 0.05f);
        if (isMoving)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StartWalkSound();
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.StopWalkSound();
        }
    }

    public float GetHorizontalInput()
    {
        if (movementJoystick != null && Mathf.Abs(movementJoystick.Horizontal) > 0.15f)
        {
            return movementJoystick.Horizontal;
        }
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetVerticalInput()
    {
        if (movementJoystick != null && Mathf.Abs(movementJoystick.Vertical) > 0.15f)
        {
            return movementJoystick.Vertical;
        }
        return Input.GetAxisRaw("Vertical");
    }

    public void TriggerAttack()
    {
        if (player_Combat != null)
        {
            float v = GetVerticalInput();
            float h = GetHorizontalInput();
            player_Combat.ExecuteCurrentWeaponAction(v, h, lastVertical, lastHorizontal);
        }
    }

    public void StopMovement()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
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