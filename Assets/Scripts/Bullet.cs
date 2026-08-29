using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right;
    public float lifeSpan = 2;
    public float speed;
    public int damage;

    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    public SpriteRenderer sr;

    void Start()
    {
        rb.linearVelocity = direction * speed;
        RotateBullet();
        Destroy(gameObject, lifeSpan);
    }

    private void RotateBullet()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Se colpisce un oggetto che fa parte dell'Enemy Layer
        if ((enemyLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            // 1. Controlla se è un nemico normale (ha lo script Enemy_Health)
            Enemy_Health enemyHealth = collision.gameObject.GetComponent<Enemy_Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
                
                // Opzionale: applica il knockback
                Enemy_Knockback knockback = collision.gameObject.GetComponent<Enemy_Knockback>();
                if (knockback != null)
                {
                    knockback.Knockback(transform, knockbackForce, knockbackTime, stunTime);
                }
            }

            // Distruggi il proiettile dopo aver colpito un nemico o il boss
            Destroy(gameObject);
        }
        // Se colpisce un ostacolo (es. Muro)
        else if ((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Destroy(gameObject);
        }
    }
}