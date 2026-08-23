using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Navigazione Pannelli")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;
    
    [Header("Impostazioni Grafiche e Audio")]
    public Image filtroLuminosita; 
    public Slider sliderMusica;  // Sostituisce il vecchio sliderVolume
    public Slider sliderSuoni;   // Il nuovo slider per gli effetti
    public Slider sliderLuminosita;   

    void Start()
    {
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false);

        // Inizializza tutti e tre gli slider all'avvio
        if (sliderMusica != null) CambiaMusica(sliderMusica.value);
        if (sliderSuoni != null) CambiaSuoni(sliderSuoni.value);
        if (sliderLuminosita != null) CambiaLuminosita(sliderLuminosita.value);
    }

    // --- FUNZIONI NAVIGAZIONE MENU ---
    public void Gioca() 
    { 
        // Ricorda di mettere il VERO nome della tua scena Intro qui!
        SceneManager.LoadScene("Intro"); 
    }
    
    public void Esci() { Application.Quit(); }
    public void ApriImpostazioni() { pannelloPrincipale.SetActive(false); pannelloImpostazioni.SetActive(true); }
    public void ChiudiImpostazioni() { pannelloImpostazioni.SetActive(false); pannelloPrincipale.SetActive(true); }

    // --- FUNZIONI DEGLI SLIDER AUDIO ---
    
    public void CambiaMusica(float volume)
    {
        if (sliderMusica != null)
        {
            float volNormalizzato = volume / sliderMusica.maxValue;
            Debug.Log("La MUSICA è stata impostata a: " + volNormalizzato);
            
            // Qui collegheremo l'AudioMixer per la musica!
        }
    }

    public void CambiaSuoni(float volume)
    {
        if (sliderSuoni != null)
        {
            float volNormalizzato = volume / sliderSuoni.maxValue;
            Debug.Log("I SUONI (effetti) sono stati impostati a: " + volNormalizzato);
            
            // Qui collegheremo l'AudioMixer per gli effetti sonori!
        }
    }

    // --- FUNZIONE LUMINOSITÀ ---
    public void CambiaLuminosita(float luminosita)
    {
        if (filtroLuminosita != null && sliderLuminosita != null)
        {
            float luminositaNormalizzata = luminosita / sliderLuminosita.maxValue;
            Color coloreFiltro = filtroLuminosita.color;
            coloreFiltro.a = 1f - luminositaNormalizzata; 
            filtroLuminosita.color = coloreFiltro;
        }
    }
}