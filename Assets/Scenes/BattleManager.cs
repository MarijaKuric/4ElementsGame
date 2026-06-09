using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;



public enum AbilityType { Basic, Special, Passive }
public enum Element { Neutral, Fire, Water, Wind }

[System.Serializable]
public class Ability
{
    public string name;
    public string description;
    public AbilityType type;
    public Element element;
    public int energyCost;
    [Range(1, 3)] public int rarity;
}

public class BattleManager : MonoBehaviour
{

    public int playerHP = 100;
    public int enemyHP = 80;

    public TMP_Text statusText;
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;
    public GameObject fireEffect;
    public GameObject explosionEffect;
    public Transform playerTransform;
    public Transform enemyTransform;
    public SpriteRenderer enemySpriteRenderer;
    public float fireTravelDuration = 1.0f;
    public float explosionDuration = 3.5f;

    int maxPlayerHP;
    int maxEnemyHP;
    bool playerTurn = true;
    bool battleOver = false;
    
    int attackCounter = 0;
    Animator fireAnimator;
    Animator explosionAnimator;
    Animator doorsOpen;


    public GameObject abilityPanel;
    public GameObject inventoryPanel;
    public TMP_Text[] abilityTexts;
    public Ability[] allPossibleAbilities;
    public GameObject[] inventorySlots;

    void Start() {
        if (enemySpriteRenderer != null && GameState.currentEnemySprite != null)
        enemySpriteRenderer.sprite = GameState.currentEnemySprite;

        enemyHP = GameState.currentEnemyHP;
        maxPlayerHP = playerHP;
        maxEnemyHP = enemyHP;
        if (fireEffect != null)
        {
            fireAnimator = fireEffect.GetComponent<Animator>();
            fireEffect.SetActive(false);
        }
        if (explosionEffect != null)
        {
            explosionAnimator = explosionEffect.GetComponent<Animator>();
            explosionEffect.SetActive(false);
        }

        if (abilityPanel != null) abilityPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        statusText.text = "Ulazis u bitku";
        UpdateUI();
        }

    public void PlayerAttack()
    {
        if (!playerTurn || battleOver) return;
        attackCounter++;
        int dmg = Random.Range(10, 25);
    
        string attackMessage = "";
        if (attackCounter >= 3)
        {
            dmg += 15;
            attackMessage = "<color=yellow>CRITICAL HIT! </color>";
            attackCounter = 0;
        }
        enemyHP = Mathf.Max(enemyHP - dmg, 0);

        if (fireEffect != null)
        {
            fireEffect.SetActive(true);
            fireAnimator.SetTrigger("FireAttack");
            StartCoroutine(MoveFireEffect());
        }
        statusText.text = attackMessage + "Napao si za <color=red>-" + dmg + "</color> štete!";
        playerTurn = false;
        UpdateUI();

        if(CheckWin()) return;
        Invoke("EnemyTurn", 1.2f);
    }

    public void PlayerHeal()
    {
        if (!playerTurn || battleOver) return;
        int heal = Random.Range(10, 20);
        playerHP = Mathf.Min(playerHP + heal, maxPlayerHP);
        statusText.text = "Izliječio si <color=green>+" + heal + "</color> HP!";
        playerTurn = false;
        UpdateUI();
        Invoke("EnemyTurn", 1.2f);
    }

    void EnemyTurn()
    {
        if(battleOver) return;
        int dmg = Random.Range(GameState.currentEnemyDamageMin, GameState.currentEnemyDamageMax);
        playerHP = Mathf.Max(playerHP - dmg, 0);
        statusText.text = "Neprijatelj napao za <color=red>-" + dmg + "</color>!";
        playerTurn = true;
        UpdateUI();
        CheckWin();
    }

    bool CheckWin()
    {
        if (enemyHP <= 0) { 
            statusText.text = "Pobijedio si!"; 
            battleOver = true;
            CancelInvoke();
            Invoke("ShowAbilityRewards", 2f);
            return true;
            }
        if (playerHP <= 0) { 
            statusText.text = "Izgubio si..."; 
            battleOver = true;
            CancelInvoke();
            Invoke("GoBack", 2f); 
            return true;
            }
            return false;
    }

    IEnumerator MoveFireEffect()
    {
        Vector3 start = playerTransform.position;
        Vector3 end = enemyTransform.position;
        float elapsed = 0f;

        while (elapsed < fireTravelDuration)
        {
            elapsed += Time.deltaTime;
            fireEffect.transform.position = Vector3.Lerp(start, end, elapsed / fireTravelDuration);
            yield return null;
        }

        fireEffect.transform.position = end;
        fireEffect.SetActive(false);

        if (explosionEffect != null)
        {
            explosionEffect.transform.position = end;
            explosionEffect.SetActive(true);
            explosionAnimator.SetTrigger("Explode");
            yield return new WaitForSeconds(explosionDuration);
            explosionEffect.SetActive(false);
        }
    }
    void ShowAbilityRewards()
    {
        abilityPanel.SetActive(true);
        int[] chosenIndices = new int[3] { -1, -1, -1 };

        for (int i = 0; i < 3; i++)
        {
            Ability chosen;
            int randomIndex;
            bool isDuplicate;
            do
            {
                chosen = GetRandomAbility();
                randomIndex = System.Array.IndexOf(allPossibleAbilities, chosen);
                isDuplicate = false;
                for (int j = 0; j < i; j++) { if (chosenIndices[j] == randomIndex) { isDuplicate = true; break; } }
            } while (isDuplicate);

            chosenIndices[i] = randomIndex;
            abilityTexts[i].text = $"Moć: {chosen.name}\n{chosen.description}";
        }
    }

    Ability GetRandomAbility()
    {
        int attempts = 0;
        while (attempts < 100)
        {
            Ability a = allPossibleAbilities[Random.Range(0, allPossibleAbilities.Length)];
            if (InventoryManager.Instance.HasElement(a.element) && Random.Range(1, 4) <= a.rarity) return a;
            attempts++;
        }
        return allPossibleAbilities[0];
    }

    public void SelectAbility(int index)
    {
        string name = abilityTexts[index].text.Split('\n')[0].Replace("Moć: ", "").Trim();
        Ability chosen = System.Array.Find(allPossibleAbilities, a => a.name == name);
        
        InventoryManager.Instance.AddAbility(chosen);
        if (GameState.currentEnemyIsBoss){
            InventoryManager.Instance.UnlockElement(GameState.currentBossElement);
        }
        abilityPanel.SetActive(false);
        GoBack();
    }

    public void UseAbility(int index)
    {
        Ability a = InventoryManager.Instance.playerInventory[index];
        if (a == null) return;

        if (a.type == AbilityType.Special && GameState.currentEnergy < a.energyCost)
        {
            statusText.text = "Nedovoljno energije za ability!";
            return;
        }

        if (a.type == AbilityType.Special) GameState.currentEnergy -= a.energyCost;
        statusText.text = "Koristis: " + a.name;
        ToggleInventory();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }
    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf) UpdateInventoryUI();
        }
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Ability a = InventoryManager.Instance.playerInventory[i];
            if (a != null)
            {
                inventorySlots[i].SetActive(true);
                inventorySlots[i].GetComponentInChildren<TMP_Text>().text = a.name;
            }
            else { inventorySlots[i].SetActive(false); }
        }
    }
    

    void GoBack() {
        GameState.playerWonLastBattle = true;
        SceneManager.LoadScene("ExplorationScene");
    }

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP + "/" + maxPlayerHP;
        enemyHPText.text  = "Enemy HP: " + enemyHP + "/" + maxEnemyHP;
    }
}