using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public int minEnemies = 1;
    public int maxEnemies = 5;

    public Vector2 SpawnAreaMin = new Vector2(-8, -4);
    public Vector2 SpawnAreaMax = new Vector2(8, 4);

    //prikazano u inspectoru za lakse mjenjanje
    public float firstSpawnDelay = 10f;
    public float AfterBattleSpawnDelay = 3f;

    void Start()
    {
        
        //gameObject.SetActive(false);           // neprijatelj nevidljiv na početku
        //Invoke("ShowEnemy", 10f);              // pojavi se nakon 10 sekundi

        if(GameState.justFinishedBattle){ 
            // vracanje is battle-a, spawnanje novih enemy-a
            GameState.justFinishedBattle = false;
            Invoke("SpawnEnemies", AfterBattleSpawnDelay);
        }else{ 
            // fresh pocetak, cekanje 10 sekundi prije spawn-a
            Invoke("SpawnEnemies", firstSpawnDelay);
        }
    }

    public void SpawnEnemies(){
        int count = Random.Range(minEnemies, maxEnemies + 1);

        for(int i = 0; i < count; i++){
            //coordinate za spawn enemy-a
            float x = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
            float y = Random.Range(SpawnAreaMin.y, SpawnAreaMax.y);

            Vector3 spawnPosition = new Vector3(x, y, 0);

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        Debug.Log("Spawned " + count + " enemies");
    }

    void ShowEnemy()
    {
        gameObject.SetActive(true);
    }
}