using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager instance;
    public GameObject pauseMenuPanel;
    public string mainMenuSceneName = "Menu"; // Inserisci il nome esatto della scena del menù principale
    
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

    void Start() { pauseMenuPanel.SetActive(false); }

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
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void SaveGame() { Debug.Log("Salvataggio..."); }
    public void OpenSettings() { Debug.Log("Apro le impostazioni..."); }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}