using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Tipo di questa arma a terra")]
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

                // 1. Lascia cadere l'arma che il player possedeva precedentemente
                switch (combat.storedWeapon)
                {
                    case WeaponType.Sword:
                        if (swordPickupPrefab != null) DropItem(swordPickupPrefab, dropPosition);
                        break;
                    case WeaponType.Hammer:
                        if (hammerPickupPrefab != null) DropItem(hammerPickupPrefab, dropPosition);
                        break;
                    case WeaponType.Rifle:
                        if (riflePickupPrefab != null) DropItem(riflePickupPrefab, dropPosition);
                        break;
                    case WeaponType.Gun:
                        if (gunPickupPrefab != null) DropItem(gunPickupPrefab, dropPosition);
                        break;
                    case WeaponType.Bomb:
                        if (bombPickupPrefab != null) DropItem(bombPickupPrefab, dropPosition);
                        break;
                }

                // 2. Assegna e impugna la nuova arma raccolta
                combat.EquipNewWeapon(weaponToEquip);

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