using UnityEngine;

public class DijalogTrigger : MonoBehaviour
{
    public DijalogData dijalog;
    public bool triggerOnce = true;
    bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;

        DijalogManager.Instance.StartDijalog(dijalog);
        hasTriggered = true;
    }
}