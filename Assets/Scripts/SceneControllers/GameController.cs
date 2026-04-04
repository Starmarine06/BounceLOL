using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
    public static int scoreChange;
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
    float _time;
    [SerializeField] float _interval = 2f;

    void Start()
    {
        Time.timeScale = 1;
        _time = 0f;
        InvokeRepeating("addScore", 1f, 1f);
    }

    private float lastUpdate = 0f;
    void Update()
    {
        if (!IsGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenuToggle();
        }
        if (score == 20)
        {
            scoreChange = score;
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
        IsGameOver = true;
        StartCoroutine(boomEffect());
        coinsManager.adCoins(score * 10);        
        CancelInvoke("addScore");
        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(MainController.Prefs_BestScore_Key, BestScore);
        }
        
        // Save level-specific score
        int currentLevel = LevelManager.GetCurrentLevel();
        string levelScoreKey = $"Level_{currentLevel}_BestScore";
        int levelBestScore = PlayerPrefs.GetInt(levelScoreKey, 0);
        if (score > levelBestScore)
        {
            PlayerPrefs.SetInt(levelScoreKey, score);
        }
        
        ColorEffect.ColorIndex++;
        PlayerPrefs.SetInt(MainController.Prefs_ColorIndex_Key, ColorEffect.ColorIndex);
        ColorEffect.ColorIndex--;
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
            SceneManager.LoadScene(1);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        IsGameOver = false;
        IsGamePaused = false;
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
