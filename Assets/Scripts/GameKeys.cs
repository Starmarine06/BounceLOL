/// <summary>
/// Single source of truth for PlayerPrefs keys and scene names.
/// The string values match what earlier builds already saved, so existing progress is kept.
/// </summary>
public static class GameKeys
{
    public const string Coins = "Coins";
    public const string SelectedLevel = "SelectedLevel";
    public const string SelectedSkin = "selectedOption";
    public const string SelectedTrail = "Trail";
    public const string BestScore = "best_score";

    public const string MainMenuScene = "MainMenu";
    public const string PlayScene = "Play";

    public static string LevelBestScore(int level) => $"Level_{level}_BestScore";
    public static string LevelProgress(int level) => $"Level_{level}_Progress";
}
