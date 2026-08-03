using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    // Aggiungiamo i riferimenti ai componenti
    private Animator anim;
    private Collider2D col;

    private void Start(){
        currentHealth = maxHealth;

        // Recuperiamo in automatico Animator e Collider presente sul nemico
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void ChangeHealth(int amount){

        currentHealth += amount;

        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
        else if(currentHealth <= 0){
            Die(); // Chiamiamo la funzione di morte
        }
    }

    private void Die(){
        // 1. Attiva il Trigger dell'animazione di morte
        if(anim != null){
            anim.SetTrigger("die");
        }

        // 2. Disattiva il collider per non far subire più colpi al player
        if(col != null){
            col.enabled = false;
        }

        // 3. (Opzionale) Se hai uno script di movimento sul nemico, disabilitalo qui:
        // GetComponent<EnemyMovement>().enabled = false;

        // 4. Distrugge l'oggetto dopo 1.5 secondi per dare il tempo all'animazione di finire
        Destroy(gameObject, 0.7f);
    }
}