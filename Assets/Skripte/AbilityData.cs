using UnityEngine;

public enum AbilityType { Basic, Special, Passive }
public enum Element { Neutral, Fire, Water, Wind }

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
}
