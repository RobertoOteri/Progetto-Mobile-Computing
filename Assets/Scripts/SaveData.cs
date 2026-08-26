using System;

[Serializable]
public class SaveData
{
    // Scena
    public string sceneName = "Intro";

    // Posizione Player
    public float playerPosX;
    public float playerPosY;

    // Salute
    public int currentHealth = 5;
    public int maxHealth = 5;

    // Equipaggiamento Armi (corrisponde all'enum WeaponType)
    public int storedWeapon = 5; // 5 = None
    public bool isWeaponDrawn = false;
}