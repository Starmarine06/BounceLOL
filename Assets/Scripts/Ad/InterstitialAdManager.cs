using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InterstitialAdManager : MonoBehaviour
{
    public Text text;
    void Awake()
    {
        Time.timeScale= 1f;
        StartCoroutine(Ads());
    }
    IEnumerator Ads()
    {
        text.text = 3.ToString();
        yield return new WaitForSeconds(1);
        text.text = 2.ToString();
        yield return new WaitForSeconds(1);
        text.text = 1.ToString();
        yield return new WaitForSeconds(1);
        text.text = "Go!";
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }
}
