using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Read straight from PlayerPrefs so ObstacleGenerator / LevelProgressTracker get the selected
    // level even if they run Awake before this component does (script Awake order is undefined).
    private static int CurrentLevel =>
        Mathf.Clamp(PlayerPrefs.GetInt(GameKeys.SelectedLevel, 1), 1, LevelCount);
    
    // Level configuration: Store all level parameters
    [System.Serializable]
    public struct LevelConfig
    {
        public int level;
        public string title;
        public string difficulty;
        public float speedIncrement;
        public float smooth; // Minimum value is 2
        public string obstaclePrefabName; // Name of the obstacle prefab to use
    }

    private static readonly LevelConfig[] levelConfigs = new LevelConfig[]
    {
        // Level 1 - Easy
        new LevelConfig
        {
            level = 1,
            title = "Level 1",
            difficulty = "NORMAL",
            speedIncrement = 0.004f,
            smooth = 2.0f,
            obstaclePrefabName = "Obstacle_1"
        },
        // Level 2 - Medium
        new LevelConfig
        {
            level = 2,
            title = "Level 2",
            difficulty = "HARD",
            speedIncrement = 0.0055f,
            smooth = 2.0f,
            obstaclePrefabName = "Obstacle_2"
        },
        // Level 3 - Medium-Hard
        new LevelConfig
        {
            level = 3,
            title = "Level 3",
            difficulty = "HARDER",
            speedIncrement = 0.006f,
            smooth = 2.0f,
            obstaclePrefabName = "Obstacle_3"
        },
        // Level 4 - Hard
        new LevelConfig
        {
            level = 4,
            title = "Level 4",
            difficulty = "INSANE",
            speedIncrement = 0.0065f,
            smooth = 2.0f,
            obstaclePrefabName = "Obstacle_4"
        },
        // Level 5 - Very Hard
        new LevelConfig
        {
            level = 5,
            title = "Level 5",
            difficulty = "EXTREME",
            speedIncrement = 0.007f,
            smooth = 2.0f,
            obstaclePrefabName = "Obstacle_5"
        }
    };

    public static int LevelCount => levelConfigs.Length;

    private void Awake()
    {
        // Normalise a bad saved value.
        PlayerPrefs.SetInt(GameKeys.SelectedLevel, CurrentLevel);
    }

    /// <summary>
    /// Set the level to be played (call this from level selection scene)
    /// </summary>
    public static void SetLevel(int level)
    {
        PlayerPrefs.SetInt(GameKeys.SelectedLevel, Mathf.Clamp(level, 1, LevelCount));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Get the current level
    /// </summary>
    public static int GetCurrentLevel()
    {
        return CurrentLevel;
    }

    /// <summary>
    /// Get the configuration for the current level
    /// </summary>
    public static LevelConfig GetCurrentLevelConfig()
    {
        int level = Mathf.Clamp(CurrentLevel, 1, levelConfigs.Length);
        return levelConfigs[level - 1];
    }

    /// <summary>
    /// Get configuration for a specific level
    /// </summary>
    public static LevelConfig GetLevelConfig(int level)
    {
        level = Mathf.Clamp(level, 1, LevelCount);
        return levelConfigs[level - 1];
    }

    /// <summary>
    /// Get level progress (0-100)
    /// </summary>
    public static float GetLevelProgress(int level)
    {
        level = Mathf.Clamp(level, 1, LevelCount);
        return PlayerPrefs.GetFloat(GameKeys.LevelProgress(level), 0f);
    }

    /// <summary>
    /// Set level progress (0-100)
    /// </summary>
    public static void SetLevelProgress(int level, float progress)
    {
        level = Mathf.Clamp(level, 1, LevelCount);
        progress = Mathf.Clamp01(progress); // Clamp between 0 and 1
        PlayerPrefs.SetFloat(GameKeys.LevelProgress(level), progress);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Check if a level is completed
    /// </summary>
    public static bool IsLevelCompleted(int level)
    {
        return GetLevelProgress(level) >= 1.0f;
    }

    /// <summary>
    /// Mark level as completed
    /// </summary>
    public static void CompleteLevelAs(int level, float progress = 1.0f)
    {
        SetLevelProgress(level, progress);
    }

    /// <summary>
    /// Get current level's progress
    /// </summary>
    public static float GetCurrentLevelProgress()
    {
        return GetLevelProgress(CurrentLevel);
    }

    /// <summary>
    /// Set current level's progress
    /// </summary>
    public static void SetCurrentLevelProgress(float progress)
    {
        SetLevelProgress(CurrentLevel, progress);
    }

    /// <summary>
    /// Get count of completed levels
    /// </summary>
    public static int GetCompletedLevelsCount()
    {
        int count = 0;
        for (int i = 1; i <= LevelCount; i++)
        {
            if (IsLevelCompleted(i))
                count++;
        }
        return count;
    }
}
