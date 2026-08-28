using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;

    [Header("Riferimenti UI")]
    public GameObject endGameCanvas;
    public CanvasGroup fadeOverlayCanvasGroup;
    public CanvasGroup contentCanvasGroup;
    public Button returnToMenuButton;

    [Header("Impostazioni")]
    public float fadeDuration = 1.8f;
    public string mainMenuSceneName = "Menu";

    [Header("Audio")]
    public AudioClip endingMusicOrStinger;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (fadeOverlayCanvasGroup != null) fadeOverlayCanvasGroup.alpha = 0f;
        if (contentCanvasGroup != null) contentCanvasGroup.alpha = 0f;

        if (endGameCanvas != null)
            endGameCanvas.SetActive(false);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(BackToMainMenu);
    }

    public void StartEndingSequence()
    {
        if (fadeOverlayCanvasGroup != null)
        {
            fadeOverlayCanvasGroup.alpha = 0f;
            fadeOverlayCanvasGroup.blocksRaycasts = false;
        }

        if (contentCanvasGroup != null)
        {
            contentCanvasGroup.alpha = 0f;
            contentCanvasGroup.blocksRaycasts = false;
        }

        if (endGameCanvas != null)
            endGameCanvas.SetActive(true);

        StartCoroutine(EndingRoutine());
    }

    private IEnumerator EndingRoutine()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            if (endingMusicOrStinger != null)
            {
                AudioManager.Instance.PlayMusic(endingMusicOrStinger, 0.6f);
            }
        }

        // 1. Dissolvenza a scuro (FadeOverlay)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (fadeOverlayCanvasGroup != null)
                fadeOverlayCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        if (fadeOverlayCanvasGroup != null) fadeOverlayCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(0.4f);

        // 2. Comparsa del contenuto (ContentPanel)
        timer = 0f;
        while (timer < 1.2f)
        {
            timer += Time.deltaTime;
            if (contentCanvasGroup != null)
                contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / 1.2f);
            yield return null;
        }

        if (contentCanvasGroup != null)
        {
            contentCanvasGroup.alpha = 1f;
            contentCanvasGroup.blocksRaycasts = true;
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}