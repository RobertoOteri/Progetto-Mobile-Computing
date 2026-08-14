using UnityEngine;

public class SpaceshipInteract : MonoBehaviour
{
    [Header("UI Guida")]
    public GameObject interactPrompt;

    [Header("Impostazioni Dialogo")]
    public string speakerName = "Jack Orbit";
    public Sprite portrait;

    [TextArea(3, 5)]
    public string[] dialogueLines = new string[]
    {
        "È stato un atterraggio decisamente più duro del previsto...",
        "Il propulsore secondario è completamente andato. Se non trovo una fonte di energia non andrò da nessuna parte.",
        "Sarà meglio esplorare la zona e cercare qualcosa di utile."
    };

    private bool playerInRange = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        // Se il giocatore è nell'area del collider
        if (playerInRange)
        {
            // Se il dialogo è attualmente aperto a schermo, nascondi il tasto [E]
            if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null && DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                if (interactPrompt != null)
                    interactPrompt.SetActive(false);
                return;
            }
            else
            {
                if (interactPrompt != null)
                    interactPrompt.SetActive(true);
            }

            // Pressione del tasto E
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (DialogueManager.Instance != null)
                {
                    if (interactPrompt != null)
                        interactPrompt.SetActive(false);

                    // Avvia il dialogo specificando isRightSide = false (Jack a sinistra)
                    DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines, false);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
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
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}