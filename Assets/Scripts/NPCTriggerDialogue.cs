using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    [Header("Riferimenti Zone & UI")]
    [Tooltip("L'oggetto figlio con il trigger LARGO per il primo dialogo")]
    public GameObject firstContactZone;
    
    [Tooltip("L'oggetto UI con il tasto [E]")]
    public GameObject interactPrompt;

    [Header("1. Primo Dialogo (Automatico - Trigger Largo)")]
    public List<DialogueLine> firstConversation = new List<DialogueLine>();

    [Header("2. Dialogo Ripetibile (Tasto E - Trigger Stretto)")]
    public List<DialogueLine> repeatConversation = new List<DialogueLine>();

    private bool hasHadFirstTalk = false;
    private bool playerInRepeatZone = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerInRepeatZone && hasHadFirstTalk)
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel.activeSelf)
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (DialogueManager.Instance != null && repeatConversation.Count > 0)
                {
                    if (interactPrompt != null)
                        interactPrompt.SetActive(false);

                    DialogueManager.Instance.StartDialogueSequence(repeatConversation);
                }
            }
        }
    }

    public void OnFirstContactTrigger()
    {
        if (hasHadFirstTalk) return;

        if (DialogueManager.Instance != null && firstConversation.Count > 0)
        {
            hasHadFirstTalk = true;

            // Abilita l'hint solo per questo primo dialogo
            DialogueManager.Instance.EnableHintOnNextDialogueEnd();

            DialogueManager.Instance.StartDialogueSequence(firstConversation);

            if (firstContactZone != null)
                firstContactZone.SetActive(false);
        }
    }

    public void SetPlayerInRepeatZone(bool inZone)
    {
        playerInRepeatZone = inZone;

        if (!inZone && interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }
}