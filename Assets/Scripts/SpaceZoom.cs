using UnityEngine;

public class SpaceZoom : MonoBehaviour
{
    [Header("Impostazioni Avanzamento")]
    public float zoomSpeed = 0.04f;   // Velocità con cui la navicella sembra avanzare
    public float maxScale = 1.5f;     // Quanto può ingrandirsi al massimo l'immagine
    public bool loopZoom = true;      // Se vero, resetta lo zoom quando arriva al massimo

    private Vector3 initialScale;

    private void Start()
    {
        // Salva la dimensione originale della foto
        initialScale = transform.localScale;
    }

    private void Update()
    {
        // Ingrandisce gradualmente la foto in tutte le direzioni (effetto Zoom)
        transform.localScale += Vector3.one * zoomSpeed * Time.deltaTime;

        // Se attivo il loop, quando l'immagine si è espansa troppo torna alla dimensione iniziale
        if (loopZoom && transform.localScale.x >= maxScale)
        {
            transform.localScale = initialScale;
        }
    }
}