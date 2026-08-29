using UnityEngine;

public class BossBase : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float speed = 2f;
    
    // LA SOLUZIONE AL TREMOLIO: La doppia distanza
    public float stopDistance = 1.5f;   // Distanza a cui si ferma
    public float resumeDistance = 1.8f; // Distanza a cui riparte (DEVE essere maggiore di stopDistance!)

    [Header("Attacco")]
    public float attackDuration = 1f; 
    public float attackCooldown = 2f; 
    
    private float nextAttackTime = 0f; 
    private float attackEndTime = 0f; 

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 startScale; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startScale = transform.localScale; 

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;
        
        // Lucchetto: se sta attaccando, ignora tutto il resto
        if (Time.time < attackEndTime) return; 

        float distance = Vector2.Distance(transform.position, player.position);

        // Controllo: Stiamo già camminando?
        if (anim.GetBool("Moving") == true)
        {
            if (distance <= stopDistance)
            {
                // Arrivati! Ci fermiamo
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("Moving", false);
            }
            else
            {
                // Continuiamo l'inseguimento
                ChasePlayer();
            }
        }
        else // Controllo: Siamo fermi?
        {
            if (distance > resumeDistance)
            {
                // Il player è scappato lontano, ripartiamo!
                anim.SetBool("Moving", true);
            }
            else
            {
                // Siamo vicini e fermi. Assicuriamoci che non scivoli
                rb.linearVelocity = Vector2.zero;

                // Attacchiamo se il timer è pronto
                if (Time.time >= nextAttackTime)
                {
                    anim.SetTrigger("Attack"); 
                    attackEndTime = Time.time + attackDuration; 
                    nextAttackTime = Time.time + attackCooldown; 
                }
            }
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed; 
        Flip(direction.x);
    }

    void Flip(float moveDirectionX)
    {
        if (moveDirectionX < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(startScale.x), startScale.y, startScale.z);
        else if (moveDirectionX > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(startScale.x), startScale.y, startScale.z);
    }
}