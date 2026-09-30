using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public bool connectedToGooglePlay;
    int score;
    public new GameObject gameObject;
    int maxscore;
    private bool showLeaderBoardOnSignIn;
    private bool showAchievementsOnSignIn;
    private const int MaxAutoSignInRetries = 3;
    private int autoSignInRetries;

    private void Awake()
    {
        PlayGamesPlatform.Activate();
        gameObject.SetActive(false);
    }

    void Start()
    {
        gameObject.SetActive(false);
        LogInToGooglePlay(false);
    }

    private void LogInToGooglePlay(bool interactive = false)
    {
        if (interactive)
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }
        else
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
    }

    public void ShowLeaderBoard()
    {
        if (connectedToGooglePlay)
        {
            Highscore();
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            showLeaderBoardOnSignIn = true;
            LogInToGooglePlay(true);
        }
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            connectedToGooglePlay = true;
            autoSignInRetries = 0;
            Debug.Log("Google Play sign-in successful.");
            if (showLeaderBoardOnSignIn)
            {
                Highscore();
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
                showLeaderBoardOnSignIn = false;
            }
            if (showAchievementsOnSignIn)
            {
                PlayGamesPlatform.Instance.ShowAchievementsUI();
                showAchievementsOnSignIn = false;
            }
        }
        else
        {
            connectedToGooglePlay = false;
            Debug.LogWarning($"Google Play sign-in failed: {status}");
            showLeaderBoardOnSignIn = false;
            showAchievementsOnSignIn = false;

            // Retry a few times only; retrying forever loops the sign-in UI for users who cancelled.
            if (status != SignInStatus.Canceled && autoSignInRetries < MaxAutoSignInRetries)
            {
                autoSignInRetries++;
                StartCoroutine(FailedTime());
            }
        }
    }
    private void Highscore()
    {
        if (connectedToGooglePlay)
        {
            int bestScore = PlayerPrefs.GetInt(GameKeys.BestScore);
            PlayGamesPlatform.Instance.ReportScore(bestScore, GPGSIds.leaderboard_bouncelol, LeaderboardUpdate);
            CheckAchievements(bestScore);
        }
    }

    void LeaderboardUpdate(bool success)
    {
        if (success) Debug.Log("Updates");
        else Debug.Log("Failed");
    }

    IEnumerator FailedTime()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        LogInToGooglePlay();
    }

    public void ShowAchievements()
    {
        if (connectedToGooglePlay)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            showAchievementsOnSignIn = true;
            LogInToGooglePlay(true);
        }
    }

    private void UnlockAchievement(string achievementId)
    {
        if (connectedToGooglePlay)
        {
            PlayGamesPlatform.Instance.ReportProgress(achievementId, 100.0f, AchievementUpdate);
        }
    }

    void AchievementUpdate(bool success)
    {
        if (success) Debug.Log("Achievement unlocked");
        else Debug.Log("Failed to unlock achievement");
    }

    private void CheckAchievements(int currentScore)
    {
        if (currentScore >= 50)
        {
            UnlockAchievement(GPGSIds.achievement_addict);
        }/*
        if (currentScore >= 500)
        {
            UnlockAchievement(GPGSIds.achievement_score_500);
        }*/
        // Add more conditions as needed
    }

    // Call this when the player bounces for the first time or starts playing
    public void UnlockFirstBounce()
    {
        //UnlockAchievement(GPGSIds.achievement_first_bounce);
    }
}
