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

    public AudioMixerGroup outputAudioGroup;
    public AudioMixerGroup sfxAudioGroup;

    [Header("Audio Game Over")]
    [Tooltip("AudioSource per la musica di Game Over (se vuoto ne userà uno automatico)")]
    public AudioSource gameOverAudioSource;
    [Tooltip("Traccia musicale o SFX di Game Over")]
    public AudioClip gameOverMusicClip;
    [Tooltip("Suono al click dei pulsanti (Esci / Rigioca)")]
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

        // Cerca automaticamente i comandi mobile se lo slot non è assegnato a mano
        if (mobileControlsCanvasOrPanel == null)
        {
            mobileControlsCanvasOrPanel = GameObject.Find("MobileControls");
            if (mobileControlsCanvasOrPanel == null)
                mobileControlsCanvasOrPanel = GameObject.Find("MobileButtonsCanvas");
        }
    }

    public void TriggerGameOver()
    {
        // 1. Nasconde l'HUD dei cuori
        if (hudCanvasOrPanel != null)
        {
            hudCanvasOrPanel.SetActive(false);
        }

        // 2. Nasconde i controlli touch (Joystick e Pulsanti Mobile)
        if (mobileControlsCanvasOrPanel != null)
        {
            mobileControlsCanvasOrPanel.SetActive(false);
        }

        // 3. Ferma i suoni di sottofondo/nemici nel mondo
        SilenceWorldAudio();

        // 4. Riproduce la musica di Game Over
        PlayGameOverMusic();

        // 5. Avvia la dissolvenza del pannello
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
            gameOverAudioSource.clip = gameOverMusicClip;
            gameOverAudioSource.loop = false;
            gameOverAudioSource.Play();
        }
    }

    public void PlayButtonSound()
    {
        // Riproduce il click del pulsante tramite l'AudioManager
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