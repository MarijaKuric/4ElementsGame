using UnityEngine;
using UnityEngine.SceneManagement;

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
        // sinkroniziranje levela sa scenom u kojoj se nalazimo
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level"))
            int.TryParse(sceneName.Replace("Level", ""), out GameState.currentLevel);

        // handle vracanje iz bitke
        if (GameState.currentEnemy != null)
        {
            if (GameState.playerWonLastBattle)
            {
                Destroy(GameState.currentEnemy);

                // Provjera stanja nakon bitke
                if (GameState.enemiesRemaining == 0 && !GameState.bossSpawned)
                {
                    SpawnBoss();
                }
                else if (GameState.enemiesRemaining == 0 && GameState.bossSpawned && GameState.bossDefeated)
                {
                    LoadNextLevel();
                }
            }
            else
            {
                // player izgubio ili pobjegao — vrati enemija
                GameState.currentEnemy.SetActive(true);
            }

            GameState.currentEnemy = null;
        }

        GameState.justFinishedBattle = false; 

        // spawnanje pocetnih neprijatelja
        if (!GameState.initialSpawnDone)
        {
            Invoke("SpawnInitialEnemies", firstSpawnDelay);
        }
    }
    

    void SpawnInitialEnemies(){
        SpawnGroup(enemyPrefabLow, lowCount);
        SpawnGroup(enemyPrefabMid, midCount);
        SpawnGroup(enemyPrefabHigh, highCount);
        
        GameState.initialSpawnDone = true;
        GameState.enemiesRemaining = GameState.activeEnemies.Count;
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

    void LoadNextLevel()
    {
        GameState.currentLevel++;
        GameState.initialSpawnDone = false;
        GameState.bossSpawned = false;
        GameState.bossDefeated = false;
        GameState.activeEnemies.Clear();
        GameState.enemiesRemaining = 0;
        GameState.currentEnemy = null;
        Debug.Log("Boss poražen! Prelaz na Level " + GameState.currentLevel);
        SceneManager.LoadScene("Level" + GameState.currentLevel);
        
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