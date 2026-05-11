using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{

    public int playerHP = 100;
    public int enemyHP = 80;

    public TMP_Text statusText;
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;

    int maxPlayerHP;
    int maxEnemyHP;
    bool playerTurn = true;
    bool battleOver = false;

    void Start() { 
        maxPlayerHP = playerHP;
        maxEnemyHP = enemyHP;
        statusText.text = "Ulazis u bitku";
        UpdateUI(); 
        }

    public void PlayerAttack()
    {
        if (!playerTurn || battleOver) return;
        int dmg = Random.Range(10, 25);
        enemyHP -= dmg;
        statusText.text = "Napao si za " + dmg + " štete!";
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
        statusText.text = "Izliječio si " + heal + " HP!";
        playerTurn = false;
        UpdateUI();
        Invoke("EnemyTurn", 1.2f);
    }

    void EnemyTurn()
    {
        if(battleOver) return;
        int dmg = Random.Range(8, 18);
        playerHP -= dmg;
        statusText.text = "Neprijatelj napao za " + dmg + "!";
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
            Invoke("GoBack", 2f); 
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