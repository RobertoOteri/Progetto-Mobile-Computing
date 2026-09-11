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

        StartCoroutine(PlayAudioLoop());
    }

    private IEnumerator PlayAudioLoop()
    {
        while (true)
        {
            // Riproduce il suono
            audioSource.Play();

            // Aspetta che il clip audio finisca completamente
            yield return new WaitForSeconds(audioSource.clip.length);

            // Aspetta i 5 secondi di pausa prima di ripartire
            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }
}
