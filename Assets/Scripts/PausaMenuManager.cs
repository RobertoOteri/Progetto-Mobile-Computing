using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;
    
    [Header("Navigazione UI")]
    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    
    private bool isPaused = false;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        if(settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
    }

    void Update()
    {
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
        pauseMenuPanel.SetActive(true); // Nel gioco, tornando indietro riapre sempre la pausa
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        isPaused = false;
        // Distruggiamo il Canvas di pausa quando torniamo al menù, così non si porta dietro cose vecchie!
        Destroy(gameObject); 
        SceneManager.LoadScene("Menu"); 
    }
}