using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Audio Personalizzato Nemico")]
    public AudioClip hitSound; // 🟢 Suono quando viene colpito
    public float hitSoundVolume = 0.5f;

    [Space]
    public AudioClip deathSound; // Suono quando muore
    public float deathSoundVolume = 0.5f; 

    private Animator anim;
    private Collider2D col;

    private void Start(){
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void ChangeHealth(int amount){

        currentHealth += amount;

        // 🟢 Se il nemico ha subito danni (amount < 0) e non è ancora morto
        if (amount < 0 && currentHealth > 0)
        {
            if (hitSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXWithVolume(hitSound, hitSoundVolume);
            }
        }

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

        if (deathSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(deathSound, deathSoundVolume);
        }

        Destroy(gameObject, 0.7f);
    }
}