using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Per-level background clips")]
    public AudioClip[] fireClips;   // Level1
    public AudioClip[] airClips;    // Level2
    public AudioClip[] earthClips;  // Level3
    public AudioClip[] waterClips;  // Level4
    public AudioClip[] finalClips;  // Level5 (empty for now)

    [Header("Battle music (looping, played on BattleScene)")]
    public AudioClip battleMusic;

    [Header("Timing (seconds)")]
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;
    public float minPause = 10f;
    public float maxPause = 15f;

    [Range(0f, 1f)]
    public float musicVolume = 0.3f;

    AudioSource audioSource;
    Coroutine playlistRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0f;

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartForScene(scene.name);
    }

    void StartForScene(string sceneName)
    {
        if (playlistRoutine != null)
        {
            StopCoroutine(playlistRoutine);
            playlistRoutine = null;
        }

        // Battle keeps its own looping track (as before), no fade / no pauses.
        if (sceneName == "BattleScene")
        {
            PlayBattleMusic();
            return;
        }

        AudioClip[] clips = GetClipsForScene(sceneName);

        // A level with no clips (Level5) or any other scene: stop the music.
        if (clips == null || clips.Length == 0)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.volume = 0f;
            return;
        }

        audioSource.loop = false;
        playlistRoutine = StartCoroutine(PlaylistLoop(clips));
    }

    void PlayBattleMusic()
    {
        if (battleMusic == null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.volume = 0f;
            return;
        }

        audioSource.loop = true;
        audioSource.clip = battleMusic;
        audioSource.volume = musicVolume;
        audioSource.Play();
    }

    AudioClip[] GetClipsForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1": return fireClips;
            case "Level2": return airClips;
            case "Level3": return earthClips;
            case "Level4": return waterClips;
            case "Level5(final level)": return finalClips;
            default: return null; // BattleScene, intro screens, etc.
        }
    }

    IEnumerator PlaylistLoop(AudioClip[] clips)
    {
        int lastIndex = -1;

        while (true)
        {
            int index = PickIndex(clips.Length, lastIndex);
            lastIndex = index;

            AudioClip clip = clips[index];
            if (clip == null)
            {
                yield return null;
                continue;
            }

            audioSource.clip = clip;
            audioSource.volume = 0f;
            audioSource.Play();

            yield return StartCoroutine(FadeVolume(0f, musicVolume, fadeInDuration));

            float bodyTime = clip.length - fadeInDuration - fadeOutDuration;
            if (bodyTime > 0f)
                yield return new WaitForSeconds(bodyTime);

            yield return StartCoroutine(FadeVolume(audioSource.volume, 0f, fadeOutDuration));

            audioSource.Stop();

            float pause = Random.Range(minPause, maxPause);
            yield return new WaitForSeconds(pause);
        }
    }

    // Random clip, avoiding the same one twice in a row when there is more than one.
    int PickIndex(int count, int lastIndex)
    {
        if (count <= 1) return 0;

        int index = Random.Range(0, count);
        if (index == lastIndex)
            index = (index + 1) % count;
        return index;
    }

    IEnumerator FadeVolume(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            audioSource.volume = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        audioSource.volume = to;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
