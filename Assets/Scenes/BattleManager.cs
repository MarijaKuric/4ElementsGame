using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


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
    public SpriteRenderer battleBackgroundRenderer;

    int maxPlayerHP;
    int maxEnemyHP;
    bool playerTurn = true;
    bool battleOver = false;
    int attackCounter = 0;

    private int fireAttackCooldown = 0;
    private int healCooldown = 0;

    Animator fireAnimator;
    Animator explosionAnimator;

    public Image hpBarFill;
    public Image energyBarFill;

    public GameObject abilityPanel;
    public GameObject inventoryPanel;
    public TMP_Text[] abilityTexts;
    public AbilityData[] allPossibleAbilities;
    public GameObject[] inventorySlots;

    void Start()
    {
        if (enemySpriteRenderer != null && GameState.currentEnemySprite != null)
        enemySpriteRenderer.sprite = GameState.currentEnemySprite;

        if (battleBackgroundRenderer != null && GameState.currentLevelBackground != null)
        battleBackgroundRenderer.sprite = GameState.currentLevelBackground;

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

        // provjera cooldown-a
        if (fireAttackCooldown > 0)
        {
            statusText.text = "Fire Attack nije spreman još <color=orange>" + fireAttackCooldown + "</color> poteza!";
            return;
        }

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
        fireAttackCooldown = 1;

        UpdateUI();
        if (CheckWin()) return;
        Invoke("EnemyTurn", 1.2f);
    }

    public void PlayerHeal()
    {
        if (!playerTurn || battleOver) return;

        // provjera cooldown-a
        if (healCooldown > 0)
        {
            statusText.text = "Heal nije spreman još <color=orange>" + healCooldown + "</color> poteza!";
            return;
        }

        int heal = Random.Range(10, 20);
        playerHP = Mathf.Min(playerHP + heal, maxPlayerHP);
        statusText.text = "Izliječio si <color=green>+" + heal + "</color> HP!";
        playerTurn = false;
        healCooldown = 2;

        UpdateUI();
        Invoke("EnemyTurn", 1.2f);
    }

    void EnemyTurn()
    {
        if (battleOver) return;

        // smanjicanje cooldown-a
        if (fireAttackCooldown > 0) fireAttackCooldown--;
        if (healCooldown > 0) healCooldown--;

        int dmg = Random.Range(GameState.currentEnemyDamageMin, GameState.currentEnemyDamageMax);
        playerHP = Mathf.Max(playerHP - dmg, 0);
        statusText.text = "Neprijatelj napao za <color=red>-" + dmg + "</color>!";
        playerTurn = true;
        UpdateUI();
        CheckWin();
    }

    bool CheckWin()
    {
        if (enemyHP <= 0)
        {
            statusText.text = "Pobijedio si!";
            battleOver = true;
            CancelInvoke("EnemyTurn");
            GameState.playerWonLastBattle = true;
            GameState.justFinishedBattle = true;
            if (GameState.currentEnemyIsBoss){
                GameState.bossDefeated = true;
                Invoke("ShowAbilityRewards", 2f);
            }else{
                GameState.enemiesRemaining--;
                Invoke("GoBack", 2f);
            }
            return true;
        }
        if (playerHP <= 0)
        {
            statusText.text = "Izgubio si...";
            battleOver = true;
            CancelInvoke();
            GameState.playerWonLastBattle = false;
            GameState.justFinishedBattle = true;
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

        if (abilityPanel == null) { Debug.LogError("abilityPanel NULL"); GoBack(); return; }
        if (abilityTexts == null || abilityTexts.Length < 3) { Debug.LogError("abilityTexts < 3"); GoBack(); return; }
        if (allPossibleAbilities == null || allPossibleAbilities.Length == 0) { Debug.LogError("allPossibleAbilities prazan"); GoBack(); return; }


        abilityPanel.SetActive(true);
        int[] chosenIndices = new int[3] { -1, -1, -1 };
        for (int i = 0; i < 3; i++)
        {
            AbilityData chosen;
            int randomIndex;
            bool isDuplicate;
            int safetyCounter = 0;

            do
            {
                chosen = GetRandomAbility();
                randomIndex = System.Array.IndexOf(allPossibleAbilities, chosen);
                isDuplicate = false;
                for (int j = 0; j < i; j++){
                    if (chosenIndices[j] == randomIndex) { isDuplicate = true; break; }
                }

            safetyCounter++;
            if (safetyCounter > 200)
            
            {
                Debug.LogWarning("Nema dovoljno unique abilitya za sve 3 nagrade, koristim fallback.");
                break;
            }

            } while (isDuplicate);
            chosenIndices[i] = randomIndex;
            abilityTexts[i].text = $"Moć: {chosen.abilityName}\n{chosen.description}";
        }
    }

    AbilityData GetRandomAbility()
{
    if (allPossibleAbilities == null || allPossibleAbilities.Length == 0) return null;

    int attempts = 0;
    while (attempts < 100)
    {
        AbilityData a = allPossibleAbilities[Random.Range(0, allPossibleAbilities.Length)];
        if (InventoryManager.Instance.HasElement(a.element) && Random.Range(1, 4) <= a.rarity)
            return a;
        attempts++;
    }
    return allPossibleAbilities[0];
}

    public void SelectAbility(int index)
    {
        string name = abilityTexts[index].text.Split('\n')[0].Replace("Moć: ", "").Trim();
        AbilityData chosen = System.Array.Find(allPossibleAbilities, a => a.abilityName == name);
        InventoryManager.Instance.AddAbility(chosen);
        if (GameState.currentEnemyIsBoss)
            InventoryManager.Instance.UnlockElement(GameState.currentBossElement);
        abilityPanel.SetActive(false);
        GoBack();
    }

    public void UseAbility(int index)
    {
        if (!playerTurn || battleOver) return;

        PlayerAbility a = InventoryManager.Instance.GetAbilityAt(index);
        if (a == null)
        {
            statusText.text = "Nema abilitya na tom slotu!";
            return;
        }

        if (a.data.type == AbilityType.Special && GameState.currentEnergy < a.data.energyCost)
        {
            statusText.text = $"Nedovoljno energije! Trebaš <color=cyan>{a.data.energyCost}</color>, imaš <color=cyan>{GameState.currentEnergy}</color>.";
            return;
        }

        if (a.data.type == AbilityType.Special)
            GameState.currentEnergy -= a.data.energyCost;

        int dmg = a.GetDamage();
        enemyHP = Mathf.Max(enemyHP - dmg, 0);

        statusText.text = $"<color=cyan>{a.data.abilityName}</color> napravio <color=red>-{dmg}</color> štete!";

        playerTurn = false;
        if (inventoryPanel != null && inventoryPanel.activeSelf)
            inventoryPanel.SetActive(false);

        UpdateUI();
        if (CheckWin()) return;
        Invoke("EnemyTurn", 1.2f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();
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
            PlayerAbility a = InventoryManager.Instance.GetAbilityAt(i);
            if (a != null)
            {
                inventorySlots[i].SetActive(true);
                inventorySlots[i].GetComponentInChildren<TMP_Text>().text =
                    $"{a.data.abilityName} (Lv{a.level})";
            }
            else inventorySlots[i].SetActive(false);
        }
    }

    void GoBack()
    {
        SceneManager.LoadScene("Level" + GameState.currentLevel);
    }

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP + "/" + maxPlayerHP;
        enemyHPText.text = "Enemy HP: " + enemyHP + "/" + maxEnemyHP;

        if (hpBarFill != null)
            hpBarFill.fillAmount = (float)playerHP / maxPlayerHP;

        if (energyBarFill != null)
            energyBarFill.fillAmount = (float)GameState.currentEnergy / GameState.maxEnergy;
    }
}