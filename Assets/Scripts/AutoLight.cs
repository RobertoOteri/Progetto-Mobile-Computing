using UnityEngine;
using UnityEngine.UI;

public class AutoLuminosita : MonoBehaviour
{
    [Header("Imposta lo stesso valore massimo del tuo Slider (es. 1 o 100)")]
    public float valoreMassimoSlider = 17;

    void Start()
    {
        // Prende in automatico il componente Image attaccato a questo oggetto
        Image filtro = GetComponent<Image>();

        if (filtro != null)
        {
            // Legge il valore salvato (se non lo trova, usa il valore massimo)
            float luminositaSalvata = PlayerPrefs.GetFloat("Luminosita", valoreMassimoSlider);

            // Calcola e applica l'opacità (stessa formula del tuo SettingsManager)
            float luminositaNormalizzata = luminositaSalvata / valoreMassimoSlider;
            Color coloreFiltro = filtro.color;
            coloreFiltro.a = 1f - luminositaNormalizzata; 
            filtro.color = coloreFiltro;
        }
    }
}