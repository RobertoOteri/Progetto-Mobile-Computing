using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;

    [Header("Navigazione UI")]
    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject blurOverlay; // Il BlurOverlay dentro PausaCanvas
    public Button pauseButton;
    public Button saveButton;

    [Header("HUD / Gioco")]
    public GameObject healthCanvas; // Canvas dei cuori/salute

    [Header("Comandi Mobile")]
    public GameObject mobileControlsCanvas;

    [Header("Feedback Salvataggio")]
    public GameObject testoSalvataggioRiuscito;
    private Coroutine feedbackCoroutine;

    [Header("Audio Menu")]
    public AudioClip buttonClickSFX;

    public string mainMenuSceneName = "Menu";
    private bool isPaused = false;

    void Awake()
    {
        // Imposta il frame rate a 60 FPS e disattiva il VSync per eliminare il blocco a 30 FPS su Android
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            pauseMenuPanel = null;
            settingsMenuPanel = null;
            blurOverlay = null;
            pauseButton = null;
            saveButton = null;
            testoSalvataggioRiuscito = null;
            mobileControlsCanvas = null;
            healthCanvas = null;
            return;
        }

        PausaCanvasFinder();

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void PausaCanvasFinder()
    {
        // 1. Cerca il Canvas di Pausa (blur e contenitore pausa)
        GameObject pausaCanvas = GameObject.Find("PausaCanvas");
        if (pausaCanvas != null)
        {
            Transform blurT = pausaCanvas.transform.Find("BlurOverlay");
            if (blurT != null)
            {
                blurOverlay = blurT.gameObject;
                blurOverlay.SetActive(false);
            }

            Transform pauseCont = pausaCanvas.transform.Find("Pause_Container");
            if (pauseCont != null)
            {
                pauseMenuPanel = pauseCont.gameObject;
                pauseMenuPanel.SetActive(false);
            }
        }

        // 2. Cerca SettingsCanvas (anche se disattivato nella scena)
        GameObject settingsCanvas = GameObject.Find("SettingsCanvas");
        if (settingsCanvas == null)
        {
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in rootObjects)
            {
                if (root.name == "SettingsCanvas")
                {
                    settingsCanvas = root;
                    break;
                }
            }
        }

        if (settingsCanvas != null)
        {
            settingsMenuPanel = settingsCanvas;
            settingsMenuPanel.SetActive(false);
        }

        // 3. Cerca i comandi mobile touch
        if (mobileControlsCanvas == null)
        {
            mobileControlsCanvas = GameObject.Find("MobileButtonsCanvas");
            if (mobileControlsCanvas == null) mobileControlsCanvas = GameObject.Find("MobileControls");
        }

        // 4. Cerca l'Health Canvas / Cuori (se ha un nome diverso, aggiungilo qui)
        if (healthCanvas == null)
        {
            healthCanvas = GameObject.Find("HealthCanvas");
            if (healthCanvas == null) healthCanvas = GameObject.Find("HealthBarCanvas");
            if (healthCanvas == null) healthCanvas = GameObject.Find("HeartsCanvas");
            if (healthCanvas == null) healthCanvas = GameObject.Find("HUDCanvas");
        }

        // 5. Cerca e assegna il pulsante Pausa
        GameObject b = GameObject.Find("BottonePausa");
        if (b == null) b = GameObject.Find("PauseButton");
        if (b != null)
        {
            pauseButton = b.GetComponent<Button>();
            pauseButton.gameObject.SetActive(true);
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(Pause);
        }

        // 6. Collega il tasto Salva e il feedback testuale
        if (pauseMenuPanel != null)
        {
            Button[] buttons = pauseMenuPanel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.gameObject.name == "Save" || btn.gameObject.name == "BottoneSalva")
                {
                    saveButton = btn;
                    saveButton.onClick.RemoveAllListeners();
                    saveButton.onClick.AddListener(SaveGameFromPause);
                    break;
                }
            }

            Transform fb = pauseMenuPanel.transform.Find("SaveSuccess_TXT");
            if (fb != null)
            {
                testoSalvataggioRiuscito = fb.gameObject;
                testoSalvataggioRiuscito.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == mainMenuSceneName) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void PlayButtonSound()
    {
        if (AudioManager.Instance != null && buttonClickSFX != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(buttonClickSFX, 0.1f);
        }
    }

    public void SaveGameFromPause()
    {
        PlayButtonSound();

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
            Debug.Log("Partita salvata con successo!");

            if (testoSalvataggioRiuscito != null)
            {
                if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
                feedbackCoroutine = StartCoroutine(MostraFeedbackSalvataggio());
            }
        }
    }

    private IEnumerator MostraFeedbackSalvataggio()
    {
        testoSalvataggioRiuscito.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        testoSalvataggioRiuscito.SetActive(false);
    }

    public void Resume()
    {
        PlayButtonSound();

        if (testoSalvataggioRiuscito != null) testoSalvataggioRiuscito.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (blurOverlay != null) blurOverlay.SetActive(false);

        // Riattiva i controlli mobile, il tasto pausa e i cuori
        if (mobileControlsCanvas != null) mobileControlsCanvas.SetActive(true);
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        if (healthCanvas != null) healthCanvas.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        PlayButtonSound();

        if (pauseMenuPanel == null || blurOverlay == null) PausaCanvasFinder();

        // Nasconde i comandi touch, il tasto pausa e i cuori
        if (mobileControlsCanvas != null) mobileControlsCanvas.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        if (healthCanvas != null) healthCanvas.SetActive(false);

        if (testoSalvataggioRiuscito != null) testoSalvataggioRiuscito.SetActive(false);
        
        // Attiva il blur e il pannello di pausa
        if (blurOverlay != null) blurOverlay.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings()
    {
        PlayButtonSound();

        if (settingsMenuPanel == null) PausaCanvasFinder();

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
        if (settingsMenuPanel != null) 
        {
            settingsMenuPanel.SetActive(true); 

            SettingsManager sm = settingsMenuPanel.GetComponent<SettingsManager>();
            if (sm == null) sm = settingsMenuPanel.GetComponentInChildren<SettingsManager>(true);
            if (sm != null)
            {
                sm.CloseAllInfoPanels();
            }
        }

        if (blurOverlay != null) blurOverlay.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayButtonSound();
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        PlayButtonSound();

        if (blurOverlay != null) blurOverlay.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        if (healthCanvas != null) healthCanvas.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}