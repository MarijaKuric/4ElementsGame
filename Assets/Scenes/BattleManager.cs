using UnityEngine;
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
    public float fireTravelDuration = 1.0f;
    public float explosionDuration = 3.5f;

    int maxPlayerHP;
    int maxEnemyHP;
    bool playerTurn = true;
    bool battleOver = false;
    Animator fireAnimator;
    Animator explosionAnimator;

    void Start() {
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
        statusText.text = "Ulazis u bitku";
        UpdateUI();
        }

    public void PlayerAttack()
    {
        if (!playerTurn || battleOver) return;
        int dmg = Random.Range(10, 25);
        enemyHP = Mathf.Max(enemyHP - dmg, 0);
        if (fireEffect != null)
        {
            fireEffect.SetActive(true);
            fireAnimator.SetTrigger("FireAttack");
            StartCoroutine(MoveFireEffect());
        }
        statusText.text = "Napao si za <color=red>-" + dmg + "</color> štete!";
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
        int dmg = Random.Range(8, 18);
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

    void GoBack() {
        GameState.justFinishedBattle = true;
        SceneManager.LoadScene("ExplorationScene");
     }

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP + "/" + maxPlayerHP;
        enemyHPText.text  = "Enemy HP: " + enemyHP + "/" + maxEnemyHP;
    }
}