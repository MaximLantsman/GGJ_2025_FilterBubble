using UnityEngine;

[CreateAssetMenu(menuName = "News Article Scriptable")]
public class NewsScriptable : ScriptableObject
{
    public string newstitle;
    public Sprite newsImage;

    public float bluePercent;
    public float redPercent;
    public float stressPercent;
    
    public newsType type;
}

[System.Serializable]
public enum newsType
{
    Blue,
    Red,
    Green,
    Yellow
}

