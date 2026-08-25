using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;
    
    [Header("Navigazione UI")]
    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public Button pauseButton; // Deve essere di tipo Button, non GameObject!
    
    public string mainMenuSceneName = "Menu";
    private bool isPaused = false;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        } else {
            // Se esiste già un manager, distruggiamo questo doppione
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Se siamo nel menu principale, spegniamo il tasto pausa
        if (scene.name == mainMenuSceneName)
        {
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        }
        else
        {
            // 2. Se siamo in un livello di gioco, riaccendiamo il tasto e...
            if (pauseButton != null) 
            {
                pauseButton.gameObject.SetActive(true);
                
                // MAGIA VIA CODICE: Pulisce i vecchi collegamenti e attacca questo script al bottone nuovo!
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(Pause);
            }
        }
        
        pauseMenuPanel.SetActive(false);
        if(settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
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

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        if(settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        Debug.Log("Pausa attivata con successo!");
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings() 
    { 
        pauseMenuPanel.SetActive(false); 
        settingsMenuPanel.SetActive(true); 
    }

    public void CloseSettings()
    {
        settingsMenuPanel.SetActive(false); 
        pauseMenuPanel.SetActive(true); 
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}