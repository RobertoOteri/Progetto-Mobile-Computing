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

    [Header("--- Settings Camminata ---")]
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float walkMaxVolume = 0.6f; // Volume leggermente più basso
    
    [Tooltip("Imposta es. 0.85 per rallentare. L'Audio Mixer correggerà la tonalità!")]
    [SerializeField] private float walkPitch = 0.85f; // Un po' più lento
    
    private bool isWalking = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 0) sfxSource = sources[0];
            if (sources.Length > 1) walkSource = sources[1];
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

            // --- Riavvolge l'audio prima del silenzio finale ---
            // Se la traccia è in riproduzione e ha superato 4.1 secondi, riparti da 0
            if (isWalking && walkSource.isPlaying && walkSource.time >= 3.9f)
            {
                walkSource.time = 0f; // Salta il silenzio finale e riparte subito
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StartWalkSound()
    {
        isWalking = true;

        if (walkSFX != null && walkSource != null)
        {
            walkSource.pitch = walkPitch; // Regola la velocità

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
}