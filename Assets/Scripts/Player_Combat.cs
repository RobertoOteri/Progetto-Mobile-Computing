using UnityEngine;

public class Player_Combat : MonoBehaviour
{
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

        anim.SetFloat("vertical", vert);
        anim.SetFloat("horizontal", horiz);
        anim.SetBool("hasSword", hasSword);
        anim.SetBool("hasHammer", hasHammer);
        anim.SetBool("isAttacking", true);
    }

    public void FinishAttack()
    {
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
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