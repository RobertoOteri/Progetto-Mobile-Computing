using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pannelli del Menù")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;

    public void Gioca()
    {
        // Carica il primo livello del gioco
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Esci()
    {
        // Chiude l'applicazione
        Debug.Log("Uscita dal gioco!");
        Application.Quit();
    }

    // --- NUOVE FUNZIONI PER LE IMPOSTAZIONI ---

    public void ApriImpostazioni()
    {
        // Nasconde il menù principale e mostra quello delle impostazioni
        pannelloPrincipale.SetActive(false);
        pannelloImpostazioni.SetActive(true);
    }

    public void ChiudiImpostazioni()
    {
        // Nasconde il menù delle impostazioni e torna al principale
        pannelloImpostazioni.SetActive(false);
        pannelloPrincipale.SetActive(true);
    }
}