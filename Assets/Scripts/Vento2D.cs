using UnityEngine;

public class Vento2D : MonoBehaviour
{

    public float velocita = 0.5f; 

    public float angoloMax = 0.5f; 

    private float rotazioneIniziale;

    void Start()
    {
        rotazioneIniziale = transform.eulerAngles.z;
    }

    void Update()
    {
        float z = Mathf.Sin(Time.time * velocita) * angoloMax;
        
        transform.rotation = Quaternion.Euler(0, 0, rotazioneIniziale + z);
    }
}