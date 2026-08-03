using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Gioca()
    {
        // Carica la scena successiva in coda (il tuo gioco)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Esci()
    {
        // Chiude l'applicazione (funziona solo nel gioco esportato, non nell'editor)
        Debug.Log("Uscita dal gioco!");
        Application.Quit();
    }
}