using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueLine
{
    public string speakerName;
    public Sprite portrait;
    public bool isRightSide; // True = Destra (NPC/Alieno), False = Sinistra (Player)
    [TextArea(2, 4)]
    public string sentence;
}

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

    [Header("UI Destra (NPC / Alieni)")]
    public GameObject portraitBoxRight;
    public Image portraitImageRight;
    public GameObject nameTagRight;
    public TMP_Text nameTextRight;

    [Header("Impostazioni")]
    public float textSpeed = 0.03f;

    private Queue<DialogueLine> dialogueLinesQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private string currentSentence;
    private PlayerMovement playerMovement;
    private bool justOpened = false;

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
        List<DialogueLine> sequence = new List<DialogueLine>();
        foreach (string line in lines)
        {
            DialogueLine dl = new DialogueLine
            {
                speakerName = speakerName,
                portrait = portrait,
                isRightSide = isPortraitOnRight,
                sentence = line
            };
            sequence.Add(dl);
        }

        StartDialogueSequence(sequence);
    }

    public void StartDialogueSequence(List<DialogueLine> lines)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        justOpened = true; // Impedisce a Update di consumare l'input in questo frame

        if (playerMovement == null)
            FindPlayerComponents();

        if (playerMovement != null)
        {
            playerMovement.StopMovement();
            playerMovement.enabled = false;
        }
        if (AudioManager.Instance != null)
        {
            // Se hai un metodo dedicato nell'AudioManager per fermare i passi o gli SFX di movimento:
            AudioManager.Instance.StopWalkSound(); // Sostituisci col nome del tuo metodo
        }

        dialogueLinesQueue.Clear();
        foreach (var line in lines)
        {
            dialogueLinesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Se si salta la frase mentre sta scrivendo, ferma l'effetto visivo e sonoro
        if (isTyping)
        {
            StopAllCoroutines();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopTypewriterSound();
            }

            if (dialogueText != null) dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (dialogueLinesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLinesQueue.Dequeue();
        currentSentence = line.sentence;

        // Gestione Layout UI Sicura con controlli null
        if (line.isRightSide)
        {
            if (portraitBoxLeft != null) portraitBoxLeft.SetActive(false);
            if (nameTagLeft != null) nameTagLeft.SetActive(false);

            if (portraitBoxRight != null) portraitBoxRight.SetActive(line.portrait != null);
            if (portraitImageRight != null && line.portrait != null) portraitImageRight.sprite = line.portrait;

            if (nameTagRight != null) nameTagRight.SetActive(true);
            if (nameTextRight != null) nameTextRight.text = line.speakerName;
        }
        else
        {
            if (portraitBoxRight != null) portraitBoxRight.SetActive(false);
            if (nameTagRight != null) nameTagRight.SetActive(false);

            if (portraitBoxLeft != null) portraitBoxLeft.SetActive(line.portrait != null);
            if (portraitImageLeft != null && line.portrait != null) portraitImageLeft.sprite = line.portrait;

            if (nameTagLeft != null) nameTagLeft.SetActive(true);
            if (nameTextLeft != null) nameTextLeft.text = line.speakerName;
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText != null) dialogueText.text = "";
        isTyping = true;

        int charCount = 0;

        foreach (char letter in sentence.ToCharArray())
        {
            if (dialogueText != null) dialogueText.text += letter;

            // Suona ogni 2 lettere (escludendo gli spazi vuoti)
            if (letter != ' ' && charCount % 2 == 0)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayTypewriterSound();
                }
            }

            charCount++;
            yield return new WaitForSeconds(textSpeed);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTypewriterSound();
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        // Interrompe il suono se il dialogo viene chiuso
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopTypewriterSound();
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void Update()
    {
        // Se il dialogo è appena stato aperto in questo frame, ignoriamo l'input per evitare skip immediati
        if (justOpened)
        {
            justOpened = false;
            return;
        }

        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextSentence();
            }
        }
    }
}