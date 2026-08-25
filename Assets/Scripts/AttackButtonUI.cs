using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackButtonUI : MonoBehaviour
{
    public static AttackButtonUI Instance;

    [Header("Riferimenti UI")]
    public Button actionButton;
    public Image buttonIcon;
    public TextMeshProUGUI buttonLabel;
    public CanvasGroup canvasGroup;

    [Header("I 3 Sprite")]
    public Sprite meleeSprite;  // Spada / Martello
    public Sprite rangedSprite; // Pistola / Fucile
    public Sprite bombSprite;   // Bomba

    [Header("I 3 Testi")]
    public string meleeText = "ATTACK";
    public string rangedText = "SHOOT";
    public string bombText = "LAUNCH";

    private Player_Combat playerCombat;

    private void Awake()
    {
        Instance = this;
        if (actionButton == null) actionButton = GetComponent<Button>();
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        FindPlayerCombat();
    }

    private void FindPlayerCombat()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCombat = player.GetComponent<Player_Combat>();
        }
    }

    private void Update()
    {
        if (playerCombat == null)
        {
            FindPlayerCombat();
            if (playerCombat == null) return;
        }

        // L'arma è effettivamente in mano se isWeaponDrawn è true E almeno un boolean dell'arma è attivo
        bool isWeaponEquippedInHand = playerCombat.isWeaponDrawn && 
            (playerCombat.hasSword || playerCombat.hasHammer || playerCombat.hasGun || playerCombat.hasRifle || playerCombat.hasBomb);

        SetInteractable(isWeaponEquippedInHand);

        // Aggiorna l'icona in base all'arma posseduta/selezionata
        UpdateIconAndText();
    }

    private void UpdateIconAndText()
    {
        if (playerCombat == null) return;

        if (playerCombat.storedWeapon == WeaponType.Gun || playerCombat.storedWeapon == WeaponType.Rifle)
        {
            SetRangedMode();
        }
        else if (playerCombat.storedWeapon == WeaponType.Bomb)
        {
            SetBombMode();
        }
        else
        {
            SetMeleeMode();
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        if (actionButton != null && actionButton.interactable != isInteractable)
        {
            actionButton.interactable = isInteractable;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = isInteractable ? 1f : 0.4f;
        }
    }

    public void SetMeleeMode()
    {
        if (buttonIcon != null && meleeSprite != null && buttonIcon.sprite != meleeSprite) 
            buttonIcon.sprite = meleeSprite;

        if (buttonLabel != null && buttonLabel.text != meleeText) 
            buttonLabel.text = meleeText;
    }

    public void SetRangedMode()
    {
        if (buttonIcon != null && rangedSprite != null && buttonIcon.sprite != rangedSprite) 
            buttonIcon.sprite = rangedSprite;

        if (buttonLabel != null && buttonLabel.text != rangedText) 
            buttonLabel.text = rangedText;
    }

    public void SetBombMode()
    {
        if (buttonIcon != null && bombSprite != null && buttonIcon.sprite != bombSprite) 
            buttonIcon.sprite = bombSprite;

        if (buttonLabel != null && buttonLabel.text != bombText) 
            buttonLabel.text = bombText;
    }
}