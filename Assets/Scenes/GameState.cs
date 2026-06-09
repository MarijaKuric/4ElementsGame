using UnityEngine;
using System.Collections.Generic;

public static class GameState
{
    public static bool justFinishedBattle = false; // provjera dali je bitka zavsena
    public static GameObject currentEnemy = null; // provjera ako postoji interakcija sa neprijateljem
    public static bool playerWonLastBattle = false; // provjera ako j ezadnja bitka pobjedena

    // pocetni stats neprijatelja
    public static int currentEnemyHP = 80;
    public static int currentEnemyDamageMin = 8;
    public static int currentEnemyDamageMax = 18;
    public static Element currentBossElement = Element.Fire;
    public static bool currentEnemyIsBoss = false;

    //prosljedujemo u battlescene kojeg enemy-a smo "napali"
    public static Sprite currentEnemySprite;

    // player energy
    public static int currentEnergy = 100;
    public static int maxEnergy = 100;

    // world state
    public static List<GameObject> activeEnemies = new List<GameObject>();
    public static bool initialSpawnDone = false;
    public static bool bossSpawned = false;
    public static bool bossDefeated = false;
}
