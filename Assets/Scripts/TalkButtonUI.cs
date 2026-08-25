using UnityEngine;
using UnityEngine.UI;

public class TalkButtonUI : MonoBehaviour
{
    [Header("Riferimenti")]
    public Button talkButton;
    public CanvasGroup canvasGroup;

    [Header("Raggio di Ricerca")]
    public float interactionRadius = 1.8f;
    public LayerMask interactableLayers = ~0; // Cerca su tutti i layer per default

    private Transform playerTransform;

    private void Awake()
    {
        if (talkButton == null) talkButton = GetComponent<Button>();
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        bool canTalk = CheckIfCanInteract();
        SetInteractable(canTalk);
    }

    private bool CheckIfCanInteract()
    {
        // 1. Se il dialogo è aperto a schermo, il tasto è sempre attivo (serve per andare avanti)
        if (DialogueManager.Instance != null && 
            DialogueManager.Instance.dialoguePanel != null && 
            DialogueManager.Instance.dialoguePanel.activeSelf)
        {
            return true;
        }

        // 2. Se il dialogo è chiuso, controlla se c'è un NPC o la Navicella vicini al Player
        Collider2D[] colliders = Physics2D.OverlapCircleAll(playerTransform.position, interactionRadius, interactableLayers);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Player")) continue;

            // Se l'oggetto vicino ha uno script di dialogo o interazione
            if (col.GetComponent<NPCTriggerDialogue>() != null || 
                col.GetComponent<SpaceshipInteract>() != null ||
                col.GetComponentInParent<NPCTriggerDialogue>() != null ||
                col.GetComponentInParent<SpaceshipInteract>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void SetInteractable(bool state)
    {
        if (talkButton != null && talkButton.interactable != state)
        {
            talkButton.interactable = state;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = state ? 1f : 0.4f;
        }
    }
}