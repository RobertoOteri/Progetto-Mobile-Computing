using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("--- Audio Sources ---")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource walkSource;

    [Header("--- Audio Clips ---")]
    public AudioClip walkSFX;
    public AudioClip dieSFX;
    public AudioClip pickupSFX;
    public AudioClip hurtSFX;
    public AudioClip gunShootSFX;
    public AudioClip rifleShootSFX;
    public AudioClip swordAttackSFX;
    public AudioClip hammerAttackSFX;
    public AudioClip typewriterSound;
    public AudioClip introAmbientSource;

    [Header("--- Settings Camminata ---")]
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float walkMaxVolume = 0.6f;
    
    [Tooltip("Imposta es. 0.85 per rallentare. L'Audio Mixer correggerà la tonalità!")]
    [SerializeField] private float walkPitch = 0.85f;
    
    private bool isWalking = false;
    private Coroutine dieSoundCoroutine; // Per tracciare la coroutine del suono di morte

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (walkSource != null)
        {
            float targetVolume = isWalking ? walkMaxVolume : 0f;
            walkSource.volume = Mathf.MoveTowards(walkSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

            if (walkSource.volume == 0f && walkSource.isPlaying)
            {
                walkSource.Stop();
            }

            if (isWalking && walkSource.isPlaying && walkSource.time >= 3.9f)
            {
                walkSource.time = 0f;
            }
        }
    }

    // 🟢 Riproduzione SFX normali: garantisce sempre Pitch = 1!
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            // Se c'era una coroutine del suono di morte attiva, fermala per evitare che resetti il pitch dopo
            if (dieSoundCoroutine != null)
            {
                StopCoroutine(dieSoundCoroutine);
                dieSoundCoroutine = null;
            }

            sfxSource.pitch = 1f; // 👈 Forza la velocità e la tonalità originale (44.1kHz / 48kHz standard)
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StartWalkSound()
    {
        isWalking = true;

        if (walkSFX != null && walkSource != null)
        {
            walkSource.pitch = walkPitch;

            if (!walkSource.isPlaying)
            {
                walkSource.clip = walkSFX;
                walkSource.loop = true;
                walkSource.volume = 0f;
                walkSource.Play();
            }
        }
    }

    public void StopWalkSound()
    {
        isWalking = false;
    }

    public void PlayDieSound()
    {
        if (dieSFX != null && sfxSource != null)
        {
            if (dieSoundCoroutine != null)
            {
                StopCoroutine(dieSoundCoroutine);
            }
            dieSoundCoroutine = StartCoroutine(PlaySFXWithSettings(dieSFX, 0.2f, 0.8f));
        }
    }

    public void PlayHurtSound()
    {
        if (hurtSFX != null && sfxSource != null)
        {
            PlaySFX(hurtSFX); // Riproduce il suono dell'impatto a volume/pitch standard
        }
    }

    public void PlayTypewriterSound()
    {
        if (sfxSource != null && typewriterSound != null)
        {
            // Se sta già riproducendo l'effetto audio, evita di sovrapporne un altro
            if (sfxSource.isPlaying && sfxSource.clip == typewriterSound)
                return;

            sfxSource.pitch = Random.Range(0.85f, 1.15f);
            sfxSource.clip = typewriterSound;
            sfxSource.PlayOneShot(typewriterSound, 2f);
        }
    }

    public void StopTypewriterSound()
    {
        if (sfxSource != null)
        {
            if (sfxSource.clip == typewriterSound)
            {
                sfxSource.Stop();
            }
            sfxSource.pitch = 1f;
        }
    }

    // Fa iniziare il suono ambientale dell'intro
    public void PlayIntroAmbient()
    {
        if (sfxSource != null && introAmbientSource != null)
        {
            sfxSource.clip = introAmbientSource;
            sfxSource.loop = true; 
            sfxSource.Play();
        }
    }
    public void FadeOutIntroAmbient(float duration)
    {
        if (sfxSource != null && sfxSource.isPlaying)
        {
            StartCoroutine(FadeOutRoutine(duration));
        }
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = sfxSource.volume;

        while (sfxSource.volume > 0)
        {
            // Riduce il volume in base al tempo
            sfxSource.volume -= startVolume * (Time.deltaTime / duration);
            yield return null;
        }

        // Quando il volume arriva a 0, ferma l'audio e ripristina i parametri
        sfxSource.Stop();
        sfxSource.loop = false;
        sfxSource.clip = null;
        sfxSource.volume = startVolume; // Ripristina il volume per i suoni futuri
    }

    // Ferma il suono ambientale dell'intro
    public void StopIntroAmbient()
    {
        if (sfxSource != null && sfxSource.clip == introAmbientSource)
        {
            sfxSource.Stop();
            sfxSource.loop = false; 
            sfxSource.clip = null;
        }
    }

    private System.Collections.IEnumerator PlaySFXWithSettings(AudioClip clip, float volume, float pitch)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);

        yield return new WaitForSeconds(clip.length / pitch);

        sfxSource.pitch = 1f; // Ripristina il pitch standard a 1
        dieSoundCoroutine = null;
    }
    public void PlaySFXWithVolume(AudioClip clip, float volume)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.pitch = 1f; // Mantiene la tonalità corretta
            sfxSource.PlayOneShot(clip, volume); // Riproduce la clip con il volume specifico passatogli
        }
    }
}