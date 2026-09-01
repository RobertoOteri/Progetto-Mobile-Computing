using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("--- Audio Sources ---")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource introSource;

    [Header("--- Audio Clips ---")]
    public AudioClip walkSFX;
    public AudioClip dieSFX;
    public AudioClip pickupSFX;
    public AudioClip hurtSFX;
    public AudioClip gunShootSFX;
    public AudioClip rifleShootSFX;
    public AudioClip swordAttackSFX;
    public AudioClip hammerAttackSFX;
    public AudioClip bombExplodeSFX;
    public AudioClip typewriterSound;
    public AudioClip introAmbientSource;
    public AudioClip bgmMusic;
    public AudioClip bossMusicClip;

    [Header("--- Settings Camminata ---")]
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float walkMaxVolume = 2f;
    
    [Tooltip("Imposta es. 0.85 per rallentare. L'Audio Mixer correggerà la tonalità!")]
    [SerializeField] private float walkPitch = 0.85f;
    
    private bool isWalking = false;
    private Coroutine dieSoundCoroutine; 
    private Coroutine musicFadeCoroutine;

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

    // --- METODI PER IL MANAGEMENT DELLA MUSICA ---

    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }

    public void PlayMusic(AudioClip clip, float volume = 0.5f)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.isPlaying && musicSource.clip == clip) return;

        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void FadeOutMusic(float duration)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = StartCoroutine(FadeOutMusicRoutine(duration));
        }
    }

    private IEnumerator FadeOutMusicRoutine(float duration)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * (Time.deltaTime / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }

    // --- RIPRODUZIONE SFX ---

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            if (dieSoundCoroutine != null)
            {
                StopCoroutine(dieSoundCoroutine);
                dieSoundCoroutine = null;
            }

            sfxSource.pitch = 1f; 
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlaySFXWithVolume(AudioClip clip, float volume)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.pitch = 1f;
            sfxSource.PlayOneShot(clip, volume);
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
            PlaySFX(hurtSFX);
        }
    }
    
    public void PlayBombExplodeSound()
    {
        if (bombExplodeSFX != null)
        {
            PlaySFX(bombExplodeSFX);
        }
    }

    public void PlayTypewriterSound()
    {
        if (sfxSource != null && typewriterSound != null)
        {
            if (sfxSource.isPlaying && sfxSource.clip == typewriterSound)
                return;

            sfxSource.pitch = Random.Range(0.85f, 1.15f);
            sfxSource.clip = typewriterSound;
            sfxSource.PlayOneShot(typewriterSound, 24f);
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

    // --- INTRO AMBIENT (Aggiornato con introSource) ---

    public void PlayIntroAmbient(float volume = 1.5f)
    {
        if (introSource != null && introAmbientSource != null)
        {
            introSource.clip = introAmbientSource;
            introSource.loop = true; 
            introSource.volume = volume;
            introSource.Play();
        }
    }

    public void FadeOutIntroAmbient(float duration)
    {
        if (introSource != null && introSource.isPlaying)
        {
            StartCoroutine(FadeOutIntroRoutine(duration));
        }
    }

    private IEnumerator FadeOutIntroRoutine(float duration)
    {
        float startVolume = introSource.volume;

        while (introSource.volume > 0)
        {
            introSource.volume -= startVolume * (Time.deltaTime / duration);
            yield return null;
        }

        introSource.Stop();
        introSource.loop = false;
        introSource.clip = null;
        introSource.volume = startVolume;
    }

    public void StopIntroAmbient()
    {
        if (introSource != null && introSource.clip == introAmbientSource)
        {
            introSource.Stop();
            introSource.loop = false; 
            introSource.clip = null;
        }
    }

    public void StopAllSFX()
    {
        // Ferma la sorgente principale degli effetti sonori (colpi, spari, tasti, ecc.)
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }

        // Ferma e azzera il suono della camminata
        isWalking = false;
        if (walkSource != null)
        {
            walkSource.volume = 0f;
            walkSource.Stop();
        }
    }

    private IEnumerator PlaySFXWithSettings(AudioClip clip, float volume, float pitch)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);

        yield return new WaitForSeconds(clip.length / pitch);

        sfxSource.pitch = 1f;
        dieSoundCoroutine = null;
    }

    public void PlayBossMusic()
    {
        PlayMusic(bossMusicClip, 0.3f);
    }

    public void PlayRegularBGM()
    {
        PlayMusic(bgmMusic, 0.5f);
    }

    public void StopBossMusic(float duration = 1.5f)
    {
        FadeOutMusic(duration);
    }

    
}