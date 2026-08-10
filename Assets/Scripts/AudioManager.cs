using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("--- Audio Sources ---")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource walkSource;

    [Header("--- Audio Clips ---")]
    public AudioClip walkSFX;
    public AudioClip attackSFX;
    public AudioClip dieSFX;
    public AudioClip pickupSFX;

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

    private System.Collections.IEnumerator PlaySFXWithSettings(AudioClip clip, float volume, float pitch)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);

        yield return new WaitForSeconds(clip.length / pitch);

        sfxSource.pitch = 1f; // Ripristina il pitch standard a 1
        dieSoundCoroutine = null;
    }
}