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
        // Aspetta una frazione di secondo (utile se c'è un fade-in all'avvio della scena)
        yield return new WaitForSeconds(delayBeforeStart);

        // Fa partire il dialogo tramite il DialogueManager
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines);
        }
    }
}