using UnityEngine;

public class NPCZoneTrigger : MonoBehaviour
{
    public enum ZoneType { FirstContact, RepeatInteract }
    
    [Header("Tipo di Zona")]
    public ZoneType zoneType;

    public NPCTriggerDialogue parentDialogue;

    private void Awake()
    {
        if (parentDialogue == null)
            parentDialogue = GetComponentInParent<NPCTriggerDialogue>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrigger(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (zoneType == ZoneType.FirstContact)
            HandleTrigger(other);
    }

    private void HandleTrigger(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (parentDialogue == null)
            parentDialogue = GetComponentInParent<NPCTriggerDialogue>();

        if (parentDialogue == null) return;

        if (zoneType == ZoneType.FirstContact)
        {
            parentDialogue.OnFirstContactTrigger();
        }
        else if (zoneType == ZoneType.RepeatInteract)
        {
            parentDialogue.SetPlayerInRepeatZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (parentDialogue == null)
            parentDialogue = GetComponentInParent<NPCTriggerDialogue>();

        if (parentDialogue != null && zoneType == ZoneType.RepeatInteract)
        {
            parentDialogue.SetPlayerInRepeatZone(false);
        }
    }
}