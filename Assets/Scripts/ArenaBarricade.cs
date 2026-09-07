using UnityEngine;

public class ArenaBarricade : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private GameObject visualsGroup; // Il contenitore con i 14 sprite
    [SerializeField] private Collider2D barrierCollider;  // Il Box Collider 2D

    private void Awake()
    {
        // All'avvio della scena la strada deve essere aperta
        OpenBarricade();
    }

    // Chiamato quando parte la boss fight
    public void CloseBarricade()
    {
        if (visualsGroup != null) visualsGroup.SetActive(true);
        if (barrierCollider != null) barrierCollider.enabled = true;
    }

    // Chiamato quando il boss muore
    public void OpenBarricade()
    {
        if (visualsGroup != null) visualsGroup.SetActive(false);
        if (barrierCollider != null) barrierCollider.enabled = false;
    }
}