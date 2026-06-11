using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    public GameObject burnOverlay;
    public Image burnImage;
    public float burnDuration = 1.8f;
    public string nextScene = "CutsceneIntro";

    public void OnStartPressed()
    {
        StartCoroutine(BurnTransition());
    }

   IEnumerator BurnTransition()
{
    burnOverlay.SetActive(true);
    float t = 0f;
    Color c = burnImage.color;

    while (t < burnDuration)
    {
        t += Time.deltaTime;
        c.a = Mathf.Clamp01(t / burnDuration);
        burnImage.color = c;
        yield return null;
    }

    SceneManager.LoadScene(1); // 1 = INTRO ANIMATION in your build settings
}
}