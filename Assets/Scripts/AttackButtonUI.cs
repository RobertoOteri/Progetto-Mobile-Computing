using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AttackButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
    private PlayerMovement playerMovement;
    private bool isPressed = false;

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
        FindPlayerComponents();
    }

    private void FindPlayerComponents()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCombat = player.GetComponent<Player_Combat>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if (playerCombat == null || playerMovement == null)
        {
            FindPlayerComponents();
            if (playerCombat == null) return;
        }

        // L'arma è effettivamente in mano se isWeaponDrawn è true E almeno un boolean dell'arma è attivo
        bool isWeaponEquippedInHand = playerCombat.isWeaponDrawn && 
            (playerCombat.hasSword || playerCombat.hasHammer || playerCombat.hasGun || playerCombat.hasRifle || playerCombat.hasBomb);

        SetInteractable(isWeaponEquippedInHand);

        // Aggiorna l'icona e il testo in base all'arma posseduta/selezionata
        UpdateIconAndText();

        // GESTIONE RAFFICA FUCILE: se il tasto è tenuto premuto
        if (isPressed && isWeaponEquippedInHand)
        {
            if (playerCombat.hasRifle)
            {
                float v = playerMovement != null ? playerMovement.GetVerticalInput() : 0f;
                float h = playerMovement != null ? playerMovement.GetHorizontalInput() : 0f;
                float lastV = playerMovement != null ? playerMovement.lastVertical : 0f;
                float lastH = playerMovement != null ? playerMovement.lastHorizontal : 1f;

                playerCombat.ExecuteCurrentWeaponAction(v, h, lastV, lastH);
            }
        }
    }

    // Viene chiamato appena tocchi lo schermo sul pulsante
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerCombat == null || playerMovement == null)
            FindPlayerComponents();

        if (playerCombat == null) return;

        bool isWeaponEquippedInHand = playerCombat.isWeaponDrawn && 
            (playerCombat.hasSword || playerCombat.hasHammer || playerCombat.hasGun || playerCombat.hasRifle || playerCombat.hasBomb);

        if (!isWeaponEquippedInHand) return;

        isPressed = true;

        if (playerMovement != null)
        {
            playerMovement.TriggerAttack();
        }
    }

    // Viene chiamato appena alzi il dito dal pulsante
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private void OnDisable()
    {
        isPressed = false;
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