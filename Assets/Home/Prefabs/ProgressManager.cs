using UnityEngine;

public static class ProgressManager
{
    public static int GetProgress(string sceneName)
    {
        return PlayerPrefs.GetInt($"MiniGame_{sceneName}_Progress", 0);
    }

    public static void SetProgress(string sceneName, int value)
    {
        PlayerPrefs.SetInt($"MiniGame_{sceneName}_Progress", value);
    }

    public static void SetDone(string sceneName)
    {
        PlayerPrefs.SetInt($"MiniGame_{sceneName}_Done", 1);
    }
    public static int GetDone(string sceneName)
    {
        return PlayerPrefs.GetInt($"MiniGame_{sceneName}_Done", 0);
    }
}
