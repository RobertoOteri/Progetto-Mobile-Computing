using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackButtonUI : MonoBehaviour
{
    public static AttackButtonUI Instance;

    [Header("Riferimenti UI")]
    public Image buttonIcon;
    public TextMeshProUGUI buttonLabel;

    [Header("I 3 Sprite")]
    public Sprite meleeSprite;  // Spada / Martello
    public Sprite rangedSprite; // Pistola / Fucile
    public Sprite bombSprite;   // Bomba

    [Header("I 3 Testi")]
    public string meleeText = "ATTACK";
    public string rangedText = "SHOOT";
    public string bombText = "LAUNCH";

    private void Awake()
    {
        Instance = this;
    }

    // 1. Corpo a corpo
    public void SetMeleeMode()
    {
        if (buttonIcon != null && meleeSprite != null) 
            buttonIcon.sprite = meleeSprite;

        if (buttonLabel != null) 
            buttonLabel.text = meleeText;
    }

    // 2. A distanza (Pistola, Fucile)
    public void SetRangedMode()
    {
        if (buttonIcon != null && rangedSprite != null) 
            buttonIcon.sprite = rangedSprite;

        if (buttonLabel != null) 
            buttonLabel.text = rangedText;
    }

    // 3. Bomba
    public void SetBombMode()
    {
        if (buttonIcon != null && bombSprite != null) 
            buttonIcon.sprite = bombSprite;

        if (buttonLabel != null) 
            buttonLabel.text = bombText;
    }
}