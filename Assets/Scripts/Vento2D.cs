using UnityEngine;

public class Vento2D : MonoBehaviour
{

    public float velocita = 0.5f; 

    public float angoloMax = 0.5f; 

    private float rotazioneIniziale;

    void Start()
    {
        // Salva la rotazione originale così l'oggetto non si sposta
        rotazioneIniziale = transform.eulerAngles.z;
    }

    void Update()
    {
        // Calcola l'inclinazione minima
        float z = Mathf.Sin(Time.time * velocita) * angoloMax;
        
        // Applica solo la micro-rotazione senza muovere le coordinate X e Y
        transform.rotation = Quaternion.Euler(0, 0, rotazioneIniziale + z);
    }
}