using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform spawnPoint; // Il punto vicino alla navicella
    public string playerTag = "Player"; // Assicurati che l'astronauta abbia questo Tag

    private void Start()
    {
        // Cerca il giocatore nella scena tramite il suo Tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null && spawnPoint != null)
        {
            // Sposta il personaggio sulla posizione dello SpawnPoint
            player.transform.position = spawnPoint.position;
        }
        else
        {
            Debug.LogWarning("Player o SpawnPoint non trovati nella scena!");
        }
    }
}