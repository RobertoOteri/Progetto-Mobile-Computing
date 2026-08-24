using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SettingsManager : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Slider sliderSound;
    public Slider sliderMusic;
    public Slider sliderLuminosita;
    public Image filtroLuminosita; 
    public TMP_Dropdown dropdownLanguage;

    // OnEnable viene chiamato OGNI VOLTA che il pannello viene attivato (.SetActive(true))
    void OnEnable()
    {
        CaricaImpostazioni();
    }

    void CaricaImpostazioni()
    {
        // Se c'è un valore salvato lo legge, altrimenti usa il valore massimo dello slider come default
        if (sliderMusic != null) 
        {
            sliderMusic.value = PlayerPrefs.GetFloat("VolumeMusica", sliderMusic.maxValue);
            CambiaMusica(sliderMusic.value); // Applica l'effetto
        }
        
        if (sliderSound != null) 
        {
            sliderSound.value = PlayerPrefs.GetFloat("VolumeSuoni", sliderSound.maxValue);
            CambiaSuoni(sliderSound.value); // Applica l'effetto
        }

        if (sliderLuminosita != null && filtroLuminosita != null) 
        {
            sliderLuminosita.value = PlayerPrefs.GetFloat("Luminosita", sliderLuminosita.maxValue);
            CambiaLuminosita(sliderLuminosita.value); // Applica l'effetto
        }
    }

    // --- AUDIO ---
    public void CambiaSuoni(float volume)
    {
        // 1. Salva il dato globalmente!
        PlayerPrefs.SetFloat("VolumeSuoni", volume); 
        PlayerPrefs.Save();

        // 2. Applica la logica
        if (sliderSound != null)
        {
            float volNormalizzato = volume / sliderSound.maxValue;
            Debug.Log("Suoni globali impostati a: " + volNormalizzato);
            // Qui andrà l'AudioMixer
        }
    }

    public void CambiaMusica(float volume)
    {
        // 1. Salva il dato globalmente!
        PlayerPrefs.SetFloat("VolumeMusica", volume);
        PlayerPrefs.Save();

        // 2. Applica la logica
        if (sliderMusic != null)
        {
            float volNormalizzato = volume / sliderMusic.maxValue;
            Debug.Log("Musica globale impostata a: " + volNormalizzato);
            // Qui andrà l'AudioMixer
        }
    }

    // --- GRAFICA ---
    public void CambiaLuminosita(float luminosita)
    {
        // 1. Salva il dato globalmente!
        PlayerPrefs.SetFloat("Luminosita", luminosita);
        PlayerPrefs.Save();

        // 2. Applica la logica
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
}