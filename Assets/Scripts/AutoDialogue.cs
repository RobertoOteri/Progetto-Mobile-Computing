using System.Collections;
using UnityEngine;

public class AutoDialogue : MonoBehaviour
{
    [Header("Impostazioni Diario")]
    public string speakerName = "Jack Orbit";
    public Sprite portrait;

    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Ritardo Iniziale")]
    public float delayBeforeStart = 0.5f;

    [Header("Chiave Salvataggio")]
    public string saveKey = "GameIntroCompleted";

    private IEnumerator Start()
    {
        // 1. Se stiamo caricando una partita salvata con "Continua", disattiva subito
        if (SaveSystem.Instance != null && SaveSystem.Instance.IsContinuingGame())
        {
            gameObject.SetActive(false);
            yield break;
        }

        // 2. Se è già stato completato in precedenza, disattiva
        if (PlayerPrefs.GetInt(saveKey, 0) == 1)
        {
            gameObject.SetActive(false);
            yield break;
        }

        // 3. Registra immediatamente il completamento su disco per non ripeterlo
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        // Attesa iniziale
        yield return new WaitForSeconds(delayBeforeStart);

        // Avvio sequenza di dialogo
        if (DialogueManager.Instance != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines);
        }
    }
}