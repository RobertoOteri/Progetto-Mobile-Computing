using System.Collections.Generic;

[System.Serializable]
public class EnemySaveData
{
    public string enemyID;
    public float posX;
    public float posY;
    public int currentHealth;
    public bool isDead;
}

[System.Serializable]
public class DroppedWeaponSaveData
{
    public int weaponType;
    public float posX;
    public float posY;
}

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public float playerPosX;
    public float playerPosY;
    public int currentHealth;
    public int maxHealth;
    public int storedWeapon;
    public bool isWeaponDrawn;

    public List<EnemySaveData> enemiesData = new List<EnemySaveData>();
    public List<DroppedWeaponSaveData> droppedWeapons = new List<DroppedWeaponSaveData>();

    // Lista degli ID di mele e pozioni già consumate
    public List<string> consumedItems = new List<string>();
}