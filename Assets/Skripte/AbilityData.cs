using UnityEngine;

public enum AbilityType { Basic, Special, Passive }
public enum Element { Neutral, Fire, Water, Wind }

public enum PassiveEffect
{
    None,
    DamageReduction,
    EnergyRegen,
    BurnBonusDamage,
    CritHeal,
    RegenHeal,
    BlockChance,
    DodgeChance,
    FirstStrikeBonus
}

[CreateAssetMenu(fileName = "NewAbility", menuName = "4Elements/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public string description;
    public AbilityType type;
    public Element element;
    public int energyCost;
    [Range(1, 3)] public int rarity;
    public int baseDamage;
    public int damagePerLevel;
    public Sprite icon;

    [Header("Passive Only")]
    public PassiveEffect passiveEffect = PassiveEffect.None;
    [Range(0f, 1f)] public float passiveChance = 0f;
    public int passiveValue = 0;
    public int passiveValuePerLevel = 0;
}
