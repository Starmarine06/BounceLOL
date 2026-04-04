using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static int CurrentLevel;
    
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

    private static LevelConfig[] levelConfigs = new LevelConfig[5]
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

    private void Awake()
    {
        // Load the level that was selected in another scene
        CurrentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        
        // Clamp level to valid range
        CurrentLevel = Mathf.Clamp(CurrentLevel, 1, 5);
        
        // Save it back so it persists
        PlayerPrefs.SetInt("SelectedLevel", CurrentLevel);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Set the level to be played (call this from level selection scene)
    /// </summary>
    public static void SetLevel(int level)
    {
        CurrentLevel = Mathf.Clamp(level, 1, 5);
        PlayerPrefs.SetInt("SelectedLevel", CurrentLevel);
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
        return levelConfigs[CurrentLevel - 1];
    }

    /// <summary>
    /// Get configuration for a specific level
    /// </summary>
    public static LevelConfig GetLevelConfig(int level)
    {
        level = Mathf.Clamp(level, 1, 5);
        return levelConfigs[level - 1];
    }

    /// <summary>
    /// Get level progress (0-100)
    /// </summary>
    public static float GetLevelProgress(int level)
    {
        level = Mathf.Clamp(level, 1, 5);
        string progressKey = $"Level_{level}_Progress";
        return PlayerPrefs.GetFloat(progressKey, 0f);
    }

    /// <summary>
    /// Set level progress (0-100)
    /// </summary>
    public static void SetLevelProgress(int level, float progress)
    {
        level = Mathf.Clamp(level, 1, 5);
        progress = Mathf.Clamp01(progress); // Clamp between 0 and 1
        string progressKey = $"Level_{level}_Progress";
        PlayerPrefs.SetFloat(progressKey, progress);
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
        for (int i = 1; i <= 5; i++)
        {
            if (IsLevelCompleted(i))
                count++;
        }
        return count;
    }
}
