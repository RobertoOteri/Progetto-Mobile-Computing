using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerDialogue : MonoBehaviour
{
    [Header("Configurazione Tipo")]
    [Tooltip("Spunta se questo script è attaccato al Boss")]
    public bool isBoss = false;

    [Header("Identificativo NPC")]
    public string npcID = "NPC_Alieno_1";

    // Proprietà dinamica per i salvataggi (unica per ogni NPC o per il Boss)
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
            PlayerPrefs.Save(); // Forza la scrittura immediata su disco
            Debug.Log($"[DEBUG SALVATAGGIO] IsBossDefeated salvato come: {value}");
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
    private bool waitingForDialogueToEnd = false; // Serve per sbloccare il boss alla fine del testo

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
            Debug.Log($"<color=yellow>[TEST BOSS] Boss sconfitto impostato a: {IsBossDefeated}</color>");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=cyan>[RESET] PlayerPrefs resettati! Partita nuova.</color>");
        }
        #endif

        // CONTROLLO PER SBLOCCARE IL BOSS APPENA FINISCE IL PRIMO DIALOGO
        if (isBoss && waitingForDialogueToEnd)
        {
            bool isDialogueActive = DialogueManager.Instance != null && 
                                     DialogueManager.Instance.dialoguePanel != null && 
                                     DialogueManager.Instance.dialoguePanel.activeSelf;

            // Se il dialogo era attivo ma ora si è chiuso, sblocchiamo il boss!
            if (!isDialogueActive)
            {
                waitingForDialogueToEnd = false;
                DemonBoss_Movement bossMovement = FindFirstObjectByType<DemonBoss_Movement>();
                if (bossMovement != null)
                {
                    bossMovement.EnableBossChase();
                    Debug.Log("[DEBUG] Dialogo del boss terminato: adesso parte l'inseguimento!");
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
                // Mettiamo in ascolto il controllo per sbloccare il boss a fine dialogo
                waitingForDialogueToEnd = true;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.FadeOutMusic(1.0f); // Ferma la BGM con un fade out di 1 secondo
                    AudioManager.Instance.Invoke("PlayBossMusic", 1.0f); // Fa partire la musica del boss subito dopo il fade
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