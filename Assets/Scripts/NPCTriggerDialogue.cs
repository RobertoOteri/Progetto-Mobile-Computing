using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    [Header("Configurazione Tipo")]
    public bool isBoss = false;

    [Header("Identificativo NPC")]
    public string npcID = "NPC_Alieno_1";

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
    private bool waitingForDialogueToEnd = false; 

    public bool HasHadFirstTalkSession
    {
        get
        {
            string key = isBoss ? "Boss_FirstTalkDone" : $"{npcID}_FirstTalkDone";
            return PlayerPrefs.GetInt(key, 0) == 1;
        }
        set
        {
            string key = isBoss ? "Boss_FirstTalkDone" : $"{npcID}_FirstTalkDone";
            PlayerPrefs.SetInt(key, value ? 1 : 0);
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsBossDefeated = !IsBossDefeated;

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
        }
        #endif

        if (isBoss && waitingForDialogueToEnd)
        {
            bool isDialogueActive = DialogueManager.Instance != null && 
                                    DialogueManager.Instance.dialoguePanel != null && 
                                    DialogueManager.Instance.dialoguePanel.activeSelf;

            if (!isDialogueActive)
            {
                waitingForDialogueToEnd = false;

                DemonBoss_Movement bossMovement = FindFirstObjectByType<DemonBoss_Movement>();
                if (bossMovement != null)
                {
                    bossMovement.EnableBossChase();
                
                }

                if (ArenaBarricade.Instance != null)
                {
                    ArenaBarricade.Instance.CloseBarricade();
                }
            }
        }

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

        if (IsBossDefeated)
        {
            if (bossDefeatedConversation.Count > 0)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
                DialogueManager.Instance.StartDialogueSequence(bossDefeatedConversation, true);
            }
            return;
        }

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

            if (isBoss)
            {
                waitingForDialogueToEnd = true;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.FadeOutMusic(1.0f); 
                    AudioManager.Instance.Invoke("PlayBossMusic", 1.0f); 
                }
            }
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