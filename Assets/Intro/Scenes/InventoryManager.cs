using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<PlayerAbility> allAbilities = new List<PlayerAbility>();
    public List<PlayerAbility> activeDeck = new List<PlayerAbility>();
    public List<Element> unlockedElements = new List<Element>();

    public const int MaxInventorySize = 20;
    public const int MaxDeckSize = 4;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            unlockedElements.Add(Element.Neutral);
            unlockedElements.Add(Element.Fire);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Vraca true ako je ability dobila level up (duplikat = bonus XP)
    public bool AddAbility(AbilityData data)
    {
        if (data == null) return false;

        PlayerAbility existing = allAbilities.Find(a => a.data == data);
        if (existing != null)
            return existing.AddXP(20);

        if (allAbilities.Count >= MaxInventorySize) return false;

        allAbilities.Add(new PlayerAbility(data));
        return false;
    }

    public bool AddToDeck(PlayerAbility ability)
    {
        if (activeDeck.Count >= MaxDeckSize) return false;
        if (activeDeck.Contains(ability)) return false;
        activeDeck.Add(ability);
        return true;
    }

    public void RemoveFromDeck(PlayerAbility ability)
    {
        activeDeck.Remove(ability);
    }

    // Pomocna metoda za BattleManager - dohvaca ability po indeksu iz allAbilities
    public PlayerAbility GetAbilityAt(int index)
    {
        if (index < 0 || index >= allAbilities.Count) return null;
        return allAbilities[index];
    }

    public bool HasElement(Element e) => unlockedElements.Contains(e);

    public void UnlockElement(Element e)
    {
        if (!unlockedElements.Contains(e))
            unlockedElements.Add(e);
    }
}
