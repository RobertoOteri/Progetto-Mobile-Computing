using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    [Header("Impostazioni Spoletta ed Esplosione")]
    public float fuseTime = 1.8f;          // Secondi prima dello scoppio
    public float explosionRadius = 2f;     // Raggio del cerchio di danno
    public int explosionDamage = 3;
    public float knockbackForce = 30f;
    public LayerMask enemyLayer;

    private Animator anim;
    private Rigidbody2D rb;
    private bool hasExploded = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Avvia il timer di esplosione
        Invoke(nameof(TriggerExplosion), fuseTime);
    }

    public void Launch(Vector2 direction, float force)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * force;
        }
    }

    private void TriggerExplosion()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Ferma lo scivolamento
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Avvia l'animazione di esplosione
        if (anim != null)
        {
            anim.SetTrigger("explode");
        }

        // Applica danno e knockback ai nemici nell'area
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            Enemy_Health health = enemy.GetComponent<Enemy_Health>();
            if (health != null)
            {
                health.ChangeHealth(-explosionDamage);
            }

            Enemy_Knockback kb = enemy.GetComponent<Enemy_Knockback>();
            if (kb != null)
            {
                kb.Knockback(transform, knockbackForce, 0.2f, 0.35f);
            }
        }

        // Distrugge l'oggetto al termine dell'animazione
        Destroy(gameObject, 0.6f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}