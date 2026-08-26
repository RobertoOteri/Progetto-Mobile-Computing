using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Riferimenti UI")]
    [Tooltip("Il pannello principale del Game Over con l'immagine")]
    public GameObject gameOverPanel;

    [Tooltip("Trascina qui il Canvas o il pannello dell'HUD (cuori, munizioni, ecc.) per nasconderlo")]
    public GameObject hudCanvasOrPanel;

    [Tooltip("Trascina qui il Canvas o il contenitore dei comandi mobile (MobileControls / MobileButtonsCanvas)")]
    public GameObject mobileControlsCanvasOrPanel;

    [Tooltip("Trascina qui il pulsante di pausa o l'oggetto del menu di pausa")]
    public GameObject pauseButtonOrCanvas;

    public AudioMixerGroup outputAudioGroup;
    public AudioMixerGroup sfxAudioGroup;

    [Header("Audio Game Over")]
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverMusicClip;
    public AudioClip buttonClickSound;

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

            // Porta il Canvas del GameOver al massimo livello visivo (sopra a tutto il resto)
            Canvas parentCanvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                parentCanvas.sortingOrder = 999;
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

            if (outputAudioGroup != null)
            {
                gameOverAudioSource.outputAudioMixerGroup = outputAudioGroup;
            }
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
        // 1. Spegne l'HUD dei cuori
        if (hudCanvasOrPanel != null)
        {
            hudCanvasOrPanel.SetActive(false);
        }

        // 2. Spegne i comandi touch/mobile
        if (mobileControlsCanvasOrPanel != null)
        {
            mobileControlsCanvasOrPanel.SetActive(false);
        }

        // 3. Spegne il tasto pausa assegnato
        if (pauseButtonOrCanvas != null)
        {
            pauseButtonOrCanvas.SetActive(false);
        }

        // 4. Cerca e spegne direttamente PauseMenuManager e tutti i suoi bottoni/canvas
        PauseMenuManager pauseManager = Object.FindAnyObjectByType<PauseMenuManager>();
        if (pauseManager != null)
        {
            pauseManager.gameObject.SetActive(false);
        }

        // 5. Cerca eventuali oggetti con nome 'Pausa' o simili nella scena e li spegne
        string[] possibleNames = { "PauseButton", "BtnPause", "PauseCanvas", "Pause_Btn", "ButtonPause", "Pause" };
        foreach (string n in possibleNames)
        {
            GameObject obj = GameObject.Find(n);
            if (obj != null && obj != gameObject && obj != gameOverPanel)
            {
                obj.SetActive(false);
            }
        }

        // 6. Ferma l'audio del mondo di gioco
        SilenceWorldAudio();

        // 7. Musica GameOver
        PlayGameOverMusic();

        // 8. Dissolvenza pannello
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

        AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
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

    public void PlayButtonSound()
    {
        if (AudioManager.Instance != null && buttonClickSound != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(buttonClickSound, 1f);
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

    // --- PULSANTI UI GAME OVER ---

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void LoadMainMenu(string menuSceneName)
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}