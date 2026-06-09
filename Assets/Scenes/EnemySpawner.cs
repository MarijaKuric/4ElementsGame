using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Enemy Prefabs")]
    public GameObject enemyPrefabLow;
    public GameObject enemyPrefabMid;
    public GameObject enemyPrefabHigh;
    public GameObject bossPrefab;

    [Header("Enemy Spawn Counts")]
    public int lowCount = 2;
    public int midCount = 2;
    public int highCount = 2;

    [Header ("Enemy Spawn Area")]
    public Vector2 SpawnAreaMin = new Vector2(-18, -10);
    public Vector2 SpawnAreaMax = new Vector2(19, 0);

    [Header("Timing Spawn")]
    //prikazano u inspectoru za lakse mjenjanje
    public float firstSpawnDelay = 5f;

    void Start()
    {
        // handle vracanje iz bitke
        if(GameState.currentEnemy != null){

            if(GameState.playerWonLastBattle){ 
            GameState.activeEnemies.Remove(GameState.currentEnemy);
            Destroy(GameState.currentEnemy);

            // provjera ako su svi enemy porazeni spawn-aj boss-a
            if(GameState.activeEnemies.Count == 0 && !GameState.bossSpawned){
                SpawnBoss();
            }else if (GameState.activeEnemies.Count == 0 && GameState.bossSpawned){ 
                GameState.bossDefeated = true;
            }
        }else{
            //ako je player pobjego ili izgubio vrati enemy
            GameState.currentEnemy.SetActive(true);
        }
        GameState.currentEnemy = null;
        }
        GameState.justFinishedBattle = false;

        if(!GameState.initialSpawnDone){
            Invoke("SpawnInitialEnemies", firstSpawnDelay);
        }
    }

    void SpawnInitialEnemies(){
        SpawnGroup(enemyPrefabLow, lowCount);
        SpawnGroup(enemyPrefabMid, midCount);
        SpawnGroup(enemyPrefabHigh, highCount);
        
        GameState.initialSpawnDone = true;
        Debug.Log("Spawned " + GameState.activeEnemies.Count + " enemies");
    }

    void SpawnGroup(GameObject prefab, int count){
        if(prefab == null) return;

        for(int i = 0; i < count; i++){
            float x = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
            float y = Random.Range(SpawnAreaMin.y, SpawnAreaMax.y);
            Vector3 spawnPosition = new Vector3(x, y, 0);

            GameObject newEnemy = Instantiate(prefab, spawnPosition, Quaternion.identity); //Quaternion.identity (objekt se spawna u default direkciji 0,0,0)
            DontDestroyOnLoad(newEnemy);
            GameState.activeEnemies.Add(newEnemy);
        }
    }

     void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab not assigned!");
            return;
        }
        
        float x = Random.Range(SpawnAreaMin.x, SpawnAreaMax.x);
        float y = Random.Range(SpawnAreaMin.y, SpawnAreaMax.y);
        Vector3 spawnPosition = new Vector3(x, y, 0);
        
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        DontDestroyOnLoad(boss);
        GameState.activeEnemies.Add(boss);
        GameState.bossSpawned = true;
        
        Debug.Log("Boss has appeared!");
    }
}