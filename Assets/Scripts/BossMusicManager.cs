using System.Collections;
using UnityEngine;

public class BossMusicManager : MonoBehaviour
{
    public static BossMusicManager Instance;

    [Header("Audio Settings")]
    public AudioSource musicAudioSource;
    public AudioClip bossMusic;
    [Range(0.1f, 5f)] public float fadeDuration = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Avvia la musica (chiamalo quando finisce il dialogo del boss)
    public void PlayBossMusic()
    {
        if (musicAudioSource != null && bossMusic != null)
        {
            musicAudioSource.clip = bossMusic;
            musicAudioSource.volume = 0f;
            musicAudioSource.Play();
            StartCoroutine(FadeVolume(0f, 0.3f, fadeDuration, false)); // Modifica 0.3f con il volume desiderato
        }
    }

    // Ferma la musica con effetto scala (chiamalo quando il boss muore)
    public void StopBossMusicWithFade()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            StartCoroutine(FadeVolume(musicAudioSource.volume, 0f, fadeDuration, true));
        }
    }

    private IEnumerator FadeVolume(float startVol, float targetVol, float duration, bool stopAtEnd)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVol, targetVol, elapsed / duration);
            yield return null;
        }
        musicAudioSource.volume = targetVol;

        if (stopAtEnd)
        {
            musicAudioSource.Stop();
        }
    }
}