using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    private string saveFilePath;
    public SaveData currentSaveData;
    private bool isContinuing = false;

    [Header("Prefab Armi a Terra (Pickups)")]
    [Tooltip("Assegna i prefab dei pickup delle armi corrispondenti all'enum WeaponType")]
    public GameObject[] weaponPickupPrefabs;

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

    // Controlla se il file di salvataggio esiste su disco
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    // Getter per sapere se stiamo caricando una partita salvata da "Continua"
    public bool IsContinuingGame()
    {
        return isContinuing;
    }

    // Cancella il file di salvataggio
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

    // Registra un consumabile (mela/pozione) come raccolto
    public void RegisterConsumedItem(string itemID)
    {
        if (currentSaveData == null) currentSaveData = new SaveData();

        if (!currentSaveData.consumedItems.Contains(itemID))
        {
            currentSaveData.consumedItems.Add(itemID);
        }
    }

    // Controlla se il consumabile è già stato usato
    public bool IsItemConsumed(string itemID)
    {
        if (currentSaveData != null && currentSaveData.consumedItems != null)
        {
            return currentSaveData.consumedItems.Contains(itemID);
        }
        return false;
    }

    // --- SALVATAGGIO COMPLETO ---
    public void SaveGame()
    {
        // 1. Forza la scrittura su disco dei PlayerPrefs (dialoghi, trigger, opzioni)
        PlayerPrefs.Save();

        if (currentSaveData == null) currentSaveData = new SaveData();

        // 2. Dati Player
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

        // 3. Dati Nemici
        currentSaveData.enemiesData.Clear();
        EnemySaveable[] allEnemies = Object.FindObjectsByType<EnemySaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (EnemySaveable enemy in allEnemies)
        {
            currentSaveData.enemiesData.Add(enemy.GetSaveData());
        }

        // 4. Dati Armi a Terra
        currentSaveData.droppedWeapons.Clear();
        WeaponPickupSaveable[] allGroundWeapons = Object.FindObjectsByType<WeaponPickupSaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (WeaponPickupSaveable w in allGroundWeapons)
        {
            if (w.gameObject.activeInHierarchy)
            {
                currentSaveData.droppedWeapons.Add(w.GetSaveData());
            }
        }

        // 5. Scrittura su file JSON
        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Partita salvata con successo in: " + saveFilePath);
    }

    // --- CARICAMENTO DATI ---
    public bool LoadGame()
    {
        if (!HasSaveFile()) return false;

        string json = File.ReadAllText(saveFilePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);
        return true;
    }

    // --- NUOVA PARTITA ---
    public void NewGame(string firstSceneName)
    {
        isContinuing = false;
        DeleteSaveFile();
        currentSaveData = null;

        // Resetta la memoria statica della vita per ripartire con i cuori pieni
        PlayerHealth.sessionHealth = -1;

        // Conserva le impostazioni di volume e luminosità
        float volMusica = PlayerPrefs.GetFloat("VolumeMusica", 10f);
        float volSuoni = PlayerPrefs.GetFloat("VolumeSuoni", 10f);
        float luminosita = PlayerPrefs.GetFloat("Luminosita", 1f);

        // Reset completo dei trigger di dialogo e memoria
        PlayerPrefs.DeleteAll();

        // Ripristina le opzioni utente
        PlayerPrefs.SetFloat("VolumeMusica", volMusica);
        PlayerPrefs.SetFloat("VolumeSuoni", volSuoni);
        PlayerPrefs.SetFloat("Luminosita", luminosita);
        PlayerPrefs.Save();

        SceneManager.LoadScene(firstSceneName);
    }

    // --- CONTINUA PARTITA ---
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
        StartCoroutine(RipristinaStatoScena(scene));
    }

    // Ripristina lo stato aspettando il termine del primo frame della scena
    private IEnumerator RipristinaStatoScena(Scene scene)
    {
        yield return null;

        GestisciMusicaScena(scene.name);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth hp = (player != null) ? player.GetComponent<PlayerHealth>() : null;
        Player_Combat combat = (player != null) ? player.GetComponent<Player_Combat>() : null;
        Rigidbody2D rb = (player != null) ? player.GetComponent<Rigidbody2D>() : null;

        if (isContinuing && currentSaveData != null)
        {
            // 1. Ripristino Player (Posizione, Fisica, Vita, Armi)
            if (player != null)
            {
                Vector3 targetPos = new Vector3(currentSaveData.playerPosX, currentSaveData.playerPosY, player.transform.position.z);
                player.transform.position = targetPos;

                if (rb != null)
                {
                    rb.position = targetPos;
                    rb.linearVelocity = Vector2.zero;
                }

                if (hp != null)
                {
                    hp.maxHealth = currentSaveData.maxHealth;
                    hp.currentHealth = currentSaveData.currentHealth;
                    PlayerHealth.sessionHealth = currentSaveData.currentHealth; // Allinea la variabile tra scene
                }

                if (combat != null)
                {
                    combat.storedWeapon = (WeaponType)currentSaveData.storedWeapon;
                    combat.isWeaponDrawn = currentSaveData.isWeaponDrawn;
                    combat.ApplyWeaponState();
                }
            }

            // 2. Ripristino dei Nemici
            EnemySaveable[] currentEnemies = Object.FindObjectsByType<EnemySaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (EnemySaveable enemy in currentEnemies)
            {
                EnemySaveData savedEnemy = currentSaveData.enemiesData.Find(e => e.enemyID == enemy.enemyID);
                if (savedEnemy != null)
                {
                    enemy.LoadData(savedEnemy);
                }
            }

            // 3. Ripristino Armi a Terra
            WeaponPickupSaveable[] existingWeapons = Object.FindObjectsByType<WeaponPickupSaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (WeaponPickupSaveable w in existingWeapons)
            {
                Destroy(w.gameObject);
            }

            if (weaponPickupPrefabs != null && weaponPickupPrefabs.Length > 0)
            {
                foreach (DroppedWeaponSaveData drop in currentSaveData.droppedWeapons)
                {
                    GameObject prefabToSpawn = null;
                    foreach (GameObject p in weaponPickupPrefabs)
                    {
                        if (p != null)
                        {
                            WeaponPickupSaveable script = p.GetComponent<WeaponPickupSaveable>();
                            if (script != null && (int)script.weaponType == drop.weaponType)
                            {
                                prefabToSpawn = p;
                                break;
                            }
                        }
                    }

                    if (prefabToSpawn != null)
                    {
                        Vector3 spawnPos = new Vector3(drop.posX, drop.posY, 0f);
                        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    }
                }
            }
        }

        isContinuing = false;
    }

    // Gestione automatica della musica durante il caricamento
    private void GestisciMusicaScena(string sceneName)
    {
        if (sceneName == "Menu") return;

        if (AudioManager.Instance != null && !AudioManager.Instance.IsMusicPlaying())
        {
            if (AudioManager.Instance.bgmMusic != null)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.bgmMusic, 0.5f);
            }
            else
            {
                AudioSource scenaMusic = Object.FindAnyObjectByType<AudioSource>();
                if (scenaMusic != null && scenaMusic.clip != null)
                {
                    AudioManager.Instance.PlayMusic(scenaMusic.clip, 0.5f);
                }
            }
        }
    }
}