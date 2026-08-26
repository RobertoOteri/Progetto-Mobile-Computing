using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform defaultSpawnPoint; // SpaceShip
    public string playerTag = "Player";

    private void Start()
    {
        // Se stiamo caricando una partita salvata da "Continua", lasciamo fare a SaveSystem
        if (SaveSystem.Instance != null && SaveSystem.Instance.IsContinuingGame())
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        // Controlla se siamo appena passati da una porta/teleport tra scene
        string targetSpawnName = PlayerPrefs.GetString("TargetSpawnPoint", "");

        if (!string.IsNullOrEmpty(targetSpawnName))
        {
            GameObject targetSpawn = GameObject.Find(targetSpawnName);
            if (targetSpawn != null)
            {
                player.transform.position = targetSpawn.transform.position;
            }
            PlayerPrefs.DeleteKey("TargetSpawnPoint");
            PlayerPrefs.Save();
        }
        else if (defaultSpawnPoint != null)
        {
            // Spawn standard vicino all'astronave (Nuova Partita)
            player.transform.position = defaultSpawnPoint.position;
        }
    }

    [ContextMenu("Reset PlayerPrefs (Pulisci Salvataggi)")]
    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs completamente cancellati!");
    }
}