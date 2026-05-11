using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    [Header("Battle UI")]
    public GameObject battleUI;        // prazan GameObject koji drži sve UI elemente
    public TMP_Text statusText;
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;
    public Button attackButton;
    public Button healButton;

    [Header("Stats")]
    public int playerHP = 100;
    public int enemyHP = 80;

    bool inBattle = false;
    bool playerTurn = true;
    bool playerWon = false;
    GameObject player;

    void Start()
    {
        battleUI.SetActive(false);     // UI skriven na početku
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && !inBattle)
        {
            player = col.gameObject;
            StartBattle();
        }
    }

    void StartBattle()
    {
        inBattle = true;
        battleUI.SetActive(true);

        // zaustavi igrača
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player.GetComponent<PlayerMovement>().enabled = false;

        attackButton.onClick.AddListener(PlayerAttack);
        healButton.onClick.AddListener(PlayerHeal);

        UpdateUI();
        statusText.text = "Neprijatelj se pojavio!";
    }

    public void PlayerAttack()
    {
        if (!playerTurn) return;
        int dmg = Random.Range(10, 25);
        enemyHP -= dmg;
        statusText.text = "Napao si za " + dmg + " štete!";
        playerTurn = false;
        CheckWin();
        if (inBattle) Invoke("EnemyTurn", 1.2f);
        UpdateUI();
    }

    public void PlayerHeal()
    {
        if (!playerTurn) return;
        int heal = Random.Range(10, 20);
        playerHP = Mathf.Min(playerHP + heal, 100);
        statusText.text = "Izliječio si " + heal + " HP!";
        playerTurn = false;
        if (inBattle) Invoke("EnemyTurn", 1.2f);
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
        if (enemyHP <= 0)
        {
            statusText.text = "Pobijedio si!";
            playerWon = true;
            inBattle = false;
            CancelInvoke();
            Invoke("EndBattle", 2f);
        }
        else if (playerHP <= 0)
        {
            statusText.text = "Izgubio si...";
            inBattle = false;
            CancelInvoke();
            Invoke("EndBattle", 2f);
        }
    }

    void EndBattle()
    {
        battleUI.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;

        if (playerWon)
            Destroy(gameObject);
    }

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP;
        enemyHPText.text = "Enemy HP: " + enemyHP;
    }
}