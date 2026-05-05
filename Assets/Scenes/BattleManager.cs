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

    bool playerTurn = true;

    void Start() { UpdateUI(); }

    public void PlayerAttack()
    {
        if (!playerTurn) return;
        int dmg = Random.Range(10, 25);
        enemyHP -= dmg;
        statusText.text = "Napao si za " + dmg + " štete!";
        playerTurn = false;
        CheckWin();
        Invoke("EnemyTurn", 1.2f);
        UpdateUI();
    }

    public void PlayerHeal()
    {
        if (!playerTurn) return;
        int heal = Random.Range(10, 20);
        playerHP = Mathf.Min(playerHP + heal, 100);
        statusText.text = "Izliječio si " + heal + " HP!";
        playerTurn = false;
        Invoke("EnemyTurn", 1.2f);
        UpdateUI();
    }

    void EnemyTurn()
    {
        int dmg = Random.Range(8, 18);
        playerHP -= dmg;
        statusText.text = "Neprijatelj napao za " + dmg + "!";
        CheckWin();
        playerTurn = true;
        UpdateUI();
    }

    void CheckWin()
    {
        if (enemyHP <= 0) { statusText.text = "Pobijedio si!"; Invoke("GoBack", 2f); }
        if (playerHP <= 0) { statusText.text = "Izgubio si..."; Invoke("GoBack", 2f); }
    }

    void GoBack() { SceneManager.LoadScene("ExplorationScene"); }

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP;
        enemyHPText.text  = "Enemy HP: " + enemyHP;
    }
}