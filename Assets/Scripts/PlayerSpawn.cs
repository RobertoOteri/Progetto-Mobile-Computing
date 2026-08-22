using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform defaultSpawnPoint; // SpaceShip
    public string playerTag = "Player";

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        // Controlla se siamo appena passati da una porta/teleport
        string targetSpawnName = PlayerPrefs.GetString("TargetSpawnPoint", "");

        if (!string.IsNullOrEmpty(targetSpawnName))
        {
            GameObject targetSpawn = GameObject.Find(targetSpawnName);
            if (targetSpawn != null)
            {
                player.transform.position = targetSpawn.transform.position;
            }
            // Cancella subito la chiave per il prossimo avvio pulito
            PlayerPrefs.DeleteKey("TargetSpawnPoint");
        }
        else if (defaultSpawnPoint != null)
        {
            // Spawn standard vicino alla astronave
            player.transform.position = defaultSpawnPoint.position;
        }
    }

    // 🔴 AGGIUNGI QUESTO: Cancella la memoria vecchia premendo Clic Destro sullo script nell'Inspector
    [ContextMenu("Reset PlayerPrefs (Pulisci Salvataggi)")]
    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs completamente cancellati!");
    }
}