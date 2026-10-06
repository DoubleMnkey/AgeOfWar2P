using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public MenuManager.MenuState currentMenu =
        MenuManager.MenuState.Main;

    public int selectedIndex = 0;

    public int age = 1;

    public int exp = 0;

    public int gold = 1000;

    public int currentHealth = 500;

    public int maxHealth = 500;

    public int unlockedSlots = 0;
}