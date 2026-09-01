using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBarUI : MonoBehaviour
{
    public static BossHealthBarUI Instance;

    [Header("Riferimenti UI")]
    public GameObject healthBarContainer;
    public Image fillImage; 
    public CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private Enemy_Health bossHealth;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(false);
        }
    }

    public void InitializeBossBar(Enemy_Health health)
    {
        bossHealth = health;
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += UpdateHealthBar;
            bossHealth.OnDeath += HideHealthBar;
            UpdateHealthBar(bossHealth.currentHealth, bossHealth.maxHealth);
        }

        StartCoroutine(FadeInBar());
    }

    private void UpdateHealthBar(int current, int max)
    {
        if (fillImage != null && max > 0)
        {
            fillImage.fillAmount = Mathf.Clamp01((float)current / max);
        }
    }

    private IEnumerator FadeInBar()
    {
        if (healthBarContainer != null) healthBarContainer.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    public void HideHealthBar()
    {
        StartCoroutine(FadeOutBar());
    }

    private IEnumerator FadeOutBar()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (healthBarContainer != null) healthBarContainer.SetActive(false);
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
            bossHealth.OnDeath -= HideHealthBar;
        }
    }
}