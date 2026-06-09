using UnityEngine;

// Easy:  hp=40,  damageMin=5,  damageMax=10
// Mid:   hp=70,  damageMin=10, damageMax=18
// Hard:  hp=100, damageMin=15, damageMax=25
// Boss:  hp=200, damageMin=20, damageMax=35
public class EnemyStats : MonoBehaviour
{
    public int hp = 40;
    public int damageMin = 5;
    public int damageMax = 18;
    public Element element = Element.Neutral;
    public bool isBoss = false;
}
