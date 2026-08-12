using UnityEngine;
using System.Collections;

public class LoopWithDelay : MonoBehaviour
{
    public AudioSource audioSource;
    public float delayBetweenLoops = 5f; // Tempo di attesa in secondi

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Avvia la Coroutine per il ciclo con ritardo
        StartCoroutine(PlayAudioLoop());
    }

    private IEnumerator PlayAudioLoop()
    {
        while (true)
        {
            // 1. Riproduce il suono
            audioSource.Play();

            // 2. Aspetta che il clip audio finisca completamente
            yield return new WaitForSeconds(audioSource.clip.length);

            // 3. Aspetta i 5 secondi di pausa prima di ripartire
            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }
}
