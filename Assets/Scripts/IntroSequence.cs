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
    
    [Header("Scena Successiva")]
    public string nextSceneName = "Scena1";

    private bool isSkipping = false;
    private Coroutine currentRoutine;

    private void Start()
    {
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

            // Effetto Macchina da Scrivere (scrive lettera per lettera)
            yield return StartCoroutine(TypeSentence(sentences[i]));

            // Pausa di lettura a fine frase
            yield return new WaitForSeconds(displayDuration);

            // Sfumatura in uscita (Fade Out) del testo prima della prossima frase
            yield return StartCoroutine(FadeCanvasGroup(textCanvasGroup, 1f, 0f, fadeDuration));
        }

        // Carica la scena di gioco principale
        LoadGameScene();
    }

    private IEnumerator TypeSentence(string sentence)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            introText.text += letter;
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
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}