using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;

    // Ora Attack accetta sia gli input correnti (vInput, hInput) 
    // che la memoria dell'ultimo movimento (lastV, lastH)
    public void Attack(float vInput, float hInput, float lastV, float lastH)
    {
        if (anim == null)
        {
            return;
        }

        float vert = 0f;
        float horiz = 0f;

        // Se stiamo premendo un tasto usiamo quello, 
        // altrimenti usiamo l'ultima direzione verso cui era rivolto il personaggio
        float targetV = (vInput != 0) ? vInput : lastV;
        float targetH = (hInput != 0) ? hInput : lastH;

        // Se l'ultima direzione (o quella attuale) era verticale
        if (targetV != 0 && hInput == 0)
        {
            vert = targetV > 0 ? 1f : -1f;
            horiz = 0f;
        }
        else // Altrimenti usiamo l'orizzontale
        {
            horiz = targetH < 0 ? -1f : 1f;
            vert = 0f;
        }

        // Impostiamo i parametri 
        anim.SetFloat("vertical", vert);
        anim.SetFloat("horizontal", horiz);
        anim.SetBool("isAttacking", true);

    }

    public void FinishAttack()
    {
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
            
        }
    }
}