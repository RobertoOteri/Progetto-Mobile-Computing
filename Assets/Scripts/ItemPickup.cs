using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum WeaponType { Sword, Hammer }

    [Header("Tipo di questa arma")]
    public WeaponType weaponToEquip;

    [Header("Prefab delle armi da terra")]
    public GameObject swordPickupPrefab;  
    public GameObject hammerPickupPrefab; 

    private bool canBePickedUp = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;

        if (other.CompareTag("Player")) // Permette solo al Player di raccogliere le armi
        {
            Player_Combat combat = other.GetComponent<Player_Combat>();

            if (combat != null)
            {
                Vector3 dropPosition = other.transform.position + new Vector3(0.8f, 0f, 0f); //Posizione attuale del player, e la droppa poco più a destra

                // Spawna a terra l'arma precedente e imposta la scala corretta
                if (combat.hasSword && swordPickupPrefab != null)
                {
                    GameObject droppedItem = Instantiate(swordPickupPrefab, dropPosition, Quaternion.identity);
                    droppedItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Moifica la dimensione dell'item appena viene droppato
                    droppedItem.GetComponent<ItemPickup>().StartCoroutine(droppedItem.GetComponent<ItemPickup>().PickupCooldown());
                }
                else if (combat.hasHammer && hammerPickupPrefab != null)
                {
                    GameObject droppedItem = Instantiate(hammerPickupPrefab, dropPosition, Quaternion.identity);
                    droppedItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Moifica la dimensione dell'item appena viene droppato
                    droppedItem.GetComponent<ItemPickup>().StartCoroutine(droppedItem.GetComponent<ItemPickup>().PickupCooldown());
                }

                // Equipaggia la nuova arma
                if (weaponToEquip == WeaponType.Sword) combat.EquipSword();
                else if (weaponToEquip == WeaponType.Hammer) combat.EquipHammer();

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