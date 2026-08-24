using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DroppedWeaponData
{
    public WeaponType type;
    public Vector3 position;
}

[System.Serializable]
public class SceneDroppedItemsContainer
{
    public List<DroppedWeaponData> items = new List<DroppedWeaponData>();
}

public class DroppedItemSaver : MonoBehaviour
{
    public static DroppedItemSaver Instance;

    [Header("Prefab delle Armi")]
    public GameObject swordPrefab;
    public GameObject hammerPrefab;
    public GameObject riflePrefab;
    public GameObject gunPrefab;
    public GameObject bombPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    public void RegisterDroppedItem(string sceneName, WeaponType type, Vector3 position)
    {
        SceneDroppedItemsContainer container = LoadContainerForScene(sceneName);
        container.items.Add(new DroppedWeaponData { type = type, position = position });
        SaveContainerForScene(sceneName, container);
    }

    public void UnregisterDroppedItem(string sceneName, WeaponType type, Vector3 position)
    {
        SceneDroppedItemsContainer container = LoadContainerForScene(sceneName);
        for (int i = 0; i < container.items.Count; i++)
        {
            if (container.items[i].type == type && Vector3.Distance(container.items[i].position, position) < 1.5f)
            {
                container.items.RemoveAt(i);
                break;
            }
        }
        SaveContainerForScene(sceneName, container);
    }

    // Parte in automatico al cambio scena: legge le armi salvate e le ricrea nella mappa.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneDroppedItemsContainer container = LoadContainerForScene(scene.name);
        if (container.items.Count == 0) return;

        for (int i = 0; i < container.items.Count; i++)
        {
            GameObject prefabToSpawn = GetPrefabForWeapon(container.items[i].type);
            if (prefabToSpawn != null)
            {
                GameObject spawned = Instantiate(prefabToSpawn, container.items[i].position, Quaternion.identity);
                ItemPickup pickup = spawned.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    pickup.isRuntimeDropped = true;
                }
            }
        }
    }

    public GameObject GetPrefabForWeapon(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return swordPrefab;
            case WeaponType.Hammer: return hammerPrefab;
            case WeaponType.Rifle: return riflePrefab;
            case WeaponType.Gun: return gunPrefab;
            case WeaponType.Bomb: return bombPrefab;
            default: return null;
        }
    }

    // Legge la stringa JSON salvata nei PlayerPrefs e la converte in oggetti C#
    private SceneDroppedItemsContainer LoadContainerForScene(string sceneName)
    {
        string key = "DroppedItems_" + sceneName;
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<SceneDroppedItemsContainer>(json) ?? new SceneDroppedItemsContainer();
        }
        return new SceneDroppedItemsContainer();
    }

    // Converte gli oggetti C# in stringa JSON e li salva su disco nei PlayerPrefs
    private void SaveContainerForScene(string sceneName, SceneDroppedItemsContainer container)
    {
        string key = "DroppedItems_" + sceneName;
        string json = JsonUtility.ToJson(container);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
}