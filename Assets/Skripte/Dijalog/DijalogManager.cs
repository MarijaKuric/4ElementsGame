using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DijalogManager : MonoBehaviour
{
    public static DijalogManager Instance;

    public GameObject dijalogPanel;
    public TMP_Text speakerNameText;
    public TMP_Text dijalogText;
    public Image portraitImage;

    public float typingSpeed = 0.02f;

    DijalogLine[] currentLines;
    int currentIndex;
    bool isTyping;
    System.Action onDijalogComplete;

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

        if (dijalogPanel != null)
            dijalogPanel.SetActive(false);
    }

    void Update()
    {
        if (dijalogPanel != null && dijalogPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                AdvanceDijalog();
        }
    }

    public void StartDijalog(DijalogData data, System.Action onComplete = null)
    {
        if (data == null || data.lines == null || data.lines.Length == 0) return;

        currentLines = data.lines;
        currentIndex = 0;
        onDijalogComplete = onComplete;

        if (dijalogPanel != null) dijalogPanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        DijalogLine line = currentLines[currentIndex];

        if (speakerNameText != null) speakerNameText.text = line.speakerName;
        if (portraitImage != null)
        {
            portraitImage.sprite = line.portrait;
            portraitImage.enabled = line.portrait != null;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine(line.text));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        dijalogText.text = "";
        foreach (char c in text)
        {
            dijalogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void AdvanceDijalog()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dijalogText.text = currentLines[currentIndex].text;
            isTyping = false;
            return;
        }

        currentIndex++;
        if (currentIndex >= currentLines.Length)
        {
            EndDijalog();
        }
        else
        {
            ShowLine();
        }
    }

    void EndDijalog()
    {
        if (dijalogPanel != null) dijalogPanel.SetActive(false);
        System.Action callback = onDijalogComplete;
        onDijalogComplete = null;
        callback?.Invoke();
    }
}
