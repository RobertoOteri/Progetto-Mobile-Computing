using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroSequence : MonoBehaviour
{
    [Header("Elementi UI")]
    public TextMeshProUGUI introText;
    public CanvasGroup textCanvasGroup;
    public Image backgroundImage;
    public CanvasGroup imageCanvasGroup;

    [Header("Contenuti Intro")]
    [TextArea(2, 5)]
    public string[] sentences;

    [Header("Impostazioni Macchina da Scrivere")]
    public float typingSpeed = 0.04f;      // Velocità con cui compaiono le lettere
    public float displayDuration = 3.0f;    // Tempo in cui la frase resta visibile dopo essere stata scritta
    public float fadeDuration = 0.8f;       // Tempo di dissolvenza prima della frase successiva
    
    [Header("Impostazioni Audio Sfumatura")]
    public float ambientFadeDuration = 1.5f; // Tempo di sfumatura dell'audio prima del cambio scena

    [Header("Scena Successiva")]
    public string nextSceneName = "Scena1";

    private bool isSkipping = false;
    private Coroutine currentRoutine;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayIntroAmbient(10f);
        }

        currentRoutine = StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        // Premere Spazio, Invio o Mouse per saltare direttamente al gioco
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && !isSkipping)
        {
            SkipIntro();
        }
    }

    private IEnumerator PlayIntro()
    {
        // Mostra lo sfondo e prepara il testo
        if (imageCanvasGroup != null) imageCanvasGroup.alpha = 1f;
        textCanvasGroup.alpha = 1f;
        introText.text = "";

        for (int i = 0; i < sentences.Length; i++)
        {
            textCanvasGroup.alpha = 1f;
            introText.text = "";

            // Effetto visivo lettera per lettera (senza suono)
            yield return StartCoroutine(TypeSentence(sentences[i]));

            // Pausa di lettura a fine frase
            yield return new WaitForSeconds(displayDuration);

            // Sfumatura in uscita (Fade Out) del testo prima della prossima frase
            yield return StartCoroutine(FadeCanvasGroup(textCanvasGroup, 1f, 0f, fadeDuration));
        }

        // Quando le frasi finiscono, fa sfumare il suono ambientale
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutIntroAmbient(ambientFadeDuration);
        }

        // Attende la conclusione della sfumatura audio
        yield return new WaitForSeconds(ambientFadeDuration);

        LoadGameScene();
    }

    private IEnumerator TypeSentence(string sentence)
    {
        int charCount = 0;

        foreach (char letter in sentence.ToCharArray())
        {
            introText.text += letter;

            charCount++;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    private void SkipIntro()
    {
        isSkipping = true;

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        // Fa sfumare rapidamente l'audio d'ambiente (es. 0.5 secondi) in caso di skip
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutIntroAmbient(0.5f);
        }

        Invoke(nameof(LoadGameScene), 0.5f);
    }

    private void LoadGameScene()
    {
        // Fai partire la BGM solo adesso che l'intro è finita
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.bgmMusic);
        }

        SceneManager.LoadScene(nextSceneName);
    }
}