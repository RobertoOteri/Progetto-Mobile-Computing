using UnityEngine;

public class Player_Combat : MonoBehaviour
{   
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 25;
    public float knockbackTime = .15f;
    public float stunTime = .3f;
    public LayerMask enemyLayer;
    public int damage = 1;

    public Animator anim;

    [Header("Oggetti Figli nel Player")]
    public GameObject swordObject;  
    public GameObject hammerObject; 

    [Header("Stato Equipaggiamento")]
    public bool hasSword = false;
    public bool hasHammer = false;

    private void Start()
    {
        UpdateWeaponVisibility();
    }

    public void Attack(float vInput, float hInput, float lastV, float lastH)
    {
        if (anim == null) return;
        if (!hasSword && !hasHammer) return; // Se disarmato, non attacca

        float vert = 0f;
        float horiz = 0f;

        // Prende l'ultima direzione se non stiamo premendo nessun tasto
        float targetV = (vInput != 0) ? vInput : lastV;
        float targetH = (hInput != 0) ? hInput : lastH;

        if (targetV != 0 && hInput == 0)
        {
            vert = targetV > 0 ? 1f : -1f;
        }
        else
        {
            horiz = targetH < 0 ? -1f : 1f;
        }

        if (attackPoint != null){
        float offset = 0.8f; // Distanza dell'attacco dal centro del personaggio

        if (vert > 0)      attackPoint.localPosition = new Vector3(0f, offset, 0f);  // Guarda in alto
        else if (vert < 0) attackPoint.localPosition = new Vector3(0f, -offset, 0f); // Guarda in basso
        else if (horiz > 0) attackPoint.localPosition = new Vector3(offset, 0f, 0f);  // Guarda a destra
        else if (horiz < 0) attackPoint.localPosition = new Vector3(-offset, 0f, 0f); // Guarda a sinistra
        }

        anim.SetFloat("vertical", vert);
        anim.SetFloat("horizontal", horiz);
        anim.SetBool("hasSword", hasSword);
        anim.SetBool("hasHammer", hasHammer);
        anim.SetBool("isAttacking", true);


    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position,weaponRange,enemyLayer);
        if(enemies.Length > 0){

            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-damage);
            enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime, stunTime);
        }
    }

    public void FinishAttack()
    {
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
    }

    private void onDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,weaponRange);
    }

    public void EquipSword()
    {
        hasSword = true;
        hasHammer = false;
        UpdateWeaponVisibility();
    }

    public void EquipHammer()
    {
        hasSword = false;
        hasHammer = true;
        UpdateWeaponVisibility();
    }

    // Viene chiamata ogni volta che l'arma viene cambiata oppure all'inizio quando non ha nessun arma
    private void UpdateWeaponVisibility()
    {
        if (swordObject != null) swordObject.SetActive(hasSword);
        if (hammerObject != null) hammerObject.SetActive(hasHammer);
    }
}