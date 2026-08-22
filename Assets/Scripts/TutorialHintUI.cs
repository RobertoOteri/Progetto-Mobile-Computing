using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialHintUI : MonoBehaviour
{
    public static TutorialHintUI Instance;

    [Header("Riferimenti UI")]
    public GameObject hintPanel;
    public TMP_Text hintText;

    [Header("Impostazioni")]
    public float displayDuration = 6f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        Instance = this;

        if (hintPanel == null)
        {
            Transform found = transform.Find("HintBox");
            if (found != null) hintPanel = found.gameObject;
            else
            {
                GameObject sceneObj = GameObject.Find("HintBox");
                if (sceneObj != null) hintPanel = sceneObj;
            }
        }

        if (hintText == null && hintPanel != null)
        {
            hintText = hintPanel.GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void Start()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (hintPanel != null && hintPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                HideHint();
            }
        }
    }

    public void ShowHint(string message)
    {
        if (hintPanel == null)
        {
            GameObject sceneObj = GameObject.Find("HintBox");
            if (sceneObj != null) hintPanel = sceneObj;
        }

        if (hintPanel == null) return;

        if (hintText != null)
        {
            hintText.text = message;
        }

        hintPanel.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(AutoHideRoutine());
    }

    public void HideHint()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }

    private IEnumerator AutoHideRoutine()
    {
        yield return new WaitForSeconds(displayDuration);
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
}