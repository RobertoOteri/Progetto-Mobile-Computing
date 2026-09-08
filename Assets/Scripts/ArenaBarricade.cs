using UnityEngine;

public class ArenaBarricade : MonoBehaviour
{
    public static ArenaBarricade Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Se il boss è già stato sconfitto in precedenza, assicurati che la strada sia libera
        if (NPCTriggerDialogue.IsBossDefeated)
        {
            OpenBarricade();
            return;
        }

        OpenBarricade();
    }

    // Chiamato a inizio scontro dal Boss
    public void CloseBarricade()
    {
        // Se il boss è già stato sconfitto, la barriera non deve mai più chiudersi
        if (NPCTriggerDialogue.IsBossDefeated)
        {
            Debug.Log("<color=orange>[BARRICATA] Chiusura ignorata: il boss è già sconfitto!</color>");
            return;
        }

        gameObject.SetActive(true);
        Debug.Log("<color=yellow>[BARRICATA] Chiusa: muro alzato e collider attivo!</color>");
    }

    // Chiamato alla sconfitta del Boss (da BossHealthBarUI)
    public void OpenBarricade()
    {
        gameObject.SetActive(false);
        Debug.Log("<color=green>[BARRICATA] Aperta: muro scomparso e passaggio libero!</color>");
    }
}