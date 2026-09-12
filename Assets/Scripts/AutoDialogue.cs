using System.Collections;
using UnityEngine;

public class AutoDialogue : MonoBehaviour
{
    [Header("Impostazioni Diario")]
    public string speakerName = "Jack Orbit";
    public Sprite portrait;

    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Ritardo Iniziale")]
    public float delayBeforeStart = 0.5f;

    [Header("Chiave Salvataggio")]
    public string saveKey = "GameIntroCompleted";

    private IEnumerator Start()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.IsContinuingGame())
        {
            gameObject.SetActive(false);
            yield break;
        }

        if (PlayerPrefs.GetInt(saveKey, 0) == 1)
        {
            gameObject.SetActive(false);
            yield break;
        }

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(delayBeforeStart);

        if (DialogueManager.Instance != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(speakerName, portrait, dialogueLines);
        }
    }
}