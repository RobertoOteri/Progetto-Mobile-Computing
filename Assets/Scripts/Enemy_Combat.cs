using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    public void Attack()
    {
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