using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Riferimenti UI")]
    public GameObject gameOverPanel;
    public GameObject hudCanvasOrPanel;

    [Header("Audio Game Over")]
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverMusicClip;
    public AudioMixerGroup musicMixerGroup; 

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
        // 1. Nasconde l'HUD
        if (hudCanvasOrPanel != null)
        {
            hudCanvasOrPanel.SetActive(false);
        }

        // 2. Sfuma la musica principale di sottofondo in 1.5 secondi
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutMusic(1.5f);
        }

        // 3. Ferma gli altri suoni del mondo
        SilenceWorldAudio();

        // 4. Riproduce la musica di Game Over (collegata al Mixer)
        PlayGameOverMusic();

        // 5. Avvia la dissolvenza della schermata
        if (gameOverPanel != null)
        {
            gameOverPanel.transform.SetAsLastSibling();
            StartCoroutine(FadeInRoutine());
        }
    }

    private void SilenceWorldAudio()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopWalkSound();
            AudioManager.Instance.StopTypewriterSound();
        }

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
            if (musicMixerGroup != null)
            {
                gameOverAudioSource.outputAudioMixerGroup = musicMixerGroup;
            }

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