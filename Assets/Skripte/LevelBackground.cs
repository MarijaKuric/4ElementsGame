using UnityEngine;

public class LevelBackground : MonoBehaviour
{
    public Sprite backgroundSprite;   // postavlajmo u Inspectoru za svaki level

    void Start()
    {
        GameState.currentLevelBackground = backgroundSprite;
    }

}
