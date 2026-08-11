using UnityEngine;

public class SpaceshipInteract : MonoBehaviour
{
    [Header("Impostazioni Dialogo")]
    public string speakerName = "Jack Orbit";
    public Sprite portrait;

    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("UI Guida (Opzionale)")]
    public GameObject interactPrompt;

    private bool playerInRange = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        // Controlla se il pannello dei dialoghi è già aperto per evitare di riattivare la conversazione
        bool isDialogueActive = DialogueManager.Instance != null && 
                                DialogueManager.Instance.dialoguePanel != null && 
                                DialogueManager.Instance.dialoguePanel.activeSelf;

        // Gestione della visibilità del prompt "Premi E"
        if (interactPrompt != null)
        {
            // Mostra l'icona SOLO se il player è vicino E il dialogo NON è in corso
            interactPrompt.SetActive(playerInRange && !isDialogueActive);
        }

        // Se il giocatore è vicino, premi 'E' e il dialogo non è già aperto -> Avvia il dialogo
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}