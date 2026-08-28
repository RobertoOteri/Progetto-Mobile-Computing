using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    [Header("Identificativo NPC")]
    [Tooltip("Nome unico per salvare questo dialogo (es. NPC_Alieno_1)")]
    public string npcID = "NPC_Alieno_1";

    public static bool HasHadFirstTalkSession
    {
        get => PlayerPrefs.GetInt("NPC_FirstTalkDone", 0) == 1;
        set
        {
            PlayerPrefs.SetInt("NPC_FirstTalkDone", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // Flag globale della sconfitta del Boss (salvata in PlayerPrefs)
    public static bool IsBossDefeated
    {
        get => PlayerPrefs.GetInt("BossDefeatedState", 0) == 1;
        set
        {
            PlayerPrefs.SetInt("BossDefeatedState", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    [Header("Riferimenti Zone & UI")]
    [Tooltip("L'oggetto figlio con il trigger LARGO per il primo dialogo")]
    public GameObject firstContactZone;
    
    [Tooltip("L'oggetto UI con il tasto [E]")]
    public GameObject interactPrompt;

    [Header("1. Primo Dialogo (Automatico - Trigger Largo)")]
    public List<DialogueLine> firstConversation = new List<DialogueLine>();

    [Header("2. Dialogo Ripetibile (Tasto E / Talk - Prima del Boss)")]
    public List<DialogueLine> repeatConversation = new List<DialogueLine>();

    [Header("3. Dialogo Finale (Tasto E / Talk - Dopo la sconfitta del Boss)")]
    public List<DialogueLine> bossDefeatedConversation = new List<DialogueLine>();

    private bool playerInRepeatZone = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (HasHadFirstTalkSession && firstContactZone != null)
        {
            firstContactZone.SetActive(false);
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR
        // TASTO DI TEST: Premi B per attivare/disattivare lo stato del Boss sconfitto
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsBossDefeated = !IsBossDefeated;
            Debug.Log($"<color=yellow>[TEST BOSS] Boss sconfitto impostato a: {IsBossDefeated}</color>");
        }
        #endif

        if (playerInRepeatZone && HasHadFirstTalkSession)
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel != null && DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
                return;
            }
            else
            {
                if (interactPrompt != null) interactPrompt.SetActive(true);
            }

            // Tasto E da tastiera su PC
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    // Metodo chiamato sia da tasto E che dal pulsante mobile TALK
    public void Interact()
    {
        if (!playerInRepeatZone) return;

        if (DialogueManager.Instance == null) return;

        // SE IL BOSS È STATO SCONFITTO -> Avvia il Dialogo Finale
        if (IsBossDefeated)
        {
            if (bossDefeatedConversation.Count > 0)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
                DialogueManager.Instance.StartDialogueSequence(bossDefeatedConversation);
            }
            return;
        }

        // SE IL BOSS È ANCORA VIVO -> Avvia il Dialogo Ripetibile normale
        if (HasHadFirstTalkSession && repeatConversation.Count > 0)
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            DialogueManager.Instance.StartDialogueSequence(repeatConversation);
        }
    }

    // Chiamato quando il player entra nella zona LARGA
    public void OnFirstContactTrigger()
    {
        if (HasHadFirstTalkSession) return;

        if (DialogueManager.Instance != null && firstConversation.Count > 0)
        {
            HasHadFirstTalkSession = true;
            DialogueManager.Instance.EnableHintOnNextDialogueEnd();
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