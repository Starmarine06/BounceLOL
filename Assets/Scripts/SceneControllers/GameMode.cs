using UnityEngine;

public static class GameMode
{
    public const string PrefsVsBotKey = "game_mode_vs_bot";

    public static bool IsVsBot => PlayerPrefs.GetInt(PrefsVsBotKey, 0) == 1;

    public static void SetVsBot(bool vsBot)
    {
        PlayerPrefs.SetInt(PrefsVsBotKey, vsBot ? 1 : 0);
        PlayerPrefs.Save();
    }
}
