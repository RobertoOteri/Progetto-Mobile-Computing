using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    [Header("Identificatore Unico")]
    [Tooltip("Scrivi un nome unico per le armi piazzate da te nella mappa (es: Scena2_Martello).")]
    public string weaponID;

    [Header("Tipo di questa arma a terra")]
    public WeaponType weaponToEquip;

    [Header("Prefab delle armi da terra")]
    public GameObject swordPickupPrefab;  
    public GameObject hammerPickupPrefab; 
    public GameObject riflePickupPrefab;
    public GameObject gunPickupPrefab;
    public GameObject bombPickupPrefab;

    [HideInInspector] public bool isRuntimeDropped = false;
    private bool canBePickedUp = true;

    private void Start()
    {
        // Se è un'arma originale della mappa e risulta già raccolta, la distrugge
        if (!isRuntimeDropped && !string.IsNullOrEmpty(weaponID) && PlayerPrefs.GetInt("WeaponPicked_" + weaponID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;

        if (other.CompareTag("Player"))
        {
            Player_Combat combat = other.GetComponent<Player_Combat>();

            if (combat != null)
            {
                Vector3 dropPosition = other.transform.position + new Vector3(0.8f, 0f, 0f);

                // 1. Lascia cadere l'arma precedente del Player
                switch (combat.storedWeapon)
                {
                    case WeaponType.Sword:
                        if (swordPickupPrefab != null) DropItem(swordPickupPrefab, dropPosition, WeaponType.Sword);
                        break;
                    case WeaponType.Hammer:
                        if (hammerPickupPrefab != null) DropItem(hammerPickupPrefab, dropPosition, WeaponType.Hammer);
                        break;
                    case WeaponType.Rifle:
                        if (riflePickupPrefab != null) DropItem(riflePickupPrefab, dropPosition, WeaponType.Rifle);
                        break;
                    case WeaponType.Gun:
                        if (gunPickupPrefab != null) DropItem(gunPickupPrefab, dropPosition, WeaponType.Gun);
                        break;
                    case WeaponType.Bomb:
                        if (bombPickupPrefab != null) DropItem(bombPickupPrefab, dropPosition, WeaponType.Bomb);
                        break;
                }

                // 2. Se stiamo raccogliendo un'arma creata a runtime, la rimuoviamo dal salvataggio
                if (isRuntimeDropped && DroppedItemSaver.Instance != null)
                {
                    DroppedItemSaver.Instance.UnregisterDroppedItem(SceneManager.GetActiveScene().name, weaponToEquip, transform.position);
                }
                // Se è un'arma originale della mappa, la segniamo come raccolta permanentemente
                else if (!string.IsNullOrEmpty(weaponID))
                {
                    PlayerPrefs.SetInt("WeaponPicked_" + weaponID, 1);
                    PlayerPrefs.Save();
                }

                // 3. Equipaggia la nuova arma
                combat.EquipNewWeapon(weaponToEquip);

                if (AudioManager.Instance != null && AudioManager.Instance.pickupSFX != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupSFX);
                }
                    
                Destroy(gameObject);
            }
        }
    }

    private void DropItem(GameObject prefab, Vector3 position, WeaponType type)
    {
        GameObject droppedItem = Instantiate(prefab, position, Quaternion.identity);
        droppedItem.transform.localScale = prefab.transform.localScale;

        ItemPickup pickup = droppedItem.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.isRuntimeDropped = true;
            
            // Registra l'arma lasciata a terra per ritrovarla quando si torna in questa scena
            if (DroppedItemSaver.Instance == null)
            {
                GameObject saverObj = new GameObject("DroppedItemSaver");
                saverObj.AddComponent<DroppedItemSaver>();
            }
            
            DroppedItemSaver.Instance.RegisterDroppedItem(SceneManager.GetActiveScene().name, type, position);

            pickup.StartCoroutine(pickup.PickupCooldown());
        }
    }

    public GameObject GetPrefabForWeapon(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return swordPickupPrefab;
            case WeaponType.Hammer: return hammerPickupPrefab;
            case WeaponType.Rifle: return riflePickupPrefab;
            case WeaponType.Gun: return gunPickupPrefab;
            case WeaponType.Bomb: return bombPickupPrefab;
            default: return null;
        }
    }

    public IEnumerator PickupCooldown()
    {
        canBePickedUp = false;
        yield return new WaitForSeconds(1f);
        canBePickedUp = true;
    }
}