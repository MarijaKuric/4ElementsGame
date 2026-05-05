using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);           // neprijatelj nevidljiv na početku
        Invoke("ShowEnemy", 10f);              // pojavi se nakon 10 sekundi
    }

    void ShowEnemy()
    {
        gameObject.SetActive(true);
    }
}