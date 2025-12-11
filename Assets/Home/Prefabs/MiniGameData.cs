using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameData", menuName = "Game/MiniGameData")]
public class MiniGameData : ScriptableObject
{
    public string sceneName;
    public Sprite icon;
    public int totalLevels;
}

