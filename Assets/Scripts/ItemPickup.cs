using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum WeaponType { Sword, Hammer, Rifle, Gun, Bomb }

    [Header("Tipo di questa arma")]
    public WeaponType weaponToEquip;

    [Header("Prefab delle armi da terra")]
    public GameObject swordPickupPrefab;  
    public GameObject hammerPickupPrefab; 
    public GameObject riflePickupPrefab;
    public GameObject gunPickupPrefab;
    public GameObject bombPickupPrefab;

    private bool canBePickedUp = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;

        if (other.CompareTag("Player"))
        {
            Player_Combat combat = other.GetComponent<Player_Combat>();

            if (combat != null)
            {
                Vector3 dropPosition = other.transform.position + new Vector3(0.8f, 0f, 0f);

                // Drop dell'arma precedente mantenendo la scala del Prefab originale
                if (combat.hasSword && swordPickupPrefab != null)
                {
                    DropItem(swordPickupPrefab, dropPosition);
                }
                else if (combat.hasHammer && hammerPickupPrefab != null)
                {
                    DropItem(hammerPickupPrefab, dropPosition);
                }
                else if (combat.hasRifle && riflePickupPrefab != null)
                {
                    DropItem(riflePickupPrefab, dropPosition);
                }
                else if (combat.hasGun && gunPickupPrefab != null)
                {
                    DropItem(gunPickupPrefab, dropPosition);
                }
                else if (combat.hasBomb && bombPickupPrefab != null)
                {
                    DropItem(bombPickupPrefab, dropPosition);
                }

                // Equipaggia la nuova arma
                if (weaponToEquip == WeaponType.Sword) combat.EquipSword();
                else if (weaponToEquip == WeaponType.Hammer) combat.EquipHammer();
                else if (weaponToEquip == WeaponType.Rifle) combat.EquipRifle();
                else if (weaponToEquip == WeaponType.Gun) combat.EquipGun();
                else if (weaponToEquip == WeaponType.Bomb) combat.EquipBomb();

                if (AudioManager.Instance != null && AudioManager.Instance.pickupSFX != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupSFX);
                }
                    
                Destroy(gameObject);
            }
        }
    }

    private void DropItem(GameObject prefab, Vector3 position)
    {
        GameObject droppedItem = Instantiate(prefab, position, Quaternion.identity);
        droppedItem.transform.localScale = prefab.transform.localScale;

        ItemPickup pickup = droppedItem.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.StartCoroutine(pickup.PickupCooldown());
        }
    }

    public IEnumerator PickupCooldown()
    {
        canBePickedUp = false;
        yield return new WaitForSeconds(1f);
        canBePickedUp = true;
    }
}