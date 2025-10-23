using UnityEngine;

[CreateAssetMenu(fileName = "LevelTimerConfig", menuName = "Game/Level Timer Config")]
public class LevelTimerConfig : ScriptableObject
{
    [Tooltip("Time limit in seconds for this level")]
    public float timeLimit = 60f;

    [Tooltip("Name of the level or scene (optional)")]
    public string levelName;
}