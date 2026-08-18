using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Navigazione Pannelli")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;
    
    [Header("Impostazioni Audio")]
    public int volumeCorrente = 5;
    public int volumeMassimo = 10;
    public GameObject[] taccheVolume;

    [Header("Impostazioni Luminosità")]
    public Image filtroLuminosita;        // Il pannello nero semitrasparente
    public int luminositaCorrente = 10;   // Luminosità di partenza (es. 10 su 10, schermo chiaro)
    public int luminositaMassima = 10;
    public GameObject[] taccheLuminosita; // La lista delle immagini delle tacche luminosità

    void Start()
    {
        // All'avvio, imposta sia il volume che la luminosità
        ApplicaVolume();
        ApplicaLuminosita();
    }

    // --- FUNZIONI NAVIGAZIONE MENU ---
    public void Gioca() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    public void Esci() { Application.Quit(); }
    public void ApriImpostazioni() { pannelloPrincipale.SetActive(false); pannelloImpostazioni.SetActive(true); }
    public void ChiudiImpostazioni() { pannelloImpostazioni.SetActive(false); pannelloPrincipale.SetActive(true); }

    // --- FUNZIONI AUDIO ---
    public void AumentaVolume() { if (volumeCorrente < volumeMassimo) { volumeCorrente++; ApplicaVolume(); } }
    public void DiminuisciVolume() { if (volumeCorrente > 0) { volumeCorrente--; ApplicaVolume(); } }

    private void ApplicaVolume()
    {
        AudioListener.volume = (float)volumeCorrente / volumeMassimo;
        for (int i = 0; i < taccheVolume.Length; i++) { taccheVolume[i].SetActive(i < volumeCorrente); }
    }

    // --- NUOVE FUNZIONI LUMINOSITÀ ---
    public void AumentaLuminosita()
    {
        if (luminositaCorrente < luminositaMassima)
        {
            luminositaCorrente++;
            ApplicaLuminosita();
        }
    }

    public void DiminuisciLuminosita()
    {
        // Ci fermiamo a 1 (invece di 0) per non far diventare lo schermo del tutto nero!
        if (luminositaCorrente > 1)
        {
            luminositaCorrente--;
            ApplicaLuminosita();
        }
    }

    private void ApplicaLuminosita()
    {
        // 1. Calcola la trasparenza.
        // Se luminositaCorrente è 10, valoreLuminosita è 1. (Trasparenza al 0% = Schermo Chiaro)
        // Se luminositaCorrente è 5, valoreLuminosita è 0.5 (Trasparenza al 50% = Schermo Scuro)
        float valoreLuminosita = (float)luminositaCorrente / luminositaMassima;

        // Prendi il colore del filtro nero e cambiagli la trasparenza (Alpha 'a')
        Color coloreFiltro = filtroLuminosita.color;
        coloreFiltro.a = 1f - valoreLuminosita; // Invertiamo: meno luminosità = più nero (Alpha alto)
        filtroLuminosita.color = coloreFiltro;

        // 2. Accende e spegne le tacche visive della luminosità
        for (int i = 0; i < taccheLuminosita.Length; i++)
        {
            taccheLuminosita[i].SetActive(i < luminositaCorrente);
        }
    }
}