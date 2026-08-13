using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    [Header("Impostazioni Dialogo NPC")]
    public string speakerName = "Mister Catto";
    public Sprite portrait;
    public bool portraitOnRight = true; // <--- Spunta per mettere il ritratto a destra!

    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Opzioni")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;

            if (DialogueManager.Instance != null)
            {
                hasTriggered = true;
                // Passa anche la preferenza sulla posizione del ritratto
                DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines, portraitOnRight);
            }
        }
    }

    public void ResetDialogueTrigger()
    {
        hasTriggered = false;
    }
}