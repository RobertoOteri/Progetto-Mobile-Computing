using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    private string saveFilePath;
    public SaveData currentSaveData;
    private bool isContinuing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    // Cancella il file se vuoi resettare del tutto
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

    // Salva SOLO quando viene chiamato esplicitamente dal tasto Salva
    public void SaveGame()
    {
        if (currentSaveData == null) currentSaveData = new SaveData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            currentSaveData.playerPosX = player.transform.position.x;
            currentSaveData.playerPosY = player.transform.position.y;

            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                currentSaveData.currentHealth = hp.currentHealth;
                currentSaveData.maxHealth = hp.maxHealth;
            }

            Player_Combat combat = player.GetComponent<Player_Combat>();
            if (combat != null)
            {
                currentSaveData.storedWeapon = (int)combat.storedWeapon;
                currentSaveData.isWeaponDrawn = combat.isWeaponDrawn;
            }
        }

        currentSaveData.sceneName = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Partita salvata in: " + saveFilePath);
    }

    public bool LoadGame()
    {
        if (!HasSaveFile()) return false;

        string json = File.ReadAllText(saveFilePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);
        return true;
    }

    // NUOVA PARTITA: Non scrive il file subito, ma rimuove il vecchio
    public void NewGame(string firstSceneName)
    {
        isContinuing = false;
        DeleteSaveFile(); // Elimina il vecchio salvataggio
        currentSaveData = null;
        SceneManager.LoadScene(firstSceneName);
    }

    // CONTINUA: Carica i dati e viaggia verso la scena salvata
    public void ContinueGame()
    {
        if (LoadGame())
        {
            isContinuing = true;
            SceneManager.LoadScene(currentSaveData.sceneName);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isContinuing || currentSaveData == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(currentSaveData.playerPosX, currentSaveData.playerPosY, player.transform.position.z);

            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.maxHealth = currentSaveData.maxHealth;
                hp.currentHealth = currentSaveData.currentHealth;
            }

            Player_Combat combat = player.GetComponent<Player_Combat>();
            if (combat != null)
            {
                combat.storedWeapon = (WeaponType)currentSaveData.storedWeapon;
                combat.isWeaponDrawn = currentSaveData.isWeaponDrawn;
                combat.ApplyWeaponState();
            }
        }

        isContinuing = false;
    }
}