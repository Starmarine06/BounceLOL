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
    public GameObject gameObject;
    int maxscore;

    private void Awake()
    {
        PlayGamesPlatform.Activate();
        gameObject.SetActive(false);
    }

    void Start()
    {
        gameObject.SetActive(false);
        LogInToGooglePlay();
    }

    private void LogInToGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ShowLeaderBoard()
    {
        if (!connectedToGooglePlay) LogInToGooglePlay();
        Social.ShowLeaderboardUI();
        Highscore();
        
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            connectedToGooglePlay = true;
        }
        else
        {
            connectedToGooglePlay = false;
            StartCoroutine(FailedTime());
        }
    }
    private void Highscore()
    {
        if (connectedToGooglePlay)
        {
            Social.ReportScore(PlayerPrefs.GetInt("best_score"), GPGSIds.leaderboard_bouncelol, LeaderboardUpdate);
        }
    }

    private void LeaderboardUpdate(bool success)
    {
        if (success) Debug.Log("Updates");
        else Debug.Log("Failed");
    }

    IEnumerator FailedTime()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
