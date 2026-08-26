using System.Collections;
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

    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }

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

    public void NewGame(string firstSceneName)
    {
        isContinuing = false;
        DeleteSaveFile();
        currentSaveData = null;

        // Salva le impostazioni per non perderle
        float volMusica = PlayerPrefs.GetFloat("VolumeMusica", 10f);
        float volSuoni = PlayerPrefs.GetFloat("VolumeSuoni", 10f);
        float luminosita = PlayerPrefs.GetFloat("Luminosita", 1f);

        // Reset completo di chiavi e trigger
        PlayerPrefs.DeleteAll();

        // Ripristina opzioni
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
        if (player == null) yield break;

        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        Player_Combat combat = player.GetComponent<Player_Combat>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (isContinuing && currentSaveData != null)
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
    public bool IsContinuingGame()
    {
        return isContinuing;
    }   
}