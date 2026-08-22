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
    public Slider sliderVolume;       
    public Slider sliderLuminosita;   

    void Start()
    {
        // Impostiamo lo stato iniziale dei pannelli
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false);

        // --- INIZIALIZZAZIONE AUDIO E LUMINOSITÀ ---
        
        // Applica subito il volume leggendo il valore di partenza dello slider 
        if (sliderVolume != null)
        {
            CambiaVolume(sliderVolume.value);
        }

        // Applica subito la luminosità leggendo il valore di partenza dello slider 
        if (sliderLuminosita != null)
        {
            CambiaLuminosita(sliderLuminosita.value);
        }
    }

    // --- FUNZIONI NAVIGAZIONE MENU ---
    public void Gioca() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    public void Esci() { Application.Quit(); }
    public void ApriImpostazioni() { pannelloPrincipale.SetActive(false); pannelloImpostazioni.SetActive(true); }
    public void ChiudiImpostazioni() { pannelloImpostazioni.SetActive(false); pannelloPrincipale.SetActive(true); }

    // --- FUNZIONI DEGLI SLIDER ---
    
    public void CambiaVolume(float volume)
    {
        // Il volume in ingresso ora è un numero da 0 a 10 (o i blocchi massimi che hai impostato).
        // Lo dividiamo per il valore massimo dello slider per ottenere un numero tra 0.0 e 1.0.
        if (sliderVolume != null)
        {
            float volumeNormalizzato = volume / sliderVolume.maxValue;
            AudioListener.volume = volumeNormalizzato;
        }
    }

    public void CambiaLuminosita(float luminosita)
    {
        if (filtroLuminosita != null && sliderLuminosita != null)
        {
            // Stessa cosa: normalizziamo il valore da 0-10 a 0.0-1.0
            float luminositaNormalizzata = luminosita / sliderLuminosita.maxValue;
            
            Color coloreFiltro = filtroLuminosita.color;
            coloreFiltro.a = 1f - luminositaNormalizzata; 
            filtroLuminosita.color = coloreFiltro;
        }
    }
}