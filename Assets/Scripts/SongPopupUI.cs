using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SongPopupUI : MonoBehaviour
{
    public static SongPopupUI Instance;

    [Header("Componenti UI")]
    public CanvasGroup canvasGroup;
    public Image popupImage;

    [Header("Tempi")]
    public float fadeInTime = 0.5f;
    public float displayDuration = 3.5f;
    public float fadeOutTime = 0.8f;

    private Coroutine displayRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Se non assegnato a mano, prendi il CanvasGroup da questo stesso oggetto
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (popupImage == null)
            popupImage = GetComponent<Image>();

        // Nasconde l'immagine all'avvio impostando l'alpha a 0 (senza disattivare il GameObject)
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowSongPopup()
    {
        Debug.Log("<color=magenta>[POPUP] Avvio visualizzazione canzone...</color>");

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        if (displayRoutine != null)
        {
            StopCoroutine(displayRoutine);
        }

        displayRoutine = StartCoroutine(DoFadeRoutine());
    }

    private IEnumerator DoFadeRoutine()
    {
        // 1. Fade In
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 2. Rimane visibile a schermo
        yield return new WaitForSeconds(displayDuration);

        // 3. Fade Out
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        displayRoutine = null;
    }
}