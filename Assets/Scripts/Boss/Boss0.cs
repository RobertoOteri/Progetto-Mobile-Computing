using UnityEngine;

public class BossBase : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float stopDistance = 1.5f;

    private Rigidbody2D rb;
    private Animator anim;
    
    private Vector3 startScale; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        // Salviamo la grandezza originale
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

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            
            // Si muove fisicamente
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            
            // Attiva l'animazione con il parametro esatto richiesto
            anim.SetBool("Moving", true);

            // Capovolge il modello passando la direzione sull'asse X
            Flip(direction.x);
        }
        else
        {
            // Si ferma
            anim.SetBool("Moving", false);
        }
    }

    void Flip(float moveDirectionX)
    {
        if (moveDirectionX < 0)
        {
            // Sinistra
            transform.localScale = new Vector3(-Mathf.Abs(startScale.x), startScale.y, startScale.z);
        }
        else if (moveDirectionX > 0)
        {
            // Destra
            transform.localScale = new Vector3(Mathf.Abs(startScale.x), startScale.y, startScale.z);
        }
    }
}