using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    [Header("Navigation")]
    public Button backButton;
    public Button frontButton;
    public Button playButton;
    public Button homeButton;

    [Header("Level Display")]
    public Text levelTitle;
    public Text difficultyText;
    public Text difficultyIcon; // Optional: for color coding
    public Slider progressSlider;
    public Text progressText;

    [Header("Colors")]
    public Color[] difficultyColors = new Color[5];

    private int currentPreviewLevel = 1;
    private const int MaxLevels = 5;

    private void Start()
    {
        // Load the last selected level
        currentPreviewLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        currentPreviewLevel = Mathf.Clamp(currentPreviewLevel, 1, MaxLevels);

        // Setup button listeners
        if (backButton != null)
            backButton.onClick.AddListener(PreviousLevel);
        
        if (frontButton != null)
            frontButton.onClick.AddListener(NextLevel);
        
        if (playButton != null)
            playButton.onClick.AddListener(PlayCurrentLevel);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(GoHome);

        // Setup progress slider
        if (progressSlider != null)
        {
            progressSlider.interactable = false; // Read-only display
            progressSlider.minValue = 0;
            progressSlider.maxValue = 1;
        }

        // Initialize difficulty colors if not set
        if (difficultyColors.Length == 0 || difficultyColors[0] == Color.clear)
        {
            difficultyColors = new Color[5]
            {
                new Color(0.2f, 1f, 0.2f),    // Green - Normal
                new Color(1f, 0.8f, 0.2f),   // Yellow - Hard
                new Color(1f, 0.5f, 0.2f),   // Orange - Harder
                new Color(1f, 0.2f, 0.2f),   // Red - Insane
                new Color(0.8f, 0.2f, 1f)    // Purple - Extreme
            };
        }

        // Display initial level
        UpdateLevelDisplay();
    }

    private void PreviousLevel()
    {
        currentPreviewLevel--;
        if (currentPreviewLevel < 1)
            currentPreviewLevel = MaxLevels;
        
        UpdateLevelDisplay();
    }

    private void NextLevel()
    {
        currentPreviewLevel++;
        if (currentPreviewLevel > MaxLevels)
            currentPreviewLevel = 1;
        
        UpdateLevelDisplay();
    }

    private void UpdateLevelDisplay()
    {
        LevelManager.LevelConfig config = LevelManager.GetLevelConfig(currentPreviewLevel);
        
        // Update title
        if (levelTitle != null)
            levelTitle.text = config.title;
        
        // Update difficulty
        if (difficultyText != null)
            difficultyText.text = config.difficulty;
        
        // Update difficulty color
        if (difficultyIcon != null)
        {
            difficultyIcon.color = difficultyColors[currentPreviewLevel - 1];
        }

        // Get best score from PlayerPrefs
        string scoreKey = $"Level_{currentPreviewLevel}_BestScore";
        int levelBestScore = PlayerPrefs.GetInt(scoreKey, 0);
        
        // Update progress bar (divide by 100)
        float normalizedProgress = levelBestScore / 100f;
        if (progressSlider != null)
            progressSlider.value = normalizedProgress;
        
        // Update progress text with the actual score
        if (progressText != null)
            progressText.text = levelBestScore.ToString();

        Debug.Log($"Previewing Level {currentPreviewLevel}: {config.title} ({config.difficulty}) - Best Score: {levelBestScore}");
    }

    private void PlayCurrentLevel()
    {
        LevelManager.SetLevel(currentPreviewLevel);
        PlayerPrefs.SetInt("SelectedLevel", currentPreviewLevel);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("Play");
    }

    private void GoHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Called from Play scene when level is completed
    /// </summary>
    public static void UpdateLevelProgressFromPlay(int level, float progress)
    {
        LevelManager.SetLevelProgress(level, progress);
    }
}
