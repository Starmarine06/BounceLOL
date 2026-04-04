using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelProgressTracker : MonoBehaviour
{
    public static LevelProgressTracker Instance { get; private set; }

    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text progressText;
    [SerializeField] private float maxScoreForCompletion = 1000f; // Score needed to "complete" level

    private float currentProgress = 0f;
    private int currentLevel;
    private int levelBestScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Get current level
        currentLevel = LevelManager.GetCurrentLevel();

        // Setup progress slider
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = 1;
            progressSlider.interactable = false;
        }

        // Load best score for this level from PlayerPrefs
        LoadLevelBestScore();
        UpdateProgressUI();
    }

    private void LoadLevelBestScore()
    {
        string key = $"Level_{currentLevel}_BestScore";
        levelBestScore = PlayerPrefs.GetInt(key, 0);
        
        // Calculate progress from saved best score
        currentProgress = Mathf.Min(levelBestScore / maxScoreForCompletion, 1.0f);
        
        Debug.Log($"Loaded Level {currentLevel} best score: {levelBestScore}");
    }

    /// <summary>
    /// Save the best score for this level to PlayerPrefs
    /// </summary>
    private void SaveLevelBestScore()
    {
        string key = $"Level_{currentLevel}_BestScore";
        PlayerPrefs.SetInt(key, levelBestScore);
        PlayerPrefs.Save();
        
        Debug.Log($"Saved Level {currentLevel} best score: {levelBestScore}");
    }

    /// <summary>
    /// Update level progress (0-1 scale) - based on best score
    /// </summary>
    public void UpdateProgress(float normalizedProgress)
    {
        currentProgress = Mathf.Clamp01(normalizedProgress);
        LevelManager.SetLevelProgress(currentLevel, currentProgress);
        UpdateProgressUI();
    }

    /// <summary>
    /// Update progress based on score - saves to PlayerPrefs
    /// </summary>
    public void UpdateProgressByScore(int score)
    {
        if (score > levelBestScore)
        {
            levelBestScore = score;
            SaveLevelBestScore();
        }
        
        float progress = Mathf.Min(levelBestScore / maxScoreForCompletion, 1.0f);
        UpdateProgress(progress);
    }

    /// <summary>
    /// Update the progress UI elements
    /// </summary>
    private void UpdateProgressUI()
    {
        if (progressSlider != null)
            progressSlider.value = currentProgress;
        
        if (progressText != null)
            progressText.text = (currentProgress * 100f).ToString("F0") + "%";
    }

    /// <summary>
    /// Complete the level with final score
    /// </summary>
    public void CompleteLevelWith(int finalScore)
    {
        if (finalScore > levelBestScore)
        {
            levelBestScore = finalScore;
            SaveLevelBestScore();
        }
        
        currentProgress = Mathf.Min(levelBestScore / maxScoreForCompletion, 1.0f);
        LevelManager.SetLevelProgress(currentLevel, currentProgress);
        UpdateProgressUI();
        
        Debug.Log($"Level {currentLevel} completed with best score: {levelBestScore}");
    }

    /// <summary>
    /// Get current level's best score
    /// </summary>
    public int GetLevelBestScore()
    {
        return levelBestScore;
    }

    /// <summary>
    /// Get current progress
    /// </summary>
    public float GetCurrentProgress()
    {
        return currentProgress;
    }

    /// <summary>
    /// Return to level selection
    /// </summary>
    public void ReturnToLevelSelection()
    {
        LevelManager.SetLevelProgress(currentLevel, currentProgress);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Retry current level
    /// </summary>
    public void RetryLevel()
    {
        SceneManager.LoadScene("Play");
    }
}
