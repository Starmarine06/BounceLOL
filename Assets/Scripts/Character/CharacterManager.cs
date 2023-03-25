using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;

    public Text nameText;
    public SpriteRenderer artworkSprite;

    public Text titleText;

    public AudioSource audioSource;

    private int selectedOption = 0;
    void Start()
    {
        //Selected Option
        if (PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedOption);
        PlayerPrefs.SetInt("0", 1);
        titleText.enabled = false;
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Audio") == 1)
        {
            audioSource.enabled = true;
        }
        else
        {
            audioSource.enabled = false;
        }
    }

    public void NextOption()
    {
        selectedOption++;
        if(selectedOption >= characterDB.characterCount)
        {
            selectedOption = 0;
        }

        UpdateCharacter(selectedOption);
        Save();
    }
    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = characterDB.characterCount -1;
        }

        UpdateCharacter(selectedOption);
        Save();
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption);
        artworkSprite.sprite = character.characterSprite;
        nameText.text = character.characterName;
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("selectedOption", selectedOption);
    }

    public void ChangeScene(int sceneID)
    {
        if(PlayerPrefs.GetInt(PlayerPrefs.GetInt("selectedOption").ToString()) == 1)
        {
            SceneManager.LoadScene(sceneID);
        }
        else
        {
            StartCoroutine(Scene());
        }
    }

    IEnumerator Scene()
    {
        titleText.enabled = true;
        yield return new WaitForSeconds(1);
        titleText.enabled = false;
    }
}
