using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            EnemyStats stats = GetComponent<EnemyStats>();

            if(stats != null){
                GameState.currentEnemyHP = stats.hp;
                GameState.currentEnemyDamageMin = stats.damageMin;
                GameState.currentEnemyDamageMax = stats.damageMax;
                GameState.currentBossElement = stats.element;
                GameState.currentEnemyIsBoss = stats.isBoss;
            }

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) GameState.currentEnemySprite = sr.sprite;

            Vector2 pushDir = (col.transform.position - transform.position).normalized;
            GameState.playerReturnPosition = col.transform.position + (Vector3)(pushDir * 2f);
            GameState.currentEnemy = this.gameObject;
            GameState.activeEnemies.Remove(this.gameObject);
            this.gameObject.SetActive(false);

            SceneManager.LoadScene("BattleScene");
        }
    }
}