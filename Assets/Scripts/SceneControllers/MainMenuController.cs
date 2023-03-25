using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject QuitPopupUI;
    public Text BestScoreText;
    public InterstitialAd adsManager;

    void Awake()
    {
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
        float chance = Random.Range(1, 6);
        Debug.Log(chance);
        if (chance == 1)
        {
            adsManager.LoadAd();
            adsManager.ShowAd();
        }
        else
        {
            SceneManager.LoadScene(1);
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
