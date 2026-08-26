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
    public GameObject pannelloSelezionePartita;
    public GameObject pannelloImpostazioni;
    public GameObject pannelloConfermaNuovaPartita; // Il nuovo pop-up di avviso
    
    [Header("Pulsanti Partita")]
    public Button bottoneContinua;
    public string scenaNuovaPartita = "Intro";

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
        if (pannelloSelezionePartita != null) pannelloSelezionePartita.SetActive(false);
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false);
        if (pannelloConfermaNuovaPartita != null) pannelloConfermaNuovaPartita.SetActive(false);

        // Slider
        if (sliderMusica != null) sliderMusica.value = PlayerPrefs.GetFloat("VolumeMusica", sliderMusica.value);
        if (sliderSuoni != null) sliderSuoni.value = PlayerPrefs.GetFloat("VolumeSuoni", sliderSuoni.value);
        if (sliderLuminosita != null) sliderLuminosita.value = PlayerPrefs.GetFloat("Luminosita", sliderLuminosita.value);

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

    // --- NAVIGAZIONE SOTTOMENU PARTITA ---

    public void ApriSelezionePartita()
    {
        PlayButtonSound();
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(false);
        if (pannelloSelezionePartita != null) pannelloSelezionePartita.SetActive(true);

        if (bottoneContinua != null)
        {
            bool hasSave = SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile();
            bottoneContinua.interactable = hasSave;
        }
    }

    public void ChiudiSelezionePartita()
    {
        PlayButtonSound();
        if (pannelloSelezionePartita != null) pannelloSelezionePartita.SetActive(false);
        if (pannelloConfermaNuovaPartita != null) pannelloConfermaNuovaPartita.SetActive(false);
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true);
    }

    // --- LOGICA NUOVA PARTITA CON POPUP DI CONFERMA ---

    // Chiamato dal tasto "Nuova Partita"
    public void ClickNuovaPartita()
    {
        PlayButtonSound();

        // Se esiste già un salvataggio, mostriamo il pop-up di avviso
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile())
        {
            if (pannelloConfermaNuovaPartita != null)
            {
                pannelloConfermaNuovaPartita.SetActive(true);
            }
        }
        else
        {
            // Se non c'è nessun salvataggio, avvia subito
            EseguiNuovaPartita();
        }
    }

    // Chiamato dal tasto "SÌ / CONFERMA" del pop-up
    public void EseguiNuovaPartita()
    {
        PlayButtonSound();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.NewGame(scenaNuovaPartita);
        }
        else
        {
            SceneManager.LoadScene(scenaNuovaPartita);
        }
    }

    // Chiamato dal tasto "NO / ANNULLA" del pop-up
    public void AnnullaNuovaPartita()
    {
        PlayButtonSound();
        if (pannelloConfermaNuovaPartita != null)
        {
            pannelloConfermaNuovaPartita.SetActive(false);
        }
    }

    // --- CONTINUA ---

    public void Continua()
    {
        PlayButtonSound();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.ContinueGame();
        }
    }

    public void Esci() 
    { 
        PlayButtonSound(); 
        Application.Quit(); 
    }

    public void ApriImpostazioni() 
    { 
        PlayButtonSound(); 
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(false); 
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(true); 
    }

    public void ChiudiImpostazioni() 
    { 
        PlayButtonSound(); 
        if (pannelloImpostazioni != null) pannelloImpostazioni.SetActive(false); 
        if (pannelloPrincipale != null) pannelloPrincipale.SetActive(true); 
    }

    public void CambiaMusica(float volume)
    {
        if (sliderMusica != null && mainMixer != null)
        {
            float volNormalizzato = volume / sliderMusica.maxValue;
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
            float dB = (volNormalizzato > 0.0001f) ? Mathf.Log10(volNormalizzato) * 20f : -80f;
            mainMixer.SetFloat("SFXVol", dB);
            PlayerPrefs.SetFloat("VolumeSuoni", volume);
            PlayerPrefs.Save();
        }
    }

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