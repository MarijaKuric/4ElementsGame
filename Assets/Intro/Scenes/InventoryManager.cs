using UnityEngine;

public enum AbilityType { Basic, Special, Passive }
public enum Element { Neutral, Fire, Water, Wind }

public class Ability
{
    public string name;
    public string description;
    public AbilityType type;
    public Element element;
    public int energyCost;
    public int rarity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public Ability[] playerInventory = new Ability[14];
    public Element[] unlockedElements = new Element[5];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            unlockedElements[0] = Element.Neutral;
            unlockedElements[1] = Element.Fire;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddAbility(Ability ability)
    {
        for (int i = 0; i < playerInventory.Length; i++)
        {
            if (playerInventory[i] == null)
            {
                playerInventory[i] = ability;
                break;
            }
        }
    }

    public bool HasElement(Element e)
    {
        for (int i = 0; i < unlockedElements.Length; i++)
        {
            if (unlockedElements[i] == e) return true;
        }
        return false;
    }

    public void UnlockElement(Element e)
    {
        for (int i = 0; i < unlockedElements.Length; i++)
        {
            if (unlockedElements[i] == default(Element) && !HasElement(e))
            {
                unlockedElements[i] = e;
                break;
            }
        }
    }
}