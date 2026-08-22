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
    public bool isRightSide;
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
    private bool triggerGunHint = false;

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

    public void EnableHintOnNextDialogueEnd()
    {
        triggerGunHint = true;
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

        justOpened = true; 

        if (playerMovement == null)
            FindPlayerComponents();

        if (playerMovement != null)
        {
            playerMovement.StopMovement();
            playerMovement.enabled = false;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopWalkSound();
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

        if (triggerGunHint)
        {
            if (TutorialHintUI.Instance != null)
            {
                TutorialHintUI.Instance.ShowHint("Premi [1] per estrarre la pistola");
            }
            triggerGunHint = false;
        }
    }

    private void Update()
    {
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