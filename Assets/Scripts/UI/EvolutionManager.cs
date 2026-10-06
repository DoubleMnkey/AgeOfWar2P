using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [Header("시대별 기지 최대 체력 (1시대 ~ 5시대)")]
    [SerializeField] private int[] eraMaxHealths = new int[] { 500, 1000, 2000, 3500, 5000 };

    [Header("시대별 필요 진화 경험치 (1시대 -> 2시대 ~ 4시대 -> 5시대)")]
    [SerializeField] private int[] eraEvolutionExps = new int[] { 400, 1000, 2000, 4000 };

    public bool IsMaxAge(PlayerData player)
    {
        return player.age >= eraMaxHealths.Length;
    }

    public bool Evolve(PlayerData player, BaseHealth baseHealth = null)
    {
        if (IsMaxAge(player))
        {
            return false; 
        }

        int requiredExp = GetRequiredExp(player.age);
        if (player.exp < requiredExp)
        {
            return false; 
        }

        int oldMax = player.maxHealth;

        player.age++;

        int newMaxHealth = GetMaxHealthForAge(player.age);
        player.maxHealth = newMaxHealth;

        int increase = newMaxHealth - oldMax;
        player.currentHealth += increase;
        if (player.currentHealth > player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
        }

        if (baseHealth != null)
        {
            baseHealth.SetEraMaxHealth(newMaxHealth);
        }

        return true; 
    }

    private int GetRequiredExp(int age)
    {
        int index = age - 1; 
        if (eraEvolutionExps != null && index >= 0 && index < eraEvolutionExps.Length)
            return eraEvolutionExps[index];
        return 0;
    }

    private int GetMaxHealthForAge(int age)
    {
        int index = age - 1;
        if (eraMaxHealths != null && index >= 0 && index < eraMaxHealths.Length)
            return eraMaxHealths[index];
        
        return GameDatabase.MaxHealth[Mathf.Clamp(index, 0, GameDatabase.MaxHealth.Length - 1)];
    }
}