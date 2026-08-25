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
    
    [Header("Audio Menu")]
    public AudioClip buttonClickSFX;

    public string mainMenuSceneName = "Menu";
    private bool isPaused = false;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            // Se siamo nel menu principale, nascondi il pulsante di pausa se presente
            if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        }
        else
        {
            // --- RICERCA AUTOMATICA DEI PANNELLI E DEL BOTTONE IN GIOCO ---
            
            // Se i riferimenti sono andati persi (Missing/Null), li ritrova nella nuova scena
            if (pauseMenuPanel == null)
            {
                GameObject p = GameObject.Find("Pause_Container"); // Sostituisci con il NOME ESATTO del GameObject nel Canvas
                if (p != null) pauseMenuPanel = p;
            }

            if (settingsMenuPanel == null)
            {
                GameObject s = GameObject.Find("SettingsContainer"); // Sostituisci con il NOME ESATTO del Canvas impostazioni
                if (s != null) settingsMenuPanel = s;
            }

            if (pauseButton == null)
            {
                GameObject b = GameObject.Find("BottonePausa"); // NOME ESATTO del pulsante di pausa nella Hierarchy
                if (b != null) pauseButton = b.GetComponent<Button>();
            }

            // --- ASSEGNAZIONE EVENTI ---
            if (pauseButton != null) 
            {
                pauseButton.gameObject.SetActive(true);
                
                // Pulisce e assegna il listener al nuovo bottone
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(Pause);
            }
            
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        }

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

    // --- FUNZIONE PER RIPRODURRE IL SUONO ---
    public void PlayButtonSound()
    {
        if (AudioManager.Instance != null && buttonClickSFX != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(buttonClickSFX, 1f);
        }
    }

    // --- FUNZIONI DI NAVIGAZIONE ---
    public void Resume()
    {
        PlayButtonSound();
        pauseMenuPanel.SetActive(false);
        if(settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        PlayButtonSound();
        Debug.Log("Pausa attivata con successo!");
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings() 
    { 
        PlayButtonSound();
        pauseMenuPanel.SetActive(false); 
        settingsMenuPanel.SetActive(true); 
    }

    public void CloseSettings()
    {
        PlayButtonSound();
        settingsMenuPanel.SetActive(false); 
        pauseMenuPanel.SetActive(true); 
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