using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    [Header("Identificativo NPC")]
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
    public GameObject firstContactZone;
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
        // B per invertire lo stato del boss
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsBossDefeated = !IsBossDefeated;
            Debug.Log($"<color=yellow>[TEST BOSS] Boss sconfitto impostato a: {IsBossDefeated}</color>");
        }

        // C per cancellare tutti i salvataggi PlayerPrefs e ripartire da zero
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=cyan>[RESET] PlayerPrefs resettati! Partita nuova.</color>");
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }

    public void Interact()
    {
        if (!playerInRepeatZone || DialogueManager.Instance == null) return;

        // Se il boss è sconfitto -> avvia dialogo con flag di fine gioco = true
        if (IsBossDefeated)
        {
            if (bossDefeatedConversation.Count > 0)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
                DialogueManager.Instance.StartDialogueSequence(bossDefeatedConversation, true);
            }
            return;
        }

        // Se il boss è vivo -> avvia dialogo ripetibile normale
        if (HasHadFirstTalkSession && repeatConversation.Count > 0)
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            DialogueManager.Instance.StartDialogueSequence(repeatConversation, false);
        }
    }

    public void OnFirstContactTrigger()
    {
        if (HasHadFirstTalkSession) return;

        if (DialogueManager.Instance != null && firstConversation.Count > 0)
        {
            HasHadFirstTalkSession = true;
            DialogueManager.Instance.EnableHintOnNextDialogueEnd();
            DialogueManager.Instance.StartDialogueSequence(firstConversation, false);

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