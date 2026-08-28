using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public float shakeDuration = 0.2f;
    public float shakeAmount = 5f;
    private int previousHealth;
    private Vector2[] startPositions; 

    void Start()
    {
        startPositions = new Vector2[hearts.Length];
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                startPositions[i] = hearts[i].rectTransform.anchoredPosition;
        }

        if (playerHealth != null)
            previousHealth = playerHealth.currentHealth;
    }

    void Update()
    {
        int totalHeartsVisible = maxHealth / 2;

        for (int i = 0; i < hearts.Length; i++)
        {
            // AGGIUNGI QUESTA RIGA: Se lo slot del cuore è vuoto, saltalo e passa al prossimo!
            if (hearts[i] == null) continue; 

            int heartValue = (i + 1) * 2;

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

    IEnumerator ShakeHearts()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null && hearts[i].enabled)
                {
                    // Genera uno spostamento casuale
                    float offsetX = Random.Range(-1f, 1f) * shakeAmount;
                    float offsetY = Random.Range(-1f, 1f) * shakeAmount;

                    hearts[i].rectTransform.anchoredPosition = startPositions[i] + new Vector2(offsetX, offsetY);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].rectTransform.anchoredPosition = startPositions[i];
        }
    }
}