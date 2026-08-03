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
            Die(); 
        }
    }

    private void Die(){

        Enemy_Movement movement = GetComponent<Enemy_Movement>();
        if (movement != null) {
        movement.enabled = false;
        }

        if(anim != null){
            anim.SetTrigger("die");
        }

        if(col != null){
            col.enabled = false;
        }

        Destroy(gameObject, 0.7f);
    }
}