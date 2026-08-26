using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; 
using TMPro; 

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer mainMixer; 

    [Header("Riferimenti UI")]
    public Slider sliderSound;
    public Slider sliderMusic;
    public Slider sliderLuminosita;
    public Image filtroLuminosita; 
    public TMP_Dropdown dropdownLanguage;

    [Header("Gestione Visibilità Impostazioni")]
    public CanvasGroup mainSettingsCanvasGroup; 
    public GameObject settingsTitle; // <--- Aggiunta variabile per il titolo

    [Header("Pannelli Info Pop-up")]
    public GameObject ratePanel;
    public GameObject aboutPanel;
    public GameObject supportPanel;

    void OnEnable()
    {
        CaricaImpostazioni();
        CloseAllInfoPanels();
    }

    void CaricaImpostazioni()
    {
        if (sliderMusic != null) 
        {
            sliderMusic.value = PlayerPrefs.GetFloat("VolumeMusica", sliderMusic.maxValue);
            CambiaMusica(sliderMusic.value);
        }
        
        if (sliderSound != null) 
        {
            sliderSound.value = PlayerPrefs.GetFloat("VolumeSuoni", sliderSound.maxValue);
            CambiaSuoni(sliderSound.value);
        }

        if (sliderLuminosita != null) 
        {
            sliderLuminosita.value = PlayerPrefs.GetFloat("Luminosita", sliderLuminosita.maxValue);
            CambiaLuminosita(sliderLuminosita.value);
        }
    }

    // --- AUDIO ---
    public void CambiaSuoni(float volume)
    {
        PlayerPrefs.SetFloat("VolumeSuoni", volume); 
        PlayerPrefs.Save();

        if (sliderSound != null && mainMixer != null)
        {
            float volNormalizzato = volume / sliderSound.maxValue;
            float dB = (volNormalizzato > 0.0001f) ? Mathf.Log10(volNormalizzato) * 20f : -80f;
            
            mainMixer.SetFloat("SFXVol", dB);
        }
    }

    public void CambiaMusica(float volume)
    {
        PlayerPrefs.SetFloat("VolumeMusica", volume);
        PlayerPrefs.Save();

        if (sliderMusic != null && mainMixer != null)
        {
            float volNormalizzato = volume / sliderMusic.maxValue;
            float dB = (volNormalizzato > 0.0001f) ? Mathf.Log10(volNormalizzato) * 20f : -80f;

            mainMixer.SetFloat("MusicVol", dB);
        }
    }

    // --- GRAFICA ---
    public void CambiaLuminosita(float luminosita)
    {
        PlayerPrefs.SetFloat("Luminosita", luminosita);
        PlayerPrefs.Save();

        if (filtroLuminosita == null)
        {
            AutoLuminosita autoFiltro = FindObjectOfType<AutoLuminosita>();
            if (autoFiltro != null)
            {
                filtroLuminosita = autoFiltro.GetComponent<Image>();
            }
        }

        if (filtroLuminosita != null && sliderLuminosita != null)
        {
            float luminositaNormalizzata = luminosita / sliderLuminosita.maxValue;
            Color coloreFiltro = filtroLuminosita.color;
            coloreFiltro.a = 1f - luminositaNormalizzata; 
            filtroLuminosita.color = coloreFiltro;
        }
    }

    // --- ALTRE IMPOSTAZIONI ---
    public void CambiaLingua(int indiceLingua) 
    { 
        PlayerPrefs.SetInt("Lingua", indiceLingua); 
        PlayerPrefs.Save();
        Debug.Log("Lingua globale: " + indiceLingua); 
    }

    // --- GESTIONE POP-UP (APERTURA) ---
    public void OpenRate()
    {
        HideAllPopups();
        SetMainSettingsVisible(false);
        if (ratePanel != null) ratePanel.SetActive(true);
    }

    public void OpenAbout()
    {
        HideAllPopups();
        SetMainSettingsVisible(false);
        if (aboutPanel != null) aboutPanel.SetActive(true);
    }

    public void OpenSupport()
    {
        HideAllPopups();
        SetMainSettingsVisible(false);
        if (supportPanel != null) supportPanel.SetActive(true);
    }

    // --- GESTIONE POP-UP (CHIUSURA VIA TASTO X) ---
    public void CloseAllInfoPanels()
    {
        HideAllPopups();
        SetMainSettingsVisible(true);
    }

    private void HideAllPopups()
    {
        if (ratePanel != null) ratePanel.SetActive(false);
        if (aboutPanel != null) aboutPanel.SetActive(false);
        if (supportPanel != null) supportPanel.SetActive(false);
    }

    private void SetMainSettingsVisible(bool visible)
    {
        if (mainSettingsCanvasGroup != null)
        {
            mainSettingsCanvasGroup.alpha = visible ? 1f : 0f;
            mainSettingsCanvasGroup.interactable = visible;
            mainSettingsCanvasGroup.blocksRaycasts = visible;
        }

        // Mostra o nasconde il titolo insieme al CanvasGroup
        if (settingsTitle != null)
        {
            settingsTitle.SetActive(visible);
        }
    }
}