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
    public GameObject blurOverlay; 
    public Button pauseButton;
    public Button saveButton;

    [Header("HUD / Gioco")]
    public GameObject healthCanvas; 

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
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        instance = this;
    }

    void Start()
    {
        PausaCanvasFinder();
        ForceCloseAll();
    }

    private void ForceCloseAll()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (blurOverlay != null) blurOverlay.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (testoSalvataggioRiuscito != null) testoSalvataggioRiuscito.SetActive(false);

        if (mobileControlsCanvas != null) mobileControlsCanvas.SetActive(true);
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        if (healthCanvas != null) healthCanvas.SetActive(true);
    }

    public void PausaCanvasFinder()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            if (root.name == "PausaCanvas")
            {
                Transform blurT = root.transform.Find("BlurOverlay");
                if (blurT != null) blurOverlay = blurT.gameObject;

                Transform pauseCont = root.transform.Find("Pause_Container");
                if (pauseCont != null) pauseMenuPanel = pauseCont.gameObject;

                // Cerca SaveSuccess_TXT sia dentro PausaCanvas che dentro Pause_Container
                Transform fb = root.transform.Find("SaveSuccess_TXT");
                if (fb == null && pauseCont != null) fb = pauseCont.Find("SaveSuccess_TXT");
                if (fb != null)
                {
                    testoSalvataggioRiuscito = fb.gameObject;
                    testoSalvataggioRiuscito.SetActive(false);
                }

                break;
            }
        }

        foreach (GameObject root in rootObjects)
        {
            if (root.name == "SettingsCanvas")
            {
                settingsMenuPanel = root;
                break;
            }
        }

        foreach (GameObject root in rootObjects)
        {
            if (root.name == "MobileButtonsCanvas" || root.name == "MobileControls")
            {
                mobileControlsCanvas = root;
                break;
            }
        }

        foreach (GameObject root in rootObjects)
        {
            if (root.name == "HealthCanvas" || root.name == "HealthBarCanvas" || 
                root.name == "HeartsCanvas" || root.name == "HUDCanvas")
            {
                healthCanvas = root;
                break;
            }
        }

        GameObject b = GameObject.Find("BottonePausa");
        if (b == null) b = GameObject.Find("PauseButton");
        if (b != null)
        {
            pauseButton = b.GetComponent<Button>();
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(Pause);
            }
        }

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
            AudioManager.Instance.PlaySFXWithVolume(buttonClickSFX, 0.3f);
        }
    }

    public void SaveGameFromPause()
    {
        PlayButtonSound();

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();

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

        if (mobileControlsCanvas != null) mobileControlsCanvas.SetActive(true);
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        if (healthCanvas != null) healthCanvas.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RestoreBatGroupParam();
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        PlayButtonSound();

        if (pauseMenuPanel == null || blurOverlay == null) PausaCanvasFinder();

        if (mobileControlsCanvas != null) mobileControlsCanvas.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        if (healthCanvas != null) healthCanvas.SetActive(false);

        if (testoSalvataggioRiuscito != null) testoSalvataggioRiuscito.SetActive(false);
        
        if (blurOverlay != null) blurOverlay.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSFX(); 
        }

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
                sm.ResetPanelsWithoutSound(); 
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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RestoreBatGroupParam();
        }

        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}