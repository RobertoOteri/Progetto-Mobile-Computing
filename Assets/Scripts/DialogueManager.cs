using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Riferimenti UI")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;

    [Header("Impostazioni")]
    public float textSpeed = 0.03f;

    private Queue<string> sentences = new Queue<string>();
    private bool isTyping = false;
    private string currentSentence;

    // Riferimento allo script di movimento del giocatore
    private PlayerMovement playerMovement;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Trova il giocatore e recupera il suo PlayerMovement
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

    public void StartDialogue(string speakerName, Sprite portrait, string[] lines)
    {
        dialoguePanel.SetActive(true);
        nameText.text = speakerName;

        // Se non abbiamo ancora il riferimento al Player, proviamo a cercarlo
        if (playerMovement == null) 
            FindPlayerComponents();

        // --- BLOCCA IL MOVIMENTO E RESETTA L'ANIMATORE ---
        if (playerMovement != null)
        {
            playerMovement.StopMovement(); // Azzera la fisica e imposta horizontal=0 e vertical=0 sull'Animator
            playerMovement.enabled = false; // Spegne lo script per bloccare gli input
        }

        // Gestione immagine ritratto
        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        // Prepara le frasi del dialogo
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

        // --- SBLOCCA IL GIOCATORE ---
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