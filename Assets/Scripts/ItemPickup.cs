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

                // 1. Droppa l'arma attualmente posseduta
                GameObject itemToDrop = null;

                if (combat.hasSword)
                {
                    itemToDrop = swordPickupPrefab;
                }
                else if (combat.hasHammer)
                {
                    itemToDrop = hammerPickupPrefab;
                }
                else if (combat.hasRifle)
                {
                    itemToDrop = riflePickupPrefab;
                }
                else if (combat.hasGun)
                {
                    itemToDrop = gunPickupPrefab;
                }
                else if (combat.hasBomb)
                {
                    itemToDrop = bombPickupPrefab;
                }

                if (itemToDrop != null)
                {
                    DropItem(itemToDrop, dropPosition);
                }

                // 2. Equipaggia la nuova arma
                switch (weaponToEquip)
                {
                    case WeaponType.Sword:
                        combat.EquipSword();
                        break;
                    case WeaponType.Hammer:
                        combat.EquipHammer();
                        break;
                    case WeaponType.Rifle:
                        combat.EquipRifle();
                        break;
                    case WeaponType.Gun:
                        combat.EquipGun();
                        break;
                    case WeaponType.Bomb:
                        combat.EquipBomb();
                        break;
                }

                // 3. Distrugge l'oggetto raccolto
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
            pickup.canBePickedUp = false;
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