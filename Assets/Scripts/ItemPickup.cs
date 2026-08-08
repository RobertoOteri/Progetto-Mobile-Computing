using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum WeaponType { Sword, Hammer, Rifle, Gun } // AGGIUNTO Gun

    [Header("Tipo di questa arma")]
    public WeaponType weaponToEquip;

    [Header("Prefab delle armi da terra")]
    public GameObject swordPickupPrefab;  
    public GameObject hammerPickupPrefab; 
    public GameObject riflePickupPrefab;
    public GameObject gunPickupPrefab; // AGGIUNTO: Prefab per la pistola da terra

    private bool canBePickedUp = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;

        if (other.CompareTag("Player")) // Permette solo al Player di raccogliere le armi
        {
            Player_Combat combat = other.GetComponent<Player_Combat>();

            if (combat != null)
            {
                Vector3 dropPosition = other.transform.position + new Vector3(0.8f, 0f, 0f); // Posizione di drop a destra

                // Spawna a terra l'arma precedente e imposta la scala corretta
                if (combat.hasSword && swordPickupPrefab != null)
                {
                    GameObject droppedItem = Instantiate(swordPickupPrefab, dropPosition, Quaternion.identity);
                    droppedItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    droppedItem.GetComponent<ItemPickup>().StartCoroutine(droppedItem.GetComponent<ItemPickup>().PickupCooldown());
                }
                else if (combat.hasHammer && hammerPickupPrefab != null)
                {
                    GameObject droppedItem = Instantiate(hammerPickupPrefab, dropPosition, Quaternion.identity);
                    droppedItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    droppedItem.GetComponent<ItemPickup>().StartCoroutine(droppedItem.GetComponent<ItemPickup>().PickupCooldown());
                }
                else if (combat.hasRifle && riflePickupPrefab != null)
                {
                    GameObject droppedItem = Instantiate(riflePickupPrefab, dropPosition, Quaternion.identity);
                    droppedItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    droppedItem.GetComponent<ItemPickup>().StartCoroutine(droppedItem.GetComponent<ItemPickup>().PickupCooldown());
                }
                else if (combat.hasGun && gunPickupPrefab != null) // AGGIUNTO: Se avevi la pistola, la droppa a terra
                {
                    GameObject droppedItem = Instantiate(gunPickupPrefab, dropPosition, Quaternion.identity);
                    droppedItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    droppedItem.GetComponent<ItemPickup>().StartCoroutine(droppedItem.GetComponent<ItemPickup>().PickupCooldown());
                }

                // Equipaggia la nuova arma
                if (weaponToEquip == WeaponType.Sword) combat.EquipSword();
                else if (weaponToEquip == WeaponType.Hammer) combat.EquipHammer();
                else if (weaponToEquip == WeaponType.Rifle) combat.EquipRifle();
                else if (weaponToEquip == WeaponType.Gun) combat.EquipGun(); // AGGIUNTO

                // Rimuove l'oggetto raccolto
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator PickupCooldown()
    {
        canBePickedUp = false;
        yield return new WaitForSeconds(1f);
        canBePickedUp = true;
    }
}