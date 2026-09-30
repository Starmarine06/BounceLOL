using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static bool IsGameOver;
    public static bool IsGamePaused;

    public ParticleSystem boom;
    public GameObject Player;
    public GameObject PlayingUI;
    public GameObject PauseMenuUI;
    public GameObject GameOverUI;
    public Text PlayingScoreText;
    [Tooltip("Optional. When vs-bot mode is on, shows bot score.")]
    public Text BotPlayingScoreText;
    public Text GameOverScoreText;
    public Text GameOverBestScoreText;
    [Tooltip("Optional. Shown on game over when vs-bot mode: You win / Bot wins / Tie.")]
    public Text VsBotResultText;

    private int score;
    private int botScore;
    private int bestScore;
    private static float playingBackgroungMusicTime;
    private AudioSource playingBackgroungMusic;

    public InterstitialAds adsManager;

    public CoinsManager coinsManager;

    public int Score
    {
        get { return score; }
        set
        {
            if (value >= 0)
            {
                score = value;
            }
            else
            {
                score = 0;
            }
            RefreshPlayingScoreTexts();
        }
    }

    public int BestScore
    {
        get { return bestScore; }
        set
        {
            if (value >= 0)
            {
                bestScore = value;
            }
            else
            {
                bestScore = 0;
            }
            GameOverBestScoreText.text = "Best Score: " + bestScore;
        }
    }

    void Awake()
    {
        IsGameOver = false;
        IsGamePaused = false;
        playingBackgroungMusic = GetComponent<AudioSource>();
        playingBackgroungMusic.time = playingBackgroungMusicTime;
        Score = 0;
        BestScore = PlayerPrefs.GetInt(MainController.Prefs_BestScore_Key, MainController.Prefs_BestScore_DefaultValue);
        PlayingUI.SetActive(true);
        GameOverUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        botScore = 0;
        if (VsBotResultText != null)
        {
            VsBotResultText.text = string.Empty;
        }
        RefreshPlayingScoreTexts();
    }
    void Start()
    {
        Time.timeScale = 1;
        InvokeRepeating("addScore", 1f, 1f);
    }
    void Update()
    {
        if (!IsGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenuToggle();
        }
    }
    private void addScore()
    {
        score++;
        if (GameMode.IsVsBot)
        {
            botScore += 1 + Random.Range(0, 2);
        }
        RefreshPlayingScoreTexts();
    }

    private void RefreshPlayingScoreTexts()
    {
        if (PlayingScoreText == null) return;

        if (GameMode.IsVsBot && BotPlayingScoreText != null)
        {
            PlayingScoreText.text = "You: " + score;
            BotPlayingScoreText.text = "Bot: " + botScore;
        }
        else
        {
            PlayingScoreText.text = "Score: " + score;
            if (BotPlayingScoreText != null)
            {
                BotPlayingScoreText.text = string.Empty;
            }
        }
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        StartCoroutine(boomEffect());
        CoinWallet.Add(score * 10);
        CancelInvoke("addScore");
        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(MainController.Prefs_BestScore_Key, BestScore);
        }
        
        // LevelProgressTracker owns the per-level best score; only fall back to writing it
        // directly when a scene has no tracker.
        if (LevelProgressTracker.Instance != null)
        {
            LevelProgressTracker.Instance.UpdateProgressByScore(score);
        }
        else
        {
            string levelScoreKey = GameKeys.LevelBestScore(LevelManager.GetCurrentLevel());
            if (score > PlayerPrefs.GetInt(levelScoreKey, 0))
            {
                PlayerPrefs.SetInt(levelScoreKey, score);
            }
        }

        // Advance the saved theme colour for the next run (wraps around the palette).
        PlayerPrefs.SetInt(MainController.Prefs_ColorIndex_Key,
            (ColorEffect.ColorIndex + 1) % ColorEffect.ThemeColors.Length);
        PlayingUI.SetActive(false);
        GameOverScoreText.text = "SCORE\n" + score;
        GameOverBestScoreText.text = "BEST SCORE\n" + bestScore;
        if (GameMode.IsVsBot && VsBotResultText != null)
        {
            if (score > botScore)
            {
                VsBotResultText.text = "You win!";
            }
            else if (score < botScore)
            {
                VsBotResultText.text = "Bot wins!";
            }
            else
            {
                VsBotResultText.text = "Tie!";
            }
        }
        else if (VsBotResultText != null)
        {
            VsBotResultText.text = string.Empty;
        }

        playingBackgroungMusic.Pause();
        playingBackgroungMusicTime = playingBackgroungMusic.time;
        PlayerPrefs.Save();
    }
    IEnumerator boomEffect()
    {
        boom.transform.position = Player.transform.position;
        boom.Play();
        yield return new WaitForSeconds(0.125f);
        GameOverUI.SetActive(true);
        Time.timeScale = 0;        
    }

    public void Restart()
    {
        if (adsManager != null)
        {
            adsManager.ShowAdOrLoadScene();
        }
        else
        {
            SceneManager.LoadScene(GameKeys.PlayScene);
        }
    }

    public void MainMenu()
    {
        IsGameOver = false;
        IsGamePaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(GameKeys.MainMenuScene);
    }

    void OnApplicationPause(bool paused)
    {
        // Returning from the home screen mid-fall would otherwise be an unfair instant death.
        if (paused && !IsGameOver && !IsGamePaused && PauseMenuUI != null)
        {
            PauseMenuToggle();
        }
    }

    public void PauseMenuToggle()
    {
        if (!PauseMenuUI.activeSelf)
        {
            IsGamePaused = true;
            Time.timeScale = 0;
            playingBackgroungMusic.Pause();
            PauseMenuUI.SetActive(true);
        }
        else
        {
            IsGamePaused = false;
            PauseMenuUI.SetActive(false);
            if (PlayerPrefs.GetInt("sounds", 1) == 1)
            {
                playingBackgroungMusic.UnPause();
            }
            Time.timeScale = 1;
        }
    }
    
}
