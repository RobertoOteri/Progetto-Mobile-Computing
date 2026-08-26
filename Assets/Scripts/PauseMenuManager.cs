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
    public Button pauseButton;
    public Button saveButton;
    
    [Header("Feedback Salvataggio")]
    public GameObject testoSalvataggioRiuscito;
    private Coroutine feedbackCoroutine;

    [Header("Audio Menu")]
    public AudioClip buttonClickSFX;

    public string mainMenuSceneName = "Menu";
    private bool isPaused = false;

    void Awake()
    {
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
            pauseButton = null;
            saveButton = null;
            testoSalvataggioRiuscito = null;
            return;
        }

        PausaCanvasFinder();

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void PausaCanvasFinder()
    {
        GameObject pausaCanvas = GameObject.Find("PausaCanvas");
        if (pausaCanvas != null)
        {
            Transform pauseCont = pausaCanvas.transform.Find("Pause_Container");
            if (pauseCont != null)
            {
                pauseMenuPanel = pauseCont.gameObject;
            }

            Transform settingsCont = pausaCanvas.transform.Find("SettingsCanvas");
            if (settingsCont == null) settingsCont = pausaCanvas.transform.Find("SettingsContainer");
            if (settingsCont != null)
            {
                settingsMenuPanel = settingsCont.gameObject;
            }
        }

        GameObject b = GameObject.Find("BottonePausa");
        if (b == null) b = GameObject.Find("PauseButton");
        if (b != null) 
        {
            pauseButton = b.GetComponent<Button>();
            pauseButton.gameObject.SetActive(true);
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(Pause);
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

            Transform fb = pauseMenuPanel.transform.Find("SaveSuccess_TXT");
            if (fb != null)
            {
                testoSalvataggioRiuscito = fb.gameObject;
                testoSalvataggioRiuscito.SetActive(false);
            }

            pauseMenuPanel.SetActive(false);
        }

        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
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
            AudioManager.Instance.PlaySFXWithVolume(buttonClickSFX, 1f);
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
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        PlayButtonSound();
        if (pauseMenuPanel == null) PausaCanvasFinder();

        if (testoSalvataggioRiuscito != null) testoSalvataggioRiuscito.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings() 
    { 
        PlayButtonSound();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false); 
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true); 
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
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}