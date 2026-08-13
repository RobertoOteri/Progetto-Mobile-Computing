using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Riferimenti UI Generali")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("UI Sinistra (Player / Navicella)")]
    public GameObject portraitBoxLeft;
    public Image portraitImageLeft;
    public GameObject nameTagLeft;
    public TMP_Text nameTextLeft;
    
    [Header("UI Destra (NPC)")]
    public GameObject portraitBoxRight;
    public Image portraitImageRight;
    public GameObject nameTagRight;
    public TMP_Text nameTextRight;

    [Header("Impostazioni")]
    public float textSpeed = 0.03f;

    private Queue<string> sentences = new Queue<string>();
    private bool isTyping = false;
    private string currentSentence;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        FindPlayerComponents();
    }

    private void FindPlayerComponents()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    public void StartDialogue(string speakerName, Sprite portrait, string[] lines, bool isPortraitOnRight = false)
    {
        dialoguePanel.SetActive(true);

        if (playerMovement == null) 
            FindPlayerComponents();

        // Blocca il movimento del personaggio
        if (playerMovement != null)
        {
            playerMovement.StopMovement();
            playerMovement.enabled = false;
        }

        // --- GESTIONE DESTRA vs SINISTRA ---
        if (isPortraitOnRight)
        {
            // === DISPOSIZIONE DESTRA (NPC) ===
            if (portraitBoxLeft != null) portraitBoxLeft.SetActive(false);
            if (nameTagLeft != null) nameTagLeft.SetActive(false);

            if (portraitBoxRight != null) portraitBoxRight.SetActive(portrait != null);
            if (portraitImageRight != null && portrait != null) portraitImageRight.sprite = portrait;

            if (nameTagRight != null) nameTagRight.SetActive(true);
            if (nameTextRight != null) nameTextRight.text = speakerName;
        }
        else
        {
            // === DISPOSIZIONE SINISTRA (Player / Navicella) ===
            if (portraitBoxRight != null) portraitBoxRight.SetActive(false);
            if (nameTagRight != null) nameTagRight.SetActive(false);

            if (portraitBoxLeft != null) portraitBoxLeft.SetActive(portrait != null);
            if (portraitImageLeft != null && portrait != null) portraitImageLeft.sprite = portrait;

            if (nameTagLeft != null) nameTagLeft.SetActive(true);
            if (nameTextLeft != null) nameTextLeft.text = speakerName;
        }

        sentences.Clear();
        foreach (string line in lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            DisplayNextSentence();
        }
    }
}