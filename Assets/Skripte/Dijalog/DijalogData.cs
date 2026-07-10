using UnityEngine;

[System.Serializable]
public class DijalogLine
{
    public string speakerName;
    [TextArea(2, 5)] public string text;
    public Sprite portrait;
}

[CreateAssetMenu(fileName = "NoviDijalog", menuName = "4Elements/Dijalog")]
public class DijalogData : ScriptableObject
{
    public DijalogLine[] lines;
}
