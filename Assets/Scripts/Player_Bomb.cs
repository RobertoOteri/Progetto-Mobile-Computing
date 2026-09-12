using UnityEngine;

public class Player_Bomb : MonoBehaviour
{
    [Header("Riferimenti")]
    public GameObject bombProjectilePrefab; 
    public Transform throwPoint;           

    [Header("Impostazioni Lancio")]
    public float throwForce = 7f;
    public KeyCode throwKey = KeyCode.Q;

    private Animator anim;
    private bool hasBomb = false;
    private Vector2 lastLookDirection = Vector2.down;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (throwPoint == null)
            throwPoint = transform;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            lastLookDirection = new Vector2(h, v).normalized;
        }

        // Tasto di Lancio
        if (hasBomb && Input.GetKeyDown(throwKey))
        {
            ThrowBombAction();
        }
    }

    public void PickUpBomb()
    {
        hasBomb = true;
        anim.SetBool("hasBomb", true);
        
        // Disattiva altre armi se necessario
        anim.SetBool("hasSword", false);
        anim.SetBool("hasGun", false);
        anim.SetBool("hasRifle", false);
        anim.SetBool("hasHammer", false);
    }

    private void ThrowBombAction()
    {
        hasBomb = false;
        anim.SetBool("hasBomb", false);
        anim.SetTrigger("throwBomb");

        // Spawn del proiettile bomba
        if (bombProjectilePrefab != null)
        {
            GameObject bomb = Instantiate(bombProjectilePrefab, throwPoint.position, Quaternion.identity);
            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = lastLookDirection * throwForce;
            }
        }
    }
}