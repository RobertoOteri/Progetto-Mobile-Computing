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
    [Tooltip("Assegna i prefab dei pickup delle armi corrispondenti all'enum WeaponType se vengono istanziati da zero")]
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

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public bool IsContinuingGame()
    {
        return isContinuing;
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

    // --- SALVATAGGIO COMPLETO ---
    public void SaveGame()
    {
        PlayerPrefs.Save();

        if (currentSaveData == null) currentSaveData = new SaveData();

        // 1. Dati Player
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

        // 2. Dati Nemici
        currentSaveData.enemiesData.Clear();
        EnemySaveable[] allEnemies = Object.FindObjectsByType<EnemySaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (EnemySaveable enemy in allEnemies)
        {
            currentSaveData.enemiesData.Add(enemy.GetSaveData());
        }

        // 3. Dati Armi a Terra
        currentSaveData.droppedWeapons.Clear();
        WeaponPickupSaveable[] allGroundWeapons = Object.FindObjectsByType<WeaponPickupSaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (WeaponPickupSaveable w in allGroundWeapons)
        {
            if (w.gameObject.activeInHierarchy)
            {
                currentSaveData.droppedWeapons.Add(w.GetSaveData());
            }
        }

        // 4. Scrittura File
        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Partita salvata con successo (con nemici e armi a terra) in: " + saveFilePath);
    }

    public bool LoadGame()
    {
        if (!HasSaveFile()) return false;

        string json = File.ReadAllText(saveFilePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);
        return true;
    }

    public void NewGame(string firstSceneName)
    {
        isContinuing = false;
        DeleteSaveFile();
        currentSaveData = null;

        float volMusica = PlayerPrefs.GetFloat("VolumeMusica", 10f);
        float volSuoni = PlayerPrefs.GetFloat("VolumeSuoni", 10f);
        float luminosita = PlayerPrefs.GetFloat("Luminosita", 1f);

        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetFloat("VolumeMusica", volMusica);
        PlayerPrefs.SetFloat("VolumeSuoni", volSuoni);
        PlayerPrefs.SetFloat("Luminosita", luminosita);
        PlayerPrefs.Save();

        SceneManager.LoadScene(firstSceneName);
    }

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
            // Ripristino Player
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
                }

                if (combat != null)
                {
                    combat.storedWeapon = (WeaponType)currentSaveData.storedWeapon;
                    combat.isWeaponDrawn = currentSaveData.isWeaponDrawn;
                    combat.ApplyWeaponState();
                }
            }

            // Ripristino Nemici
            EnemySaveable[] currentEnemies = Object.FindObjectsByType<EnemySaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (EnemySaveable enemy in currentEnemies)
            {
                EnemySaveData savedEnemy = currentSaveData.enemiesData.Find(e => e.enemyID == enemy.enemyID);
                if (savedEnemy != null)
                {
                    enemy.LoadData(savedEnemy);
                }
            }

            // Ripristino Armi a Terra
            // 1. Rimuove i pickup presenti di default nella scena caricata
            WeaponPickupSaveable[] existingWeapons = Object.FindObjectsByType<WeaponPickupSaveable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (WeaponPickupSaveable w in existingWeapons)
            {
                Destroy(w.gameObject);
            }

            // 2. Ricrea esattamente le armi registrate al momento del salvataggio
            if (weaponPickupPrefabs != null && weaponPickupPrefabs.Length > 0)
            {
                foreach (DroppedWeaponSaveData drop in currentSaveData.droppedWeapons)
                {
                    // Cerca il prefab che corrisponde al tipo di arma salvato
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
    // Registra una mela/pozione come consumata
    public void RegisterConsumedItem(string itemID)
    {
        if (currentSaveData == null) currentSaveData = new SaveData();

        if (!currentSaveData.consumedItems.Contains(itemID))
        {
            currentSaveData.consumedItems.Add(itemID);
        }
    }

    // Controlla se la mela/pozione è già stata usata in questa partita
    public bool IsItemConsumed(string itemID)
    {
        if (currentSaveData != null && currentSaveData.consumedItems != null)
        {
            return currentSaveData.consumedItems.Contains(itemID);
        }
        return false;
    }
}