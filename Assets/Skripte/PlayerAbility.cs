using UnityEngine;

[System.Serializable]
public class PlayerAbility
{
    public AbilityData data;
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;

    public const int MaxLevel = 5;

    public PlayerAbility(AbilityData abilityData)
    {
        data = abilityData;
        level = 1;
        currentXP = 0;
        xpToNextLevel = 10;
    }

    public int GetDamage() => data.baseDamage + data.damagePerLevel * (level - 1);

    public int GetPassiveValue() => data.passiveValue + data.passiveValuePerLevel * (level - 1);

    public float GetPassiveChance() => Mathf.Clamp01(data.passiveChance + 0.02f * (level - 1));
    public bool AddXP(int amount)
    {
        if (level >= MaxLevel) return false;
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            level++;
            currentXP = 0;
            xpToNextLevel = level * 15;
            return true;
        }
        return false;
    }

    public float GetXPProgress() => (float)currentXP / xpToNextLevel;
}
