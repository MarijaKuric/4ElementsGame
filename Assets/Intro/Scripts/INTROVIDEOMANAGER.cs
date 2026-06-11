using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class INTROVIDEOMANAGER : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public Button skipButton;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        skipButton.onClick.AddListener(LoadNextScene);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Level1");
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}