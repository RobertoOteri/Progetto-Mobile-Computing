using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio; 

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer mainMixer; 
    [Header("Navigazione Pannelli")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;
    
    [Header("Impostazioni Grafiche e Audio")]
    public Image filtroLuminosita; 
    public Slider sliderMusica;  
    public Slider sliderSuoni;   
    public Slider sliderLuminosita; 

    [Header("Audio Menu")]
    public AudioClip menuBGM;
    public AudioClip buttonClickSFX;  

    void Start()
    {
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false);

        // Inizializza tutti e tre gli slider all'avvio
        if (sliderMusica != null) CambiaMusica(sliderMusica.value);
        if (sliderSuoni != null) CambiaSuoni(sliderSuoni.value);
        if (sliderLuminosita != null) CambiaLuminosita(sliderLuminosita.value);

        if (AudioManager.Instance != null && menuBGM != null)
        {
            AudioManager.Instance.PlayMusic(menuBGM, 0.5f);
        }
    }

    public void PlayButtonSound()
    {
        if (AudioManager.Instance != null && buttonClickSFX != null)
        {
            AudioManager.Instance.PlaySFXWithVolume(buttonClickSFX, 1f);
        }
    }

    // --- FUNZIONI NAVIGAZIONE MENU ---
    public void Gioca() 
    {   
    // Salva le impostazioni correnti prima del reset
        float musicVol = PlayerPrefs.GetFloat("VolumeMusica", sliderMusica != null ? sliderMusica.value : 1f);
        float soundVol = PlayerPrefs.GetFloat("VolumeSuoni", sliderSuoni != null ? sliderSuoni.value : 1f);
        float lum = PlayerPrefs.GetFloat("Luminosita", sliderLuminosita != null ? sliderLuminosita.value : 1f);

        // Cancella i salvataggi di gioco
        PlayerPrefs.DeleteAll();

        // Ripristina solo le impostazioni
        PlayerPrefs.SetFloat("VolumeMusica", musicVol);
        PlayerPrefs.SetFloat("VolumeSuoni", soundVol);
        PlayerPrefs.SetFloat("Luminosita", lum);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        SceneManager.LoadScene("Intro");
    }
    
    public void Esci() {
        PlayButtonSound(); 
        Application.Quit(); 
    }

    public void ApriImpostazioni() {
        PlayButtonSound(); 
        pannelloPrincipale.SetActive(false); 
        pannelloImpostazioni.SetActive(true); 
    }

    public void ChiudiImpostazioni() {
        PlayButtonSound(); 
        pannelloImpostazioni.SetActive(false); 
        pannelloPrincipale.SetActive(true); 
    }

   // --- FUNZIONI DEGLI SLIDER AUDIO ---
    
    public void CambiaMusica(float volume)
    {
        if (sliderMusica != null && mainMixer != null)
        {
            float volNormalizzato = volume / sliderMusica.maxValue;
            // Convertiamo la scala 0..1 in Decibel (-80dB a 0dB)
            float dB = (volNormalizzato > 0.0001f) ? Mathf.Log10(volNormalizzato) * 20f : -80f;

            mainMixer.SetFloat("MusicVol", dB);

            PlayerPrefs.SetFloat("VolumeMusica", volume);
            PlayerPrefs.Save();
        }
    }

    public void CambiaSuoni(float volume)
    {
        if (sliderSuoni != null && mainMixer != null)
        {
            float volNormalizzato = volume / sliderSuoni.maxValue;
            // Convertiamo la scala 0..1 in Decibel (-80dB a 0dB)
            float dB = (volNormalizzato > 0.0001f) ? Mathf.Log10(volNormalizzato) * 20f : -80f;

            mainMixer.SetFloat("SFXVol", dB);

            PlayerPrefs.SetFloat("VolumeSuoni", volume);
            PlayerPrefs.Save();
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

            PlayerPrefs.SetFloat("Luminosita", luminosita);
            PlayerPrefs.Save();
        }
    }
}