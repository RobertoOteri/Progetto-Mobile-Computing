using System.Collections;
using UnityEngine;

public class AutoDialogue : MonoBehaviour
{
    [Header("Impostazioni Diario")]
    public string speakerName = "Diario di Bordo";
    public Sprite portrait;

    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Ritardo Iniziale")]
    public float delayBeforeStart = 0.5f; // Mezzo secondo di attesa prima che appaia il testo

    private IEnumerator Start()
    {
        // 🔴 CONTROLLO SICUREZZA: Se il gioco è già stato avviato o l'intro è completata, annulla il dialogo
        if (PlayerPrefs.GetInt("GameIntroCompleted", 0) == 1 || NPCTriggerDialogue.HasHadFirstTalkSession)
        {
            gameObject.SetActive(false);
            yield break; // Interrompe subito la Coroutine
        }

        // Aspetta una frazione di secondo (utile se c'è un fade-in all'avvio della scena)
        yield return new WaitForSeconds(delayBeforeStart);

        // Fa partire il dialogo tramite il DialogueManager
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines);
        }
    }
}