using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    public static bool HasHadFirstTalkSession = false;

    [Header("Riferimenti Zone & UI")]
    [Tooltip("L'oggetto figlio con il trigger LARGO per il primo dialogo")]
    public GameObject firstContactZone;
    
    [Tooltip("L'oggetto UI con il tasto [E]")]
    public GameObject interactPrompt;

    [Header("1. Primo Dialogo (Automatico - Trigger Largo)")]
    public List<DialogueLine> firstConversation = new List<DialogueLine>();

    [Header("2. Dialogo Ripetibile (Tasto E - Trigger Stretto)")]
    public List<DialogueLine> repeatConversation = new List<DialogueLine>();

    //private bool hasHadFirstTalk = false;
    private bool playerInRepeatZone = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // Se il dialogo è già avvenuto in questa sessione, disattiva il trigger largo
        if (HasHadFirstTalkSession && firstContactZone != null)
        {
            firstContactZone.SetActive(false);
        }
    }
    private void Update()
    {
        if (playerInRepeatZone && HasHadFirstTalkSession)
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
                return;
            }
            else
            {
                if (interactPrompt != null) interactPrompt.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (DialogueManager.Instance != null && repeatConversation.Count > 0)
                {
                    if (interactPrompt != null) interactPrompt.SetActive(false);
                    DialogueManager.Instance.StartDialogueSequence(repeatConversation);
                }
            }
        }
    }

    // Chiamato quando il player entra nella zona LARGA
    public void OnFirstContactTrigger()
    {
        if (HasHadFirstTalkSession) return;

        if (DialogueManager.Instance != null && firstConversation.Count > 0)
        {
            HasHadFirstTalkSession = true;
            DialogueManager.Instance.StartDialogueSequence(firstConversation);

            if (firstContactZone != null)
                firstContactZone.SetActive(false);
        }
    }

    // Chiamato quando il player entra/esce dalla zona STRETTA
    public void SetPlayerInRepeatZone(bool inZone)
    {
        playerInRepeatZone = inZone;
        if (!inZone && interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }
}