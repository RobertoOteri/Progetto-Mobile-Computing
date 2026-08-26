using UnityEngine;

public class WeaponPickupSaveable : MonoBehaviour
{
    [Header("Tipo di Arma di questo Pickup")]
    public WeaponType weaponType;

    public DroppedWeaponSaveData GetSaveData()
    {
        DroppedWeaponSaveData data = new DroppedWeaponSaveData();
        data.weaponType = (int)weaponType;
        data.posX = transform.position.x;
        data.posY = transform.position.y;
        return data;
    }
}