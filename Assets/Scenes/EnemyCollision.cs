using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            EnemyStats stats = GetComponent<EnemyStats>();

            if(stats != null){
                GameState.currentEnemyHP = stats.hp;
                GameState.currentEnemyDamageMin = stats.damageMin;
                GameState.currentEnemyDamageMax = stats.damageMax;
            }

            GameState.currentEnemy = this.gameObject;
            this.gameObject.SetActive(false);

            SceneManager.LoadScene("BattleScene");
        }
    }
}