using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    [Header("--- Audio ---")]
    [SerializeField] private AudioClip attackSFX; 
    [Range(0f, 1f)]
    [SerializeField] private float attackVolume = 0.4f;

    public void Attack()
    {
        if (attackSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(attackSFX, attackVolume);
        }

        // Rileva gli oggetti nel raggio d'attacco
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0)
        {
            // Recupera il PlayerHealth dall'oggetto colpito
            PlayerHealth playerHealth = hits[0].GetComponent<PlayerHealth>();

            // Esegue l'attacco SOLO se il giocatore esiste e NON è morto
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.ChangeHealth(-damage);

                PlayerMovement playerMovement = hits[0].GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.Knockback(transform, knockbackForce, stunTime);
                }
            }
        }
    }
}