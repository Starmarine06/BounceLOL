using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject QuitPopupUI;
    public Text BestScoreText;
    public InterstitialAds adsManager;

    void Awake()
    {
        // A previous run may have ended with timeScale 0; menus must never start frozen.
        Time.timeScale = 1;
        QuitPopupUI.SetActive(false);
        BestScoreText.text = "BEST SCORE\n" + PlayerPrefs.GetInt(MainController.Prefs_BestScore_Key, MainController.Prefs_BestScore_DefaultValue);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitPopupToggle();
        }
    }

    public void Play()
    {
        GameMode.SetVsBot(false);
        LoadGameScene();
    }

    public void PlayVsBot()
    {
        GameMode.SetVsBot(true);
        LoadGameScene();
    }

    private void LoadGameScene()
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

    public void QuitPopupToggle()
    {
        if (!QuitPopupUI.activeSelf)
        {
            QuitPopupUI.SetActive(true);
        }
        else
        {
            // A previous run may have ended with timeScale 0; menus must never start frozen.
        Time.timeScale = 1;
        QuitPopupUI.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(int Scene)
    {
        SceneManager.LoadScene(Scene);
    }
}
