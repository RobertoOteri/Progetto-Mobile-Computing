using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    public int maxHealth;

    [Header("Heart Sprites")]
    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Sprite halfHeart;

    [Header("UI References")]
    public Image[] hearts;
    public PlayerHealth playerHealth;

    void Update()
    {
        // Controllo di sicurezza
        if (playerHealth == null) return;

        // Ora usiamo currentHealth per leggere dal tuo PlayerHealth!
        health = playerHealth.currentHealth;
        maxHealth = playerHealth.maxHealth;

        // Calcola quanti cuori mostrare
        int totalHeartsVisible = maxHealth / 2;

        for (int i = 0; i < hearts.Length; i++)
        {
            // Punti vita necessari per riempire questo specifico cuore
            int heartValue = (i + 1) * 2;

            // Gestione dello Sprite (Pieno, Metà, Vuoto)
            if (health >= heartValue)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (health == heartValue - 1)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            // Gestione della visibilità dell'icona
            if (i < totalHeartsVisible)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}