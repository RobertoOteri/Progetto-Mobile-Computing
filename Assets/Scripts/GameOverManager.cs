using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Riferimenti UI")]
    [Tooltip("Il pannello principale del Game Over con l'immagine")]
    public GameObject gameOverPanel;

    [Tooltip("Trascina qui il Canvas o il pannello dell'HUD (cuori, munizioni, ecc.) per nasconderlo")]
    public GameObject hudCanvasOrPanel;

    [Header("Audio Game Over")]
    [Tooltip("AudioSource per la musica di Game Over (se vuoto ne userà uno automatico)")]
    public AudioSource gameOverAudioSource;
    [Tooltip("Traccia musicale o SFX di Game Over")]
    public AudioClip gameOverMusicClip;

    [Header("Impostazioni Dissolvenza")]
    public float fadeDuration = 1.0f;
    public bool pauseGameOnDeath = true;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (gameOverPanel != null)
        {
            canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }
        }

        if (gameOverAudioSource == null)
        {
            gameOverAudioSource = GetComponent<AudioSource>();
            if (gameOverAudioSource == null)
            {
                gameOverAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Assicura che la musica di Game Over suoni anche se il tempo di gioco viene fermato
        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.ignoreListenerPause = true;
            gameOverAudioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void TriggerGameOver()
    {
        // 1. Nasconde l'HUD dei cuori
        if (hudCanvasOrPanel != null)
        {
            hudCanvasOrPanel.SetActive(false);
        }

        // 2. Ferma i suoni di sottofondo/nemici nel mondo
        SilenceWorldAudio();

        // 3. Riproduce la musica di Game Over
        PlayGameOverMusic();

        // 4. Avvia la dissolvenza
        if (gameOverPanel != null)
        {
            gameOverPanel.transform.SetAsLastSibling();
            StartCoroutine(FadeInRoutine());
        }
    }

    private void SilenceWorldAudio()
    {
        // Ferma le tracce gestite dall'AudioManager se presente
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopWalkSound();
            AudioManager.Instance.StopTypewriterSound();
        }

        // Trova e muta tutti gli altri AudioSource attivi nella scena (nemici, trappole, ecc.)
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            if (source != gameOverAudioSource)
            {
                source.Stop();
            }
        }
    }

    private void PlayGameOverMusic()
    {
        if (gameOverAudioSource != null && gameOverMusicClip != null)
        {
            gameOverAudioSource.clip = gameOverMusicClip;
            gameOverAudioSource.loop = false;
            gameOverAudioSource.Play();
        }
    }

    private IEnumerator FadeInRoutine()
    {
        gameOverPanel.SetActive(true);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }
    }

    // --- PULSANTI ---

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void LoadMainMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}