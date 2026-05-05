using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger1 : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Battle start!");
            SceneManager.LoadScene("BattleScene");
        }
    }
}