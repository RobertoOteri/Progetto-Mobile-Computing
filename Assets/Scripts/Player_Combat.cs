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
    public GameObject rifleObject;
    public GameObject gunObject;

    [Header("Stato Equipaggiamento")]
    public bool hasSword = false;
    public bool hasHammer = false;
    public bool hasRifle = false;
    public bool hasGun = false;

    private void Start()
    {
        UpdateWeaponVisibility();
        UpdateAnimatorBools();
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

            if (vert > 0)       attackPoint.localPosition = new Vector3(0f, offset, 0f);  // Guarda in alto
            else if (vert < 0) attackPoint.localPosition = new Vector3(0f, -offset, 0f); // Guarda in basso
            else if (horiz > 0) attackPoint.localPosition = new Vector3(offset, 0f, 0f);  // Guarda a destra
            else if (horiz < 0) attackPoint.localPosition = new Vector3(-offset, 0f, 0f); // Guarda a sinistra
        }

        anim.SetFloat("vertical", vert);
        anim.SetFloat("horizontal", horiz);
        anim.SetBool("hasSword", hasSword);
        anim.SetBool("hasHammer", hasHammer);
        anim.SetBool("isAttacking", true);

        if (AudioManager.Instance != null)
        {
            if (hasSword)
            {
                AudioManager.Instance.PlaySFXWithVolume(AudioManager.Instance.swordAttackSFX,0.3f);
            }
            else if (hasHammer)
            {
                AudioManager.Instance.PlaySFXWithVolume(AudioManager.Instance.hammerAttackSFX,0.3f);
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,weaponRange);
    }

    public void EquipSword()
    {
        hasSword = true;
        hasHammer = false;
        hasRifle = false;
        hasGun = false;
        UpdateWeaponVisibility();
        UpdateAnimatorBools();
    }

    public void EquipHammer()
    {
        hasSword = false;
        hasHammer = true;
        hasRifle = false;
        hasGun = false;
        UpdateWeaponVisibility();
        UpdateAnimatorBools();
    }

    public void EquipRifle()
    {
        hasSword = false;
        hasHammer = false;
        hasRifle = true;
        hasGun = false;
        UpdateWeaponVisibility();
        UpdateAnimatorBools();
    }

    // AGGIUNTO: Metodo per equipaggiare la Pistola
    public void EquipGun()
    {
        hasSword = false;
        hasHammer = false;
        hasRifle = false;
        hasGun = true;
        UpdateWeaponVisibility();
        UpdateAnimatorBools();
    }

    // Viene chiamata ogni volta che l'arma viene cambiata oppure all'inizio quando non ha nessun arma
    private void UpdateWeaponVisibility()
    {
        if (swordObject != null) swordObject.SetActive(hasSword);
        if (hammerObject != null) hammerObject.SetActive(hasHammer);
        if (rifleObject != null) rifleObject.SetActive(hasRifle);
        if (gunObject != null) gunObject.SetActive(hasGun);
    }

    private void UpdateAnimatorBools()
    {
        if(anim == null) return;

        anim.SetBool("hasSword", hasSword);
        anim.SetBool("hasHammer", hasHammer);
        anim.SetBool("hasRifle", hasRifle);
        anim.SetBool("hasGun", hasGun);
    }
}